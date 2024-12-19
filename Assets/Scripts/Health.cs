
// Abstract class for Health
public abstract class Health
{
    public ChampionAbility ChampionAbility { get; private set; }
    public float CurrentHealth { get; private set; }

    protected Health(ChampionAbility championAbility, float initialHealth)
    {
        ChampionAbility = championAbility;
        CurrentHealth = initialHealth;
    }

    public abstract void Upgrade();

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public void SetHealth(float health)
    {
        CurrentHealth = health;
    }
}