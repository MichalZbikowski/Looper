using System.ComponentModel;
using System.Timers;
using NLog;
using System.Diagnostics;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System.IO;
using System.Windows;
using Avalonia.Logging;
using Avalonia.Threading;
using System.Threading.Tasks;
using Logger = NLog.Logger;

namespace Looper
{
    public class LoopMenu : INotifyPropertyChanged
    {
        private static bool isRecording;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        public AudioRecorder Recorder { get; private set; }
        public AudioPlayer Player { get; private set; }
        public bool IsRecorded => Recorder.IsRecorded; // Sprawdza czy nagranie jest zakończone i czy istnieje MemoryStream
        public static bool IsRecording => isRecording; //Sprawdza czy nagrywanie ktorejkolwiek z petli jest w trakcie
        public bool IsPlaying => Player.IsPlaying;
        public bool CanStartRecording => !IsRecording && !IsRecorded;
        public bool CanStopRecording => IsRecording && Recorder.IsRecording;
        public bool CanStartLooping => IsRecorded && !IsPlaying;
        public bool CanStopLooping => IsRecorded && IsPlaying;
        public bool CanDeleteLoop => IsRecorded && !IsRecording;



        public LoopMenu()
        {
            Recorder = new AudioRecorder();
            Player = new AudioPlayer();
            Debug.WriteLine("LoopMenu created");

        }


        #region Audio
        public void StartRecording()
        {
            Debug.WriteLine("StartRecording");
            if (!IsRecording && !IsRecorded)
            {
                Recorder.StartRecording();
                isRecording = true;
            }
            else
            {
                logger.Info("Recording already in progress or already recorded");
            }
        }

        public void StopRecording()
        {
            if (Recorder.IsRecording)
            {
                Recorder.StopRecording();
                isRecording = false;
                LengthInSeconds = (double)Recorder.MemoryStream.Length / (44100 * 2);
                logger.Info($"LoopMenu StopRecording, audio length: {LengthInSeconds}");
            }
            else
            {
                logger.Info("Recording didn't start");
            }
        }



        public void StartLooping()
        {
            if (IsRecorded && !IsPlaying)
            {
                Player.StartPlaying(Recorder.MemoryStream, true);
                Player.Volume = Volume; // Ustawienie początkowej głośności
            }
            else
            {
                logger.Info("No recording to loop or already playing");
            }
        }


        public void StopLooping()
        {
            if (Player.IsPlaying)
            {
                Player.StopPlaying();
 
            }
            else
            {
                logger.Info("No loop to stop");
            }
        }


        public void DeleteLoop()
        {
            if (IsRecorded && !IsRecording) // Sprawdzenie, czy nagrywanie nie jest w toku
            {
                logger.Info("Deleted loop");

                StopLooping(); //zatrzymanie petli przed usunieciem
                Recorder.MemoryStream.Dispose();
                Recorder.IsRecorded = false; // Po usunięciu nagrania ustawienie IsRecorded na false
            }
            else
            {
                logger.Info("Cannot delete loop while recording is in progress");
            }
        }

        #endregion

        #region ProgressBar
        public double LengthInSeconds { get; set; } //dlugosc nagrania w sekundach


        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }


        private async void UpdateProgress(LoopStream loopStream)
        {
            Progress = 0;
            for (int i = 0; i < 100; i++)
            {
                Progress = i;
                await Task.Delay((int)(LengthInSeconds * 10));
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
                OnPropertyChanged(nameof(Delay));
                if (Player != null)
                {
                    Player.Delay = delay; // Aktualizacja głośności w AudioPlayer
                }
            }
        }
        #endregion

        #region Głosność
        private double _masterVolume;
        public double MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                _masterVolume = value;
                Player.MasterVolume = value; // Aktualizacja MasterVolume w AudioPlayer
            }
        }

        private double volume = 0.5; // Domyślna wartość Volume
        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                OnPropertyChanged(nameof(Volume));
                if (Player != null)
                {
                    Player.Volume = volume; // Aktualizacja głośności w AudioPlayer
                }
            }
        }
        #endregion

        #region Reverb
        private double reverbLevel;
        public double ReverbLevel
        {
            get { return reverbLevel; }
            set
            {
                reverbLevel = value;
                logger.Info($"LoopMenu reverbLevel set to: {reverbLevel}");
            }
        }

        private double reverbTime;
        public double ReverbTime
        {
            get { return reverbTime; }
            set
            {
                reverbTime = value;
                logger.Info($"LoopMenu reverbTime set to: {reverbTime}");
            }
        }

        private double preDelay;
        public double PreDelay
        {
            get { return preDelay; }
            set
            {
                preDelay = value;
                logger.Info($"LoopMenu preDelay set to: {preDelay}");
            }
        }

        private double roomSize;
        public double RoomSize
        {
            get { return roomSize; }
            set
            {
                roomSize = value;
                logger.Info($"LoopMenu roomSize set to: {roomSize}");
            }
        }

        private double damping;
        public double Damping
        {
            get { return damping; }
            set
            {
                damping = value;
                logger.Info($"LoopMenu damping set to: {damping}");
            }
        }

        private double wetDryMix;
        public double WetDryMix
        {
            get { return wetDryMix; }
            set
            {
                wetDryMix = value;
                logger.Info($"LoopMenu wetDryMix set to: {wetDryMix}");
            }
        }

        private double diffusion;
        public double Diffusion
        {
            get { return diffusion; }
            set
            {
                diffusion = value;
                logger.Info($"LoopMenu diffusion set to: {diffusion}");
            }
        }

        private double earlyReflections;
        public double EarlyReflections
        {
            get { return earlyReflections; }
            set
            {
                earlyReflections = value;
                logger.Info($"LoopMenu earlyReflections set to: {earlyReflections}");
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
