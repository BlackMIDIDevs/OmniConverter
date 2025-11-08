using System;
using System.Collections.Generic;
using CSCore;

/*
 * Limiter ported from OmniMidiV2
 *  ---> OmniMIDIv2/OmniMIDI/src/audio/Limiter.cpp
 */

namespace OmniConverter
{
    public class Compressor
    {
        private readonly float _threshold;
        private readonly float _ratio;
        private readonly float _attackCoeff;
        private readonly float _releaseCoeff;

        private readonly float[] _delayBuffer;
        private int _writeIndex;
        private int _readIndex;
        private readonly int _bufferSize;

        private float _envelope;
        private float _gain;

        private static float CalculateCoeff(float timeMs, float sampleRate)
        {
            if (timeMs <= 0.0f) return 1.0f;
            return (float)Math.Exp(-1.0f / (timeMs * 0.001f * sampleRate));
        }

        internal Compressor(float sampleRate, float threshold,
                          float ratio, float attackMs,
                          float releaseMs, float lookaheadMs)
        {
            if (sampleRate <= 0)
                throw new ArgumentException("Invalid sample rate");

            _threshold = threshold;
            _ratio = ratio;

            _attackCoeff = CalculateCoeff(attackMs, sampleRate);
            _releaseCoeff = CalculateCoeff(releaseMs, sampleRate);

            _envelope = 0.0f;
            _gain = 1.0f;

            _bufferSize = (int)(lookaheadMs * 0.001f * sampleRate);
            if (_bufferSize < 1) _bufferSize = 1;

            _delayBuffer = new float[_bufferSize];
            _writeIndex = 0;
            _readIndex = 1 % _bufferSize;
        }

        public float Process(float input)
        {
            // Lookahead delay line
            _delayBuffer[_writeIndex] = input;
            float delayedInput = _delayBuffer[_readIndex];
            float lookaheadSample = input;

            // Envelope detection
            float rectified = Math.Abs(lookaheadSample);
            if (rectified > _envelope)
            {
                _envelope = _attackCoeff * _envelope + (1.0f - _attackCoeff) * rectified;
            }
            else
            {
                _envelope = _releaseCoeff * _envelope + (1.0f - _releaseCoeff) * rectified;
            }

            // Gain computation
            float targetGain = 1.0f;
            if (_envelope > _threshold)
            {
                targetGain = (_threshold + (_envelope - _threshold) / _ratio) / _envelope;
            }

            // Gain application
            if (targetGain < _gain)
            {
                _gain = targetGain; // Instant attack
            }
            else
            {
                // Smooth release
                _gain = _releaseCoeff * _gain + (1.0f - _releaseCoeff) * targetGain;
            }

            float output = delayedInput * _gain;

            // Update buffer indices
            _writeIndex = (_writeIndex + 1) % _bufferSize;
            _readIndex = (_readIndex + 1) % _bufferSize;

            return output;
        }
    }

    public class Limiter : ISampleSource
    {
        private readonly ISampleSource _provider;
        private readonly List<Compressor> _compressors = new();
        private readonly int _numChannels;

        private float _threshold;
        private const float Ratio = 1000.0f;
        private const float AttackMs = 10.0f;
        private const float ReleaseMs = 50.0f;
        private const float LookaheadMs = 10.0f;

        public Limiter(ISampleSource provider, float threshold)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            WaveFormat = provider.WaveFormat;
            _numChannels = WaveFormat.Channels;
            _threshold = threshold;

            for (int i = 0; i < _numChannels; i++)
            {
                _compressors.Add(new
      (
                    sampleRate: WaveFormat.SampleRate,
                    threshold: _threshold,
                    ratio: Ratio,
                    attackMs: AttackMs,
                    releaseMs: ReleaseMs,
                    lookaheadMs: LookaheadMs
                ));
            }
        }

        public WaveFormat WaveFormat { get; }
        public bool CanSeek => _provider.CanSeek;
        public long Length => _provider.Length;
        public long Position { get => _provider.Position; set => _provider.Position = value; }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = _provider.Read(buffer, offset, count);
            int end = offset + read;

            for (int i = offset; i < end; i += _numChannels)
            {
                for (byte ch = 0; ch < _numChannels; ch++)
                {
                    buffer[i + ch] = _compressors[ch].Process(buffer[i + ch]);
                }
            }

            return read;
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}
