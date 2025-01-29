using System.Collections.Generic;
using UnityEngine;

public abstract class a_Item
{
    public Entity Entity { get; private set; }
    public List<DraggableItem.ItemUpgrade> Upgrades { get; protected set; } = new();
    private bool hasBeenApplied = false;

    protected a_Item(Entity entity, List<DraggableItem.ItemUpgrade> upgrades)
    {
        Entity = entity;
        Upgrades = upgrades;
    }

    public void RunUpgrades(a_Champion champion)
    {
        if (hasBeenApplied) return;

        foreach (var upgrade in Upgrades)
        {
            Attribute attribute = null;
            switch (upgrade.attributeName.ToLower())
            {
                case "health":
                    attribute = champion.Health;
                    break;
                case "attack":
                    attribute = champion.Attack;
                    break;
                case "movement":
                    attribute = champion.Movement;
                    break;
                default:
                    Debug.LogWarning($"Attribute {upgrade.attributeName} not found on champion");
                    continue;
            }

            if (attribute != null)
            {
                attribute.Upgrade(upgrade.upgradeLevel);
                Debug.Log($"Upgraded {upgrade.attributeName} by {upgrade.upgradeLevel} levels");
            }
        }
        hasBeenApplied = true;
    }

}
