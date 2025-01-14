
// Abstract class for Attack
public abstract class Attack : ChampionAbility
{
    public float Cooldown { get; protected set; }
    public float Distance { get; protected set; }
    public float Damage { get; protected set; }

    protected Attack(float cooldown, float distance, float damage)
    {
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