using UnityEngine;

// Abstract class representing a common Entity
public abstract class Entity : MonoBehaviour
{
    public string Name { get; protected set; }
    public int Price { get; protected set; }

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
