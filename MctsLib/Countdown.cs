using System.Diagnostics;

namespace lib
{
    public class Countdown
    {
        private readonly Stopwatch stopwatch;
        private readonly int timeAvailableMs;

        public Countdown(int ms)
        {
            stopwatch = Stopwatch.StartNew();
            timeAvailableMs = ms;
        }

        public bool IsFinished => TimeLeftMs <= 0;

        public long TimeLeftMs => timeAvailableMs - ElapsedMs;
        public long ElapsedMs => stopwatch.ElapsedMilliseconds;

        public override string ToString()
        {
            return $"Elapsed {ElapsedMs} ms. TimeLeft {TimeLeftMs}. IsFinished {IsFinished} Available {timeAvailableMs} ms";
        }

        public static implicit operator Countdown(int milliseconds)
        {
            return new Countdown(milliseconds);
        }
    }
}