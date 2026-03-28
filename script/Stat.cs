using Godot;

public class Stat
{
    private readonly string name;
    private float value;
    private float targetValue;
    private readonly float coolValue;

    private readonly float rate;
    private (float min, float max) range;

    public float Value 
    { 
        get {return this.value;} 
        set 
        {
            targetValue = value;
            if(rate <= float.Epsilon)
            {
                this.value = value;
            }
        } 
    }
    public string Name => name;

    public Stat(string name, float value = 0, float rate = 0, (float min, float max) range = default)
    {
        this.name = name;
        this.value = value;
        this.rate = rate;
        this.targetValue = value;
        this.coolValue = value;
        this.range = range == (0,0) ? (0, 10) : range;
    }

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
        return range.max == 0 ?  0 : Value/range.max;
    }

    public bool IsOver(float percent)
    {
        if(GetNormalised() > percent) return true;
        else return false;
    }

    public void Update(double delta)
    {
        value = Mathf.Lerp(value, targetValue, rate * (float)delta);
    }
     
    public void CoolDown(double delta)
    {
        value = Mathf.Lerp(value, coolValue, rate * (float)delta);
    }
}
