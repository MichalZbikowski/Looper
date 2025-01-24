using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Timers;
using System.Diagnostics;
using NAudio.Wave;
using HarfBuzzSharp;
using Tmds.DBus.Protocol;
using System.IO;
using Avalonia.Platform;
using System.Reflection;



namespace Looper
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

        public MemoryStream ClickStream { get; set; }
        public string filePath;

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

                MemoryStream clickStream;
                AudioPlayer player = new()
                {
                    Volume = volume,
                    MasterVolume = 1
                };

                if (iterator == 4)
                {
                    string relativeFilePath = @"..\..\..\Assets\AudioFiles\Metronome\clickAccent.wav";
                    clickStream = new MemoryStream(ReadFileToByteArray(relativeFilePath));
                    iterator = 0;
                }
                else
                {
                    string relativeFilePath = @"..\..\..\Assets\AudioFiles\Metronome\click.wav";
                    clickStream = new MemoryStream(ReadFileToByteArray(relativeFilePath));
                }
                player.StartPlaying(clickStream, false);
                iterator++;
           

        }


        public byte[] ReadFileToByteArray(string relativeFilePath)
        {
            try
            {
                // Get the base directory where the executable is running
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Combine the base directory with the relative file path
                string fullFilePath = Path.Combine(baseDirectory, relativeFilePath);

                // Ensure the file exists
                if (!File.Exists(fullFilePath))
                {
                    Debug.WriteLine($"File not found: {fullFilePath}");
                    return null;
                }

                // Read the file into a byte array
                byte[] fileBytes = File.ReadAllBytes(fullFilePath);
                Console.WriteLine("File successfully read into byte array.");
                return fileBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }

}

