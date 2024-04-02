using System.Collections.Generic;
using Thubg.Sdk;

public class Player
{
    public string Id;
    public int Health;
    public ArmorTypes Armor;
    public float Speed;
    public FirearmTypes Firearm;
    public Dictionary<Items, int> Inventory;
    public Position PlayerPosition;

    public Player(string id, int health, ArmorTypes armor, float speed, FirearmTypes firearm, Position position, Dictionary<Items, int> inventory)
    {
        Id = id;
        Health = health;
        Armor = armor;
        Speed = speed;
        Firearm = firearm;
        PlayerPosition = position;
        Inventory = inventory;
    }
}
