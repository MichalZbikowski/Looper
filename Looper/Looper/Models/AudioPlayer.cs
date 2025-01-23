using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using NLog;
using Looper.Models.AudioEffects;

namespace Looper
{
    public class AudioPlayer
    {

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// WaveOutEvent object for playing audio
        /// </summary>
        public WaveOutEvent? WaveOut { get; private set; }
        /// <summary>
        /// LoopStream object for looping audio
        /// </summary>
        public LoopStream? LoopStream { get; private set; }
        /// <summary>
        /// Boolean value indicating whether audio is playing
        /// </summary>
        public bool IsPlaying { get; private set; } = false;

        /// <summary>
        /// VolumeSampleProvider object for adjusting volume
        /// </summary>
        private VolumeSampleProvider? volumeSampleProvider;

        private ReverbSampleProvider? reverbSampleProvider = null;

        #region Odtwarzanie

        private static WaveFormatConversionStream ConvertToMono(WaveStream inputStream)
        {
            var outFormat = new WaveFormat(inputStream.WaveFormat.SampleRate, 1);
            return new WaveFormatConversionStream(outFormat, inputStream);
        }

        public void StartPlaying(MemoryStream memoryStream, bool loop)
        {
            if (!IsPlaying)
            {
                try
                {
                    memoryStream.Position = 0; // Reset position for playback
                    var waveReader = new WaveFileReader(memoryStream);

                    ISampleProvider sampleProvider;
                    if (loop)
                    {
                        LoopStream = new LoopStream(waveReader); // Use LoopStream for continuous looping
                        sampleProvider = LoopStream.ToSampleProvider();
                    }
                    else
                    {
                        sampleProvider = ConvertToMono(waveReader).ToSampleProvider();

                    }

                    // Add effects
                    int balance = 0;
                    sampleProvider = new PanningSampleProvider(sampleProvider) { Pan = balance };
                    volumeSampleProvider = new VolumeSampleProvider(sampleProvider) { Volume = (float)Volume };

                    logger.Info("Started playback");
                    WaveOut = new WaveOutEvent();
                    WaveOut.Init(volumeSampleProvider);
                    WaveOut.Play();
                    IsPlaying = true;

                    UpdateFinalVolume();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error starting playback");
                }
            }
            else
            {
                logger.Info("Already playing");
            }
        }

        public void StopPlaying()
        {
            if (IsPlaying)
            {
                logger.Info("Stopped loop");
                WaveOut.Stop();
                WaveOut.Dispose();
                WaveOut = null;
                IsPlaying = false;
            }
            else
            {
                logger.Info("Not playing");
            }
        }

        #endregion

        #region Delay



        private double delay;
        public double Delay
        {
            get { return delay; }
            set
            {
                delay = value;
            }
        }

        #endregion

        #region Glosość
        private double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                logger.Info($"AudioPlayer Volume set to: {volume}");
                UpdateFinalVolume();
            }
        }


        private double masterVolume;
        public double MasterVolume
        {
            get { return masterVolume; }
            set
            {
                masterVolume = value;
                LogManager.GetCurrentClassLogger().Info($"AudioPlayer masterVolume set to: {MasterVolume}");
                UpdateFinalVolume();
            }
        }

        private void UpdateFinalVolume()
        {
            double finalVolume = volume * masterVolume;
            logger.Info($"AudioPlayer FinalVolume set to: {finalVolume}");
            if (volumeSampleProvider != null)
            {
                volumeSampleProvider.Volume = (float)finalVolume;
            }
        }

        #endregion

        #region Reverb
        private double reverbLevel = 0;
        public double ReverbLevel
        {
            get { return reverbLevel; }
            set
            {
                reverbLevel = value;
                UpdateReverb();
            }
        }

        private double reverbTime;
        public double ReverbTime
        {
            get { return reverbTime; }
            set
            {
                reverbTime = value;
                UpdateReverb();
            }
        }

        private double preDelay;
        public double PreDelay
        {
            get { return preDelay; }
            set
            {
                preDelay = value;
                UpdateReverb();
            }
        }

        private double roomSize;
        public double RoomSize
        {
            get { return roomSize; }
            set
            {
                roomSize = value;
                UpdateReverb();
            }
        }

        private double damping;
        public double Damping
        {
            get { return damping; }
            set
            {
                damping = value;
                UpdateReverb();
            }
        }

        private double wetDryMix;
        public double WetDryMix
        {
            get { return wetDryMix; }
            set
            {
                wetDryMix = value;
                UpdateReverb();
            }
        }

        private double diffusion;
        public double Diffusion
        {
            get { return diffusion; }
            set
            {
                diffusion = value;
                UpdateReverb();
            }
        }

        private double earlyReflections;
        public double EarlyReflections
        {
            get { return earlyReflections; }
            set
            {
                earlyReflections = value;
                UpdateReverb();
            }
        }

        private void UpdateReverb()
        {
            reverbSampleProvider.ReverbLevel = (float)reverbLevel;
            reverbSampleProvider.ReverbTime = (float)reverbTime;
            reverbSampleProvider.PreDelay = (float)preDelay;
            reverbSampleProvider.RoomSize = (float)roomSize;
            reverbSampleProvider.Damping = (float)damping;
            reverbSampleProvider.WetDryMix = (float)wetDryMix;
            reverbSampleProvider.Diffusion = (float)diffusion;
            reverbSampleProvider.EarlyReflections = (float)earlyReflections;
        }

        #endregion

    }
}
