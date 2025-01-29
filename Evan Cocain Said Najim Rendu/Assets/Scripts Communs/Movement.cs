using System.Collections.Generic;

// Abstract class for Movement
public abstract class Movement
{
    public Attribute ChampionAbility { get; private set; }
    public float Speed { get; private set; }
    public List<float> SpeedMultipliers { get; private set; } = new();

    protected Movement(Attribute championAbility, float speed)
    {
        ChampionAbility = championAbility;
        Speed = speed;
    }

    public abstract void Upgrade();

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


