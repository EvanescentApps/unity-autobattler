
// Abstract class for Attack
public class a_Attack : Attribute
{
    public float Cooldown { get; protected set; }
    public float Distance { get; protected set; }
    public float Damage { get; protected set; }

    public void Initialize(float cooldown, float range, float damage)
    {
        Cooldown = cooldown;
        Distance = range;
        Damage = damage;
    }

    public override void Upgrade(int upgrade)
    {
        Damage += upgrade;
    }

    public float GetDamage()
    {
        return Damage;
    }

    public void SetDamage(float damage)
    {
        Damage = damage;
    }
}