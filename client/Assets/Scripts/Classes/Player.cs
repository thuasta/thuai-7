using System.Collections.Generic;
using Thubg.Messages;
using UnityEngine;

public class Player
{
    public int Id;
    public string Name;
    public int Health;
    public ArmorTypes Armor;
    public float Speed;
    public FirearmTypes Firearm;
    public Dictionary<Items, int> Inventory;
    public Position PlayerPosition;
    public PlayerAnimations playerAnimations;

    public void TryGetPlayerAnimations()
    {
        if (playerAnimations == null)
        {
            playerAnimations = GameObject.Find(Name).GetComponent<PlayerAnimations>();
        }
    }

    public void Attack()
    {
        playerAnimations.SetFiring();
    }

    public void UseMedicine()
    {
        playerAnimations.SetDrinking();
    }
}
