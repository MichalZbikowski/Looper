using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Looper.Models.AudioEffects
{
    public class ReverbSampleProvider(ISampleProvider source) : ISampleProvider
    {
        private readonly float[] buffer = new float[source.WaveFormat.SampleRate * 1];
        private int bufferIndex;
        public float ReverbLevel { get; set; } = 1f;
        public float ReverbTime { get; set; } = 3.0f;
        public float PreDelay { get; set; } = 0.1f;
        public float RoomSize { get; set; } = 1.0f;
        public float Damping { get; set; } = 0.2f;
        public float WetDryMix { get; set; } = 1f;
        public float Diffusion { get; set; } = 0.5f;
        public float EarlyReflections { get; set; } = 0.5f;
        public WaveFormat WaveFormat => source.WaveFormat;
        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            for (int n = 0; n < samplesRead; n++)
            {
                float drySample = buffer[offset + n];
                float wetSample = this.buffer[bufferIndex];
                // Apply reverb settings
                float reverbSample = wetSample * ReverbLevel;
                reverbSample *= (float)Math.Exp(-bufferIndex / (ReverbTime * source.WaveFormat.SampleRate));
                reverbSample *= (1.0f - Damping);
                // Mix wet and dry samples
                buffer[offset + n] = drySample * (1.0f - WetDryMix) + reverbSample * WetDryMix;
                // Store the sample in the buffer
                this.buffer[bufferIndex] = drySample + reverbSample * EarlyReflections;
                bufferIndex++;
                if (bufferIndex >= this.buffer.Length)
                {
                    bufferIndex = 0;
                }
            }
            return samplesRead;
        }
    }
    
}
