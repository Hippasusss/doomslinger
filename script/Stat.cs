using Godot;

namespace DoomSlinger;

public class Stat(string newName, float newValue = 0, float newRate = 0, (float min, float max) newRange = default, Stat newTargetStat = null)
{
    private readonly string name = newName;
    private float currentValue = newValue;
    private float targetValue = newValue;
    private Stat targetStat = newTargetStat;
    private readonly float coolValue = newValue;
    private readonly float rate = newRate;
    private (float min, float max) range = newRange == (0, 0) ? (0, 10) : newRange;

    public float Value 
    { 
        get {return currentValue;} 
        set 
        {
            targetValue = value;
            if(rate <= float.Epsilon)
            {
                currentValue = value;
            }
        } 
    }

    public string Name => name;

    public static Stat operator + (Stat a, float b)
    {
        a.Value = Mathf.Clamp(a.targetValue + b, a.range.min, a.range.max);
        return a;
    }

    public static Stat operator + (Stat a, Stat b)
    {
        return a + b.targetValue;
    }

    public static implicit operator float(Stat b)
    {
        return b.targetValue;
    }

    public void SetChaseStat(Stat other)
    {
        targetStat = other;
    }

    public void Randomize(float min = 0, float max = 0)
    {
        if (min == 0 && max == 0)
        {
            Value = (float)GD.RandRange(range.min, range.max);
        }
        else
        {
            range.min = min;
            range.max = max;
            Value = (float)GD.RandRange(min,max);
        }
    }

    public float GetNormalised()
    {
        return range.max == 0 ?  0 : Value / (range.max - range.min);
    }

    public bool IsOver(float percent)
    {
        if(GetNormalised() > percent) return true;
        else return false;
    }

    public void Update(double delta)
    {
        float setValue = targetStat == null ? targetValue : targetStat.Value;
        currentValue = Mathf.Lerp(currentValue, setValue , rate * (float)delta);
    }
     
    public void CoolDown(double delta)
    {
        currentValue = Mathf.Lerp(currentValue, coolValue, rate * (float)delta);
    }
}
