using System.Diagnostics;
using NAudio.Wave;

namespace Looper
{
    //ZAPETLA MEMORY STREAM
    public delegate void ResetProgressBarDelegate(LoopStream loopStream);
    public class LoopStream(WaveStream sourceStream) : WaveStream
    {

        private long position;
        public double PositionPercent => (double)Position / sourceStream.Length;



        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        public event ResetProgressBarDelegate? OnResetProgressBar;
        public override long Length => long.MaxValue; // Infinite length for looping



        public override long Position
        {
            get => position;
            set => position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = sourceStream.Read(buffer, offset, count);
            if (bytesRead == 0)
            {
                OnResetProgressBar?.Invoke(this);
                sourceStream.Position = 0; // Loop back to the start
                position = 0; // Loop back to the start
                bytesRead = sourceStream.Read(buffer, offset, count);
            }
            position += bytesRead;
            return bytesRead;
        }
    }

}

