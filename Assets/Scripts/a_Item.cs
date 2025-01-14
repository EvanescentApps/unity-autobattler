using System.Collections.Generic;

// Abstract class representing a common Item
public abstract class a_Item
{
    public Entity Entity { get; private set; }
    public Dictionary<Attribute, float> Upgrades { get; private set; } = new();

    protected a_Item(Entity entity)
    {
        Entity = entity;
    }

    public void RunUpgrades()
    {
        foreach (var upgrade in Upgrades)
        {
            upgrade.Key.Upgrade((int)upgrade.Value);
        }
    }
}
