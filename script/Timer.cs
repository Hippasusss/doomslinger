
namespace Utils
{
    using System;

    public class DeltaTimer
    {
        private double _minTime;
        private double _maxTime;
        private double _currentTime;
        private readonly Random _random = new();

        public DeltaTimer(double setTime)
        {
            _minTime = setTime;
            _maxTime = setTime;
            _currentTime = setTime;
        }

        public DeltaTimer(double minTime, double maxTime)
        {
            _minTime = minTime;
            _maxTime = maxTime;
            _currentTime = GetNextRandomTime();
        }

        public bool Delta(double removeTime)
        {
            _currentTime -= removeTime;
            if (_currentTime <= 0)
            {
                _currentTime = GetNextRandomTime();
                return true;
            }
            return false;
        }


        public void SetResetTime(double time, bool resetCurrent = false)
        {
            SetResetRange(time, time, resetCurrent);
        }

        public void SetResetRange(double minTime, double maxTime, bool resetCurrent = false)
        {
            _minTime = minTime;
            _maxTime = maxTime;
            if (resetCurrent)
            {
                _currentTime = GetNextRandomTime();
            }
        }

        public void ForceFinish()
        {
            _currentTime = 0;
        }

        private double GetNextRandomTime()
        {
            if (Math.Abs(_minTime - _maxTime) < 0.0001) return _minTime;
            return _random.NextDouble() * (_maxTime - _minTime) + _minTime;
        }
    }
}
