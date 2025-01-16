using UnityEngine;

public class a_Entity : Entity
{
    public a_Entity(string name, int price) : base(name, price)
    {
    }

    public void Initialize(string name, int price)
    {
        this.Name = name;
        this.Price = price;
    }

}
