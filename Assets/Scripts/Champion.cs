
// Abstract class for Champion
public abstract class Champion
{
    public Entity Entity { get; private set; }
    public Health Health { get; private set; }
    public List<Item> Items { get; private set; } = new();
    public Attack Attack { get; private set; }
    public Movement Movement { get; private set; }

    protected Champion(Entity entity, Health health, Attack attack, Movement movement)
    {
        Entity = entity;
        Health = health;
        Attack = attack;
        Movement = movement;
    }
}
