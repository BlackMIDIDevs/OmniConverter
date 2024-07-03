using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Monad.FLParser;
using MIDIModificationFramework;
using MIDIModificationFramework.Generator;
using MIDIModificationFramework.MIDIEvents;
using Avalonia.Platform.Storage;

using Note = MIDIModificationFramework.Note;

namespace OmniConverter
{
    /*
        Base code by Kaydax, from flp2midi: https://github.com/Kaydax/flp2midi

        # DON'T BE A DICK PUBLIC LICENSE

        > Version 1.1, December 2016

        > Copyright (C) 2020 Kaydax

        Everyone is permitted to copy and distribute verbatim or modified
        copies of this license document.

        > DON'T BE A DICK PUBLIC LICENSE
        > TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION

        1. Do whatever you like with the original work, just don't be a dick.

           Being a dick includes - but is not limited to - the following instances:

         1a. Outright copyright infringement - Don't just copy this and change the name.
         1b. Selling the unmodified original with no work done what-so-ever, that's REALLY being a dick.
         1c. Modifying the original work to contain hidden harmful content. That would make you a PROPER dick.

        2. If you become rich through modifications, related works/services, or supporting the original work,
        share the love. Only a dick would make loads off this work and not buy the original work's
        creator(s) a pint.

        3. Code is provided with no warranty. Using somebody else's code and bitching when it goes wrong makes
        you a DONKEY dick. Fix the problem yourself. A non-dick would submit the fix back.
    */

    public class FLP : MIDI
    {
        static bool ForceColor { get; set; }
        static bool DisableEcho { get; set; }

        public FLP(MidiFile? loadedFile, IEnumerable<MIDIEvent> metaEvent, string name, long id, string path, TimeSpan timeLength, int tracks, long noteCount, ulong fileSize, ulong[] eventCounts) : base(loadedFile, metaEvent, name, id, path, timeLength, tracks, noteCount, fileSize, eventCounts)
        { }

        public FLP(string test) : base(test)
        { }

        //TODO: Actually support feed correctly
        static IEnumerable<Note> EchoNotes(IEnumerable<Note> notes, byte echoAmount, uint feed, uint time, int ppq)
        {
            if (feed == 0) return notes;

            List<IEnumerable<Note>> echoes = [notes];

            for (var i = 1; i <= echoAmount; i++)
            {
                var shifted = notes.OffsetTime((time * i * ppq) / 96.0 / 2);
                echoes.Add(shifted);
            }

            return echoes.MergeAll();
        }

        private static MemoryStream ConvertToMemoryMID(string filePath, ParallelOptions parallelOptions, Action<int, int> progressCallback)
        {
            object l = new object();

            var fStream = new MemoryStream();
            var proj = Project.Load(filePath, false);
            var streams = new ParallelStream(fStream);

            var notesDict = new Dictionary<int, Dictionary<Channel, Note[]>>();
            var patternDict = new Dictionary<int, Dictionary<Channel, TempoEvent[]>>();

            Parallel.For(0, proj.Patterns.Count(), parallelOptions, i =>
            {
                var pat = proj.Patterns[i];

                int id = pat.Id;
                string name = pat.Name;

                var notes = pat.Notes.ToDictionary(c => c.Key, c =>
                {
                    byte channel = 0;
                    var colorChan = false;

                    if (c.Key.Data is GeneratorData data && data.GeneratorName.ToLower() == "midi out")
                    {
                        if (data.PluginSettings[29] == 0x01) colorChan = true;
                        channel = data.PluginSettings[4];
                    }

                    var noteList = new List<Note>(c.Value.Count);

                    var lastNoteZeroTick = -1.0;
                    foreach (var n in c.Value.OrderBy(n => n.Position))
                    {
                        var newNote = new Note((colorChan || ForceColor) ? n.Color : channel, Math.Min((byte)127, n.Key), Math.Min((byte)127, n.Velocity), (double)n.Position, (double)n.Position + (double)n.Length);
                        noteList.Add(newNote);

                        if (lastNoteZeroTick != -1.0 && lastNoteZeroTick != newNote.Start)
                        {
                            lastNoteZeroTick = -1.0;
                            noteList[^2].End = newNote.Start;
                        }

                        if (newNote.Length == 0)
                        {
                            lastNoteZeroTick = newNote.Start;
                            newNote.End = double.PositiveInfinity;
                        }
                    }

                    return noteList.ToArray();
                });

                lock (l)
                {
                    progressCallback(pat.Id, proj.Patterns.Count);
                    notesDict.Add(id, notes);
                }

                Debug.PrintToConsole(Debug.LogType.Message, $"Pattern found: ({id}) {name}");
            });

            var trackID = 0;
            var tracks = proj.Tracks.Where(t => t.Items.Count != 0).ToArray();

            Parallel.For(0, tracks.Length, parallelOptions, i =>
            {
                var stream = new BufferedStream(streams.GetStream(i), 1 << 24);
                var trackWriter = new MidiWriter(stream);

                var track = tracks[i];

                var notes = track.Items.Select(item =>
                {
                    if (item is PatternPlaylistItem && item.Muted == false)
                    {
                        var pi = item as PatternPlaylistItem;
                        var pattern = notesDict[pi.Pattern.Id];
                        var merged = pattern.Select(c =>
                        {
                            var shifted = c.Value
                                          .TrimStart(Math.Max(0, item.StartOffset))
                                          .TrimEnd(Math.Max(0, item.EndOffset == -1 ? item.Length : item.EndOffset))
                                          //.Where(n => n.Length > 0)
                                          .OffsetTime(item.Position - item.StartOffset);

                            var channel = c.Key;
                            switch (channel.Data)
                            {
                                case GeneratorData data:
                                    if (data.EchoFeed > 0 && !DisableEcho)
                                        shifted = EchoNotes(shifted, data.Echo, data.EchoFeed, data.EchoTime, proj.Ppq);
                                    break;
                            }
   
                            return shifted;
                        }).MergeAll();

                        return merged;
                    }

                    return new Note[0];
                }).MergeAll().TrimStart();

                trackWriter.Write(notes.ExtractEvents());
                stream.Close();

                lock (l)
                {
                    progressCallback(trackID, tracks.Length);
                    Debug.PrintToConsole(Debug.LogType.Message, $"Generated track {i + 1}, {(trackID++) + 1}/{tracks.Length}");
                }
            });

            streams.CloseAllStreams();

            var ocStream = new MemoryStream();
            var writer = new MidiWriter(ocStream);
            writer.Init((ushort)proj.Ppq);
            writer.InitTrack();
            writer.Write(new TempoEvent(0, (int)(60000000.0 / proj.Tempo)));
            writer.EndTrack();

            for (int i = 0; i < tracks.Length; i++)
            {
                Debug.PrintToConsole(Debug.LogType.Message, $"Writing track {i + 1}/{tracks.Length} to memory stream...");

                var stream = streams.GetStream(i, true);

                stream.Position = 0;
                unchecked
                {
                    writer.WriteTrack(stream);
                }
                stream.Close();
            }

            writer.Close(false);
            streams.CloseAllStreams();
            streams.Dispose();

            ocStream.Position = 0;
            return ocStream;
        }

        public static MIDI? Load(long id, string filePath, string name, ParallelOptions parallelOptions, Action<int, int> progressCallback)
        {
            try
            {
                MidiFile midi = new MidiFile(ConvertToMemoryMID(filePath, parallelOptions, progressCallback));

                double maxTicks = 0;
                long noteCount = 0;

                var midTracks = midi.IterateTracks();
                var midiMetaEvents = GetMetaEvents(midTracks, parallelOptions, ref maxTicks, ref noteCount, out ulong[] eventCounts, progressCallback);
                var mergedMetaEvents = midiMetaEvents.MergeAll().ToArray();

                // get midi length in seconds
                var mergedWithLength = mergedMetaEvents.MergeWith(new[] { new EndOfExclusiveEvent(maxTicks) });
                double seconds = 0.0;
                foreach (var e in mergedWithLength.MakeTimeBased(midi.PPQ))
                {
                    seconds += e.DeltaTime;
                }

                return new FLP(midi, mergedMetaEvents, name, id, filePath, TimeSpan.FromSeconds(seconds), midi.TrackCount, noteCount, (ulong)(new FileInfo(filePath).Length), eventCounts);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.PrintToConsole(Debug.LogType.Error, e.ToString());
            }

            return null;
        }

        public static FilePickerFileType FlpAll { get; } = new("FLPs")
        {
            Patterns = new[] { "*.flp" },
            AppleUniformTypeIdentifiers = new[] { "flp" },
            MimeTypes = new[] { "audio/flp" }
        };
    }
}
