
// Abstract class for Attack
public abstract class Attack
{
    public ChampionAbility ChampionAbility { get; private set; }
    public float Cooldown { get; protected set; }
    public float Distance { get; protected set; }
    public float Damage { get; protected set; }

    protected Attack(ChampionAbility championAbility, float cooldown, float distance, float damage)
    {
        ChampionAbility = championAbility;
        Cooldown = cooldown;
        Distance = distance;
        Damage = damage;
    }

    public abstract void Upgrade();

    public float GetDamage()
    {
        return Damage;
    }

    public void SetDamage(float damage)
    {
        Damage = damage;
    }
}