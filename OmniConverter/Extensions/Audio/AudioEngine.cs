using CSCore;
using System;

namespace OmniConverter
{
    public enum EngineID
    {
        Unknown = -1,
        BASS = 0,
        XSynth = 1
    }

    public abstract class AudioEngine : IDisposable
    {
        protected bool _disposed = false;
        public bool Initialized { get; protected set; } = false;
        public WaveFormat WaveFormat { get; protected set; } = new(48000, 32, 2);

        public AudioEngine(WaveFormat waveFormat, bool defaultInit = true) { WaveFormat = waveFormat; Initialized = defaultInit; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
