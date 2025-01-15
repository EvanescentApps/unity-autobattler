
public class a_Attack : Attack
{
    public a_Attack(float cooldown, float distance, float damage) : base(cooldown, distance, damage)
    {
    }

    public void Initialize(float cooldown, float range, float damage)
    {
        Cooldown = cooldown;
        Distance = range;
        Damage = damage;
    }
}
