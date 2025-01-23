using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Timers;

namespace Looper.Models
{
    class Metronome
    {

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Timer timer;
        private int iterator = 0;
        private int bpm;

        /// <summary>
        /// Beats per minute of metronome
        /// </summary>
        public int Bpm
        {
            get { return bpm; }
            set
            {
                bpm = value;
                if (bpm != 0)
                {
                    timer.Interval = (double)60000 / bpm;
                    if (IsPlaying)
                    {
                        timer.Stop();
                        timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the metronome is currently playing
        /// </summary>
        public bool IsPlaying { get { return timer.Enabled; } }


        private double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
            }
        }


        public Metronome()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// starts metronome
        /// </summary>
        public void Start() { timer.Start(); }

        /// <summary>
        /// stops metronome
        /// </summary>
        public void Stop() { timer.Stop(); }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                System.IO.MemoryStream clickStream;
                AudioPlayer player = new()
                {
                    Volume = volume,
                    MasterVolume = 1
                };

                if (iterator == 4)
                {
                    clickStream = new System.IO.MemoryStream();
                    iterator = 0;
                }
                else
                {
                    clickStream = new System.IO.MemoryStream();//nie wiem jak to zrobić
                }
                player.StartPlaying(clickStream, false);
                iterator++;
            }
            catch (Exception ex)
            { // Log or handle the exception
                logger.Error($"Error: {ex.Message}");
            }
        }
    }
}
