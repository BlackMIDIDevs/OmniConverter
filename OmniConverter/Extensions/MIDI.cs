using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIDIModificationFramework;
using MIDIModificationFramework.MIDIEvents;

namespace OmniConverter
{
    public class MIDI : IDisposable
    {
        private MidiFile LoadedFile;
        private IEnumerable<MIDIEvent> MetaEvents;
        private bool _disposed = false;

        private MIDI(long I, string FN, string FP, MidiFile LF, IEnumerable<MIDIEvent> ME, TimeSpan TL, int T, long NC, ulong S)
        {
            ID = I;
            Name = FN;
            Path = FP;
            TimeLength = TL;
            Tracks = T;
            NoteCount = NC;
            Size = S;
            LoadedFile = LF;
            MetaEvents = ME;
        }

        public IEnumerable<MIDIEvent> GetSingleTrackTimeBased(int track) =>
            LoadedFile.GetTrack(track).MergeWith(MetaEvents).MakeTimeBased(LoadedFile.PPQ);

        public IEnumerable<MIDIEvent> GetFullMIDITimeBased() =>
            LoadedFile.IterateTracks().MergeAll().MakeTimeBased(LoadedFile.PPQ);

        public IEnumerable<IEnumerable<MIDIEvent>> GetIterateTracksTimeBased() =>
            LoadedFile.IterateTracks().Select(track => track.MergeWith(MetaEvents).MakeTimeBased(LoadedFile.PPQ));

        public long ID { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public TimeSpan TimeLength { get; private set; }
        public int Tracks { get; private set; }
        public long NoteCount { get; private set; }
        public ulong Size { get; private set; }

        public static MIDI LoadFromFile(long id, string filepath, string name, Action<int, int> progressCallback)
        {
            var file = new MidiFile(filepath);
            ulong fileSize = (ulong)new FileInfo(filepath).Length;

            double maxTicks = 0;
            long noteCount = 0;
            object l = new object();

            var midiMetaEvents = new List<IEnumerable<MIDIEvent>>();
            int tracksParsed = 0;

            // loop over all tracks in parallel
            Parallel.ForEach(file.IterateTracks(), ev =>
            {
                double time = 0.0;
                int nc = 0;
                double delta = 0.0;
                var trackMetaEvents = new List<MIDIEvent>();
                foreach (var e in ev)
                {
                    time += e.DeltaTime;

                    // checking the most common events first for efficiency
                    if (e is NoteOnEvent)
                    {
                        nc++;
                        delta += e.DeltaTime;
                        continue;
                    }
                    else if (e is NoteOffEvent)
                    {
                        delta += e.DeltaTime;
                        continue;
                    }
                    else if (
                        e is TempoEvent ||
                        e is ControlChangeEvent ||
                        e is ProgramChangeEvent ||
                        e is ChannelPressureEvent ||
                        e is PitchWheelChangeEvent)
                    {
                        e.DeltaTime += delta;
                        delta = 0;
                        trackMetaEvents.Add(e);
                    }
                    else
                    {
                        delta += e.DeltaTime;
                    }
                }

                lock (l)
                {
                    tracksParsed++;
                    noteCount += nc;
                    if (maxTicks < time) maxTicks = time;
                    midiMetaEvents.Add(trackMetaEvents);
                    progressCallback(tracksParsed, file.TrackCount);
                }
            });

            var mergedMetaEvents = midiMetaEvents.MergeAll().ToArray();

            // get midi length in seconds
            var mergedWithLength = mergedMetaEvents.MergeWith(new[] { new EndOfExclusiveEvent(maxTicks) });
            double seconds = 0.0;
            foreach (var e in mergedWithLength.MakeTimeBased(file.PPQ))
            {
                seconds += e.DeltaTime;
            }

            return new MIDI(id, name, filepath, file, mergedMetaEvents, TimeSpan.FromSeconds(seconds), file.TrackCount, noteCount, fileSize);
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

            if (disposing)
                LoadedFile.Dispose();

            MetaEvents = Enumerable.Empty<MIDIEvent>();
            ID = 0;
            Name = string.Empty;
            Path = string.Empty;
            TimeLength = TimeSpan.Zero;
            Tracks = 0;
            NoteCount = 0;
            Size = 0;

            _disposed = true;
        }
    }
}
