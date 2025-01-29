using System.Collections.Generic;

// Abstract class for Movement
public class a_Movement : Attribute
{
    public float Speed { get; private set; }
    public List<float> SpeedMultipliers { get; private set; } = new();

    public void Initialize(float speed)
    {
        Speed = speed;
    }

    public override void Upgrade(int upgrade)
    {
        Speed += upgrade;
    }

    public float GetSpeed()
    {
        float multiplier = 1;
        foreach (var speedMultiplier in SpeedMultipliers)
        {
            multiplier *= speedMultiplier;
        }
        return Speed * multiplier;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        SpeedMultipliers.Add(multiplier);
    }

    public List<float> GetSpeedMultipliers()
    {
        return SpeedMultipliers;
    }
}


