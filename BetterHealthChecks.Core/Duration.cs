using System;
using System.Diagnostics;

namespace BetterHealthChecks.Core
{
    public class Duration : IDisposable
    {
        private Stopwatch _stopwatch;

        public Duration()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public TimeSpan GetDuration()
        {
            _stopwatch.Stop();
            return _stopwatch.Elapsed;
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _stopwatch = null;
        }
    }
}