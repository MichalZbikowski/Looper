using Avalonia.Logging;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using Logger = NLog.Logger;
using NLog;
using Looper.Models;

namespace Looper.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";
        #region Komendy
        public ObservableCollection<LoopMenu> LoopMenus { get; set; }
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand ToggleLoopCommand { get; }
        public ICommand StartLoopingCommand { get; }
        public ICommand StopLoopingCommand { get; }
        public ICommand DeleteLoopCommand { get; }
        public ICommand StartMetronomeCommand { get; }
        public ICommand StopMetronomeCommand { get; }

        readonly Metronome metronome = new();

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public MainWindowViewModel()
        {
            LoopMenus = [];
            InitializeLoopMenus();
            StartRecordingCommand = new RelayCommand<object>(StartRecording, CanStartRecording);
            StopRecordingCommand = new RelayCommand<object>(StopRecording, CanStopRecording);
            ToggleLoopCommand = new RelayCommand<object>(ToggleLoop);
            StartLoopingCommand = new RelayCommand<object>(StartLooping, CanStartLooping);
            StopLoopingCommand = new RelayCommand<object>(StopLooping, CanStopLooping);
            DeleteLoopCommand = new RelayCommand<object>(DeleteLoop, CanDeleteLoop);
            StartMetronomeCommand = new RelayCommand<object>(StartMetronome, CanStartMetronome);
            StopMetronomeCommand = new RelayCommand<object>(StopMetronome, CanStopMetronome);


            MasterVolume = 0.5;
            Overdubs = [false, false, false];
            OverDubLoopNumbers = [0, 0, 0];
            Volumes =
            [
                new NumericItem { Value = 0.5 },
                new NumericItem { Value = 0.5 },
                new NumericItem { Value = 0.5 }
            ];
        }


        private void InitializeLoopMenus()
        {
            for (int i = 0; i < 3; i++)
            {
                LoopMenus.Add(new LoopMenu());
            }
        }



        #region ObslugaPetli


        //Metoda ToggleLoop: po kliknieciu odpowiedniego przycisku:
        //  1.jeśli nie ma nagranej petli, to zacznie nagrywać, a po ponownym nacisnieciu zatrzymuje nagrywanie i zaczyna odtwarzan
        //  2.jeśli jest nagrana pętla, to zatrzymuje i odtwarza nagrywanie

        /// <summary>
        /// toggles loop recording (when there isn't any loop recorded) and playing
        /// </summary>

        private void ToggleLoop(object loopIndex)
        {
            if (!LoopMenu.IsRecording)
            {
                StartRecording(loopIndex);

                if (LoopMenus[Convert.ToInt16(loopIndex)].IsPlaying)
                {
                    StopLooping(loopIndex);
                }
                else
                {
                    StartLooping(loopIndex);
                }
            }
            else
            {
                StopRecording(loopIndex);
                StartLooping(loopIndex);
            }
        }

        //nie wiem jak zrobic to na sztywno, ale chce sprawiać pozory zachowanego MVVM

        private ObservableCollection<bool> _overdubs;
        public ObservableCollection<bool> Overdubs
        {
            get { return _overdubs; }
            set
            {
                _overdubs = value;
                OnPropertyChanged(nameof(Overdubs));
            }
        }

        private ObservableCollection<int> _overDubLoopNumbers;
        public ObservableCollection<int> OverDubLoopNumbers
        {
            get { return _overDubLoopNumbers; }
            set
            {
                _overDubLoopNumbers = value;
                OnPropertyChanged(nameof(OverDubLoopNumbers));
            }
        }


        /// <summary>
        /// starts recording loop, autmaticlly stops recording if checkbox Overdub is checked
        /// </summary>



        private async void StartRecording(object loopIndex)
        {
            int index = Convert.ToInt16(loopIndex);
            LoopMenus[index].StartRecording();
            Debug.WriteLine(LoopMenus[index].LengthInSeconds);
            Debug.WriteLine(LoopMenus[OverDubLoopNumbers[index]].LengthInSeconds + "Aaaaaaaaaaaaaaaaaaaaaa");

            //gdy checkbox do ovedubu wlaczony
            if (Overdubs[index])
            {
                await Task.Delay(TimeSpan.FromSeconds(LoopMenus[OverDubLoopNumbers[index - 1]].LengthInSeconds));
                LoopMenus[index].StartRecording();
                LoopMenus[index].StopRecording();
                LoopMenus[index].StartLooping();
            }
        }

        private bool CanStartRecording(object loopIndex)
        {
            return LoopMenus[Convert.ToInt16(loopIndex)].CanStartRecording;
        }


        /// <summary>
        /// stops recording loop
        /// </summary>
        private void StopRecording(object loopIndex)
        {
            LoopMenus[Convert.ToInt16(loopIndex)].StopRecording();
        }

        private bool CanStopRecording(object loopIndex)
        {
            return LoopMenus[Convert.ToInt16(loopIndex)].CanStopRecording;
        }

        /// <summary>
        /// stops recording loop
        /// </summary>
        private void StartLooping(object loopIndex)
        {
            LoopMenus[Convert.ToInt16(loopIndex)].StartLooping();
        }

        private bool CanStartLooping(object loopIndex)
        {
            return LoopMenus[Convert.ToInt16(loopIndex)].CanStartLooping;
        }

        private void StopLooping(object loopIndex)
        {
            LoopMenus[Convert.ToInt16(loopIndex)].StopLooping();
        }

        private bool CanStopLooping(object loopIndex)
        {
            return LoopMenus[Convert.ToInt16(loopIndex)].CanStopLooping;
        }

        private void DeleteLoop(object loopIndex)
        {
            LoopMenus[Convert.ToInt16(loopIndex)].DeleteLoop();
        }

        private bool CanDeleteLoop(object loopIndex)
        {
            return LoopMenus[Convert.ToInt16(loopIndex)].CanDeleteLoop;
        }

        #endregion

        #region Metronom

        private int metronomeBpm;
        public int MetronomeBpm
        {
            get { return metronomeBpm; }
            set
            {
                metronomeBpm = value;
                metronome.Bpm = metronomeBpm;
                logger.Info("MetronomeBpm set to: " + metronomeBpm);
                OnPropertyChanged(nameof(MetronomeBpm));
            }
        }



        private double metronomeVolume;
        public double MetronomeVolume
        {
            get { return metronomeVolume; }
            set
            {
                metronomeVolume = value;
                metronome.Volume = metronomeVolume;
                OnPropertyChanged(nameof(MetronomeVolume));
            }
        }


        private void StartMetronome(object liczba)
        {
            metronome.Start();
        }

        private bool CanStartMetronome(object liczba)
        {
            return !metronome.IsPlaying && metronome.Bpm != 0;
        }
        private void StopMetronome(object liczba)
        {
            metronome.Stop();
        }
        private bool CanStopMetronome(object liczba)
        {
            return metronome.IsPlaying;
        }

        #endregion

        #region Glosość
        //------------------------------------GŁOŚNOŚĆ------------------------------------

        //lista głośności dla każdej pętli, binding w xaml poprzez index

        private ObservableCollection<NumericItem> _volumes;
        public ObservableCollection<NumericItem> Volumes
        {
            get { return _volumes; }
            set
            {
                if (_volumes != null)
                {
                    foreach (var volume in _volumes)
                    {
                        volume.PropertyChanged -= Volume_PropertyChanged;
                    }
                }

                _volumes = value;

                if (_volumes != null)
                {
                    foreach (var volume in _volumes)
                    {
                        volume.PropertyChanged += Volume_PropertyChanged;
                    }
                }

                OnPropertyChanged(nameof(Volumes));
            }
        }

        private void Volume_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NumericItem.Value))
            {
                var volumeItem = sender as NumericItem;
                if (volumeItem == null)
                {
                    return;
                }
                int index = Volumes.IndexOf(volumeItem);
                LoopMenus[index].Volume = volumeItem.Value;
            }
        }



        //glosnosc głowna

        private double masterVolume;
        public double MasterVolume
        {
            get { return masterVolume; }
            set
            {
                foreach (var loopMenu in LoopMenus)
                {
                    loopMenu.MasterVolume = value;
                }
                masterVolume = value;
                OnPropertyChanged(nameof(MasterVolume));
            }
        }
        #endregion

        #region Delay

        private ObservableCollection<NumericItem> _delays;
        public ObservableCollection<NumericItem> Delays
        {
            get { return _delays; }
            set
            {
                if (_delays != null)
                {
                    foreach (var delay in _delays)
                    {
                        delay.PropertyChanged -= Delay_PropertyChanged;
                    }
                }

                _delays = value;

                if (_delays != null)
                {
                    foreach (var delay in _delays)
                    {
                        delay.PropertyChanged += Delay_PropertyChanged;
                    }
                }

                OnPropertyChanged(nameof(Delays));
            }
        }

        private void Delay_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NumericItem.Value))
            {
                var delayItem = sender as NumericItem;
                if (delayItem == null)
                {
                    return;
                }
                int index = Delays.IndexOf(delayItem);
                LoopMenus[index].Delay = delayItem.Value;
            }
        }

        #endregion
        #region ProgressBar
        //------------------------------------PROGRES------------------------------------

        // NIE DZIALA, ZAWSZE MOGE ODWOLAC SIE OD RAZU DO loopMenu[INDEX].Progress W XAML

        private ObservableCollection<double> _progresses;
        public ObservableCollection<double> Progresses
        {
            get { return _progresses; }
            set
            {
                _progresses = value;
                foreach (var loopMenu in LoopMenus)
                {
                    _progresses[LoopMenus.IndexOf(loopMenu)] = loopMenu.Progress;
                    Debug.WriteLine(_progresses.ToString());
                }
                OnPropertyChanged(nameof(Progresses));
            }
        }


        #endregion

        #region INotifyPropertyChanged
        //------------------------------------INotifyPropertyChanged------------------------------------

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}


