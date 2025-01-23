using Microsoft.VisualBasic;
using NAudio.Utils;
using NAudio.Wave;
using NLog;
using System.Diagnostics;
using System.IO;

namespace Looper
{
    public class AudioRecorder
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// MemoryStream containing recorded audio
        /// </summary>
        public MemoryStream? MemoryStream { get; set; }
        /// <summary>
        /// WaveInEvent object for recording audio
        /// </summary>
        public WaveInEvent? WaveIn { get; private set; }
        /// <summary>
        ///WaveFileWriter object for writing audio to file
        /// </summary>
        public WaveFileWriter? WaveWriter { get; private set; }
        /// <summary>
        /// Boolean value indicating whether recording is in progress
        /// </summary>
        public bool IsRecording { get; private set; } = false; //Sprawdza czy nagrywanie  jest w trakcie
        /// <summary>
        /// Boolean value indicating whether audio is recorded
        /// </summary>
        public bool IsRecorded { get; internal set; } = false; // Sprawdza czy nagranie jest zakończone


        /// <summary>
        /// starts recording audio
        /// </summary>
        public void StartRecording()
        {
            if (!IsRecording)
            {
                logger.Info("Recording started");

                MemoryStream = new MemoryStream();
                WaveIn = new WaveInEvent { WaveFormat = new WaveFormat(44100, 1) };
                WaveIn.DataAvailable += OnDataAvailable;
                WaveWriter = new WaveFileWriter(new IgnoreDisposeStream(MemoryStream), WaveIn.WaveFormat);
                WaveIn.StartRecording();
                IsRecording = true;
                IsRecorded = false; // Resetowanie IsRecorded przy rozpoczęciu nagrywania
            }
            else
            {
                logger.Info("Recording already in progress");
            }
        }

        /// <summary>
        /// stops recording audio
        /// </summary>
        public void StopRecording()
        {
            if (IsRecording)
            {
                logger.Info("Recording ended");
                IsRecording = false;
                WaveIn.StopRecording();
                WaveWriter.Dispose();
                MemoryStream.Position = 0; // Reset position for playback
                IsRecorded = true; // Ustawienie IsRecorded po zakończeniu nagrywania
            }
            else
            {
                logger.Info("Recording didn't start");
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            WaveWriter.Write(e.Buffer, 0, e.BytesRecorded);
        }
    }

}
