
namespace Utils
{
    using System;

    public class DeltaTimer
    {
        private double _minTime;
        private double _maxTime;
        private double _currentTime;
        private readonly Random _random = new();
        private bool isRunning = true;

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
            Reset();
        }

        public bool Delta(double removeTime)
        {
            if(!isRunning) return false;
            _currentTime -= removeTime;
            if (_currentTime <= 0)
            {
                Reset();
                return true;
            }
            return false;
        }

        public void SetResetTime(double time, bool forceImmediateReset = false)
        {
            SetResetRange(time, time, forceImmediateReset);
        }

        public void SetResetRange(double minTime, double maxTime, bool forceImmediateReset = false)
        {
            _minTime = minTime;
            _maxTime = maxTime;
            if (forceImmediateReset)
            {
                Reset();
            }
        }

        public void Reset()
        {
            if (Math.Abs(_minTime - _maxTime) < 0.0001) 
            {
                _currentTime = _minTime; 
            }
            else
            {
                _currentTime = _random.NextDouble() * (_maxTime - _minTime) + _minTime;
            }
        }

        public void Start()
        {
            if (!isRunning) isRunning = true;
        }

        public void Stop()
        {
            if (isRunning) isRunning = false;
        }

        public void ForceFinish()
        {
            _currentTime = 0;
        }
    }
}
