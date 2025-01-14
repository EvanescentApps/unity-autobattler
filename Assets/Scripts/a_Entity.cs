using UnityEngine;

public class a_Entity : MonoBehaviour
{
    public string Name { get; private set; }
    public int Price { get; private set; }

    public void Initialize(string name, int price)
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