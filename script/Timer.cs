using System;

namespace DoomSlinger;

public class DeltaTimer
{
    private double _minTime;
    private double _maxTime;
    private double _currentTime;
    private double _resetValue;
    private readonly Random _random = new();
    private bool isRunning = true;

    public float Progress => _resetValue <= 0 ? 0f : Math.Clamp(1f - (float)(_currentTime / _resetValue), 0f, 1f);
    public bool AutoRestart = true;

    public DeltaTimer(double setTime)
    {
        _minTime = setTime;
        _maxTime = setTime;
        _currentTime = setTime;
        _resetValue = setTime;
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
            _currentTime = 0;
            if (AutoRestart) Reset();
            else Stop();
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
            Start();
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
        _resetValue = _currentTime;
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
