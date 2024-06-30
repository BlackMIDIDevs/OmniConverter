using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MIDIModificationFramework;
using MIDIModificationFramework.MIDIEvents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Xml.Linq;
using Avalonia.Platform.Storage;
using static System.Net.WebRequestMethods;
using Octokit;

namespace OmniConverter
{
    public class EventTools
    {
        enum MIDIEventType
        {
            NoteOff = 0x80,
            NoteOn = 0x90,
            Aftertouch = 0xA0,
            CC = 0xB0,
            PatchChange = 0xC0,
            ChannelPressure = 0xD0,
            PitchBend = 0xE0,

            SystemMessageStart = 0xF0,
            SystemMessageEnd = 0xF7,

            MIDITCQF = 0xF1,
            SongPositionPointer = 0xF2,
            SongSelect = 0xF3,
            TuneRequest = 0xF6,
            TimingClock = 0xF8,
            Start = 0xFA,
            Continue = 0xFB,
            Stop = 0xFC,
            ActiveSensing = 0xFE,
            SystemReset = 0xFF,

            Unknown1 = 0xF4,
            Unknown2 = 0xF5,
            Unknown3 = 0xF9,
            Unknown4 = 0xFD
        };

        static uint GetStatus(uint ev) { return (ev & 0xFF); }
        static uint GetCommand(uint ev) { return (ev & 0xF0); }
        static uint GetChannel(uint ev) { return (ev & 0xF); }
        static uint GetFirstParam(uint ev) { return ((ev >> 8) & 0xFF); }
        static uint GetSecondParam(uint ev) { return ((ev >> 16) & 0xFF); }
    }

    public class MIDI : ObservableObject
    {
        public int MetaEventsCount => _metaEvent.Count();
        public string Name { get => _name; }
        public long ID { get => _id; }
        public string Path { get => _path; }
        public TimeSpan Length { get => _timeLength; }
        public int Tracks { get => _tracks; }
        public long Notes { get => _noteCount; }
        public ulong Size { get => _fileSize; }
        public string HumanReadableSize { get => MiscFunctions.BytesToHumanReadableSize(_fileSize); }

        public string HumanReadableTime { get => MiscFunctions.TimeSpanToHumanReadableTime(_timeLength); }

        private MidiFile? _loadedFile;
        private IEnumerable<MIDIEvent> _metaEvent;
        private bool _disposed = false;
        private string _name;
        private long _id;
        private string _path;
        private TimeSpan _timeLength;
        private int _tracks;
        private long _noteCount;
        private ulong _fileSize;

        public MIDI(MidiFile? loadedFile, IEnumerable<MIDIEvent> metaEvent, string name, long id, string path, TimeSpan timeLength, int tracks, long noteCount, ulong fileSize)
        {
            _loadedFile = loadedFile;
            _metaEvent = metaEvent;
            _name = name;
            _id = id;
            _path = path;
            _timeLength = timeLength;
            _tracks = tracks;
            _noteCount = noteCount;
            _fileSize = fileSize;
        }

        public MIDI(string test)
        {
            _name = test;
        }

        public IEnumerable<MIDIEvent> GetSingleTrackTimeBased(int track) =>
            _loadedFile.GetTrackUnsafe(track).MergeWith(_metaEvent).MakeTimeBased(_loadedFile.PPQ);

        public IEnumerable<MIDIEvent> GetFullMIDITimeBased() =>
            _loadedFile.IterateTracks().MergeAll().MakeTimeBased(_loadedFile.PPQ);

        public IEnumerable<IEnumerable<MIDIEvent>> GetIterateTracksTimeBased() =>
            _loadedFile.IterateTracks().Select(track => track.MergeWith(_metaEvent).MakeTimeBased(_loadedFile.PPQ));

        public static List<IEnumerable<MIDIEvent>> GetMetaEvents(IEnumerable<IEnumerable<MIDIEvent>> tracks, ParallelOptions parallelOptions, ref double maxTicks, ref long noteCount, Action<int, int> progressCallback)
        {
            object l = new object();

            var midiMetaEvents = new List<IEnumerable<MIDIEvent>>();
            int tracksParsed = 0;
            long tNoteCount = 0;
            double tMaxTicks = 0;

            // loop over all tracks in parallel
            Parallel.For(0, tracks.Count(), parallelOptions, T =>
            {
                double time = 0.0;
                int nc = 0;
                double delta = 0.0;
                var trackMetaEvents = new List<MIDIEvent>();

                try
                {
                    var tr = tracks.ElementAt(T);

                    if (tr == null)
                        return;

                    foreach (var e in tr)
                    {
                        if (parallelOptions.CancellationToken.IsCancellationRequested)
                            parallelOptions.CancellationToken.ThrowIfCancellationRequested();

                        time += e.DeltaTime;

                        switch (e)
                        {
                            case NoteOnEvent fev:
                                nc++;
                                delta += e.DeltaTime;

                                break;

                            case TempoEvent tev:
                            case ControlChangeEvent ccev:
                            case ProgramChangeEvent pcev:
                            case ChannelPressureEvent cpev:
                            case PitchWheelChangeEvent pwcev:
                                e.DeltaTime += delta;
                                delta = 0;
                                trackMetaEvents.Add(e);

                                break;

                            default:
                                delta += e.DeltaTime;
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.PrintToConsole(Debug.LogType.Error, e.ToString());
                }

                lock (l)
                {
                    tracksParsed++;
                    tNoteCount += nc;
                    if (tMaxTicks < time) tMaxTicks = time;
                    midiMetaEvents.Add(trackMetaEvents);
                    progressCallback(tracksParsed, tracks.Count());
                }
            });

            noteCount = tNoteCount;
            maxTicks = tMaxTicks;

            return midiMetaEvents;
        }

        public static MIDI? Load(long id, string filepath, string name, ParallelOptions parallelOptions, Action<int, int> progressCallback)
        {
            var file = new MidiFile(filepath);
            ulong fileSize = (ulong)new FileInfo(filepath).Length;

            try
            {
                double maxTicks = 0;
                long noteCount = 0;

                var Tracks = file.IterateTracks();
                var midiMetaEvents = GetMetaEvents(Tracks, parallelOptions, ref maxTicks, ref noteCount, progressCallback);
                var mergedMetaEvents = midiMetaEvents.MergeAll().ToArray();

                // get midi length in seconds
                var mergedWithLength = mergedMetaEvents.MergeWith(new[] { new EndOfExclusiveEvent(maxTicks) });
                double seconds = 0.0;
                foreach (var e in mergedWithLength.MakeTimeBased(file.PPQ))
                {
                    seconds += e.DeltaTime;
                }

                return new MIDI(file, mergedMetaEvents, name, id, filepath, TimeSpan.FromSeconds(seconds), file.TrackCount, noteCount, fileSize);
            }
            catch (OperationCanceledException) { }
            catch (Exception) { }

            file.Dispose();
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && _loadedFile != null)
                _loadedFile.Dispose();

            _metaEvent = Enumerable.Empty<MIDIEvent>();
            _id = 0;
            _name = string.Empty;
            _path = string.Empty;
            _timeLength = TimeSpan.Zero;
            _tracks = 0;
            _noteCount = 0;
            _fileSize = 0;

            _disposed = true;
        }

        public static FilePickerFileType MidiAll { get; } = new("MIDIs")
        {
            Patterns = new[] { "*.mid", "*.midi", "*.mff", "*.smf", "*.kar" },
            AppleUniformTypeIdentifiers = new[] { "midi" },
            MimeTypes = new[] { "audio/midi" }
        };
    }

    public class MIDIValidator
    {
        private string _currentMidi;
        private ulong _valid;
        private ulong _notvalid;
        private ulong _total;

        private int _tracks = 0;
        private int _curTrack = 0;

        public MIDIValidator(ulong total)
        {
            _currentMidi = "";
            _valid = 0;
            _notvalid = 0;
            _tracks = 0;
            _curTrack = 0;
            _total = total;
        }

        public void SetCurrentMIDI(string midi) { _currentMidi = midi; }
        public string GetCurrentMIDI() { return _currentMidi; }
        public void AddValidMIDI() { _valid++; }
        public void AddInvalidMIDI() { _notvalid++; }
        public ulong GetValidMIDIsCount() { return _valid; }
        public ulong GetInvalidMIDIsCount() { return _notvalid; }
        public ulong GetTotalMIDIsCount() { return _total; }

        public void SetTotalTracks(int tracks) { _tracks = tracks; }
        public void AddTrack() { _curTrack++; }
        public void ResetCurrentTrack() { _curTrack = 0; }
        public int GetTotalTracks() { return _tracks; }
        public int GetCurrentTrack() { return _curTrack; }
    }

    public abstract class MIDIWorker : IDisposable
    {
        public abstract void Dispose();
        public abstract string GetStatus();
        public abstract double GetProgress();

        public abstract bool IsRunning();

        public abstract bool StartWork();
        public abstract void CancelWork();
    }
}
