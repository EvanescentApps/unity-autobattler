using UnityEngine;

// Abstract class representing a common Entity
public abstract class Entity
{
    public string Name { get; private set; }
    public int Price { get; private set; }

    protected Entity(string name, int price)
    {
        Name = name;
        Price = price;
    }

    public string GetName()
    {
        return Name;
    }

    public int GetPrice()
    {
        return Price;
    }
}
