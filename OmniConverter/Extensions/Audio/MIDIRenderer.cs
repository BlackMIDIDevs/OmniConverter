using CSCore;
using System;

namespace OmniConverter
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

    public abstract class MIDIRenderer : ISampleSource
    {
        public string UniqueID { get; protected set; } = IDGenerator.GetID();
        public bool CanSeek { get; protected set; } = false;
        public CSCore.WaveFormat WaveFormat { get; protected set; } = new(48000, 32, 2);
        public bool Initialized { get; protected set; } = false;
        public bool Disposed { get; protected set; } = false;
        public ulong ActiveVoices { get; protected set; } = 0;
        public float RenderingTime { get; protected set; } = 0.0f;

        public MIDIRenderer(WaveFormat waveFormat, bool defaultInt = true) { WaveFormat = waveFormat; Initialized = defaultInt; }

        public abstract void SystemReset();
        public abstract void ChangeVolume(float volume);
        public abstract bool SendCustomFXEvents(int channel, short reverb, short chorus);
        public abstract void SendEvent(byte[] data);
        public abstract unsafe int Read(float[] buffer, int offset, long delta, int count);
        public abstract bool SendEndEvent();
        public abstract void RefreshInfo();
        public abstract void SetRenderingTime(float rt);
        public abstract long Position { get; set; }
        public abstract long Length { get; }

        public int Read(float[] buffer, int offset, int count) => Read(buffer, offset, 0, count);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected abstract void Dispose(bool disposing);
    }
}
