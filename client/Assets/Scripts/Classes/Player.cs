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
    public GameObject playerObj;

    public void TryGetPlayerAnimations()
    {
        if (playerAnimations == null)
        {
            playerAnimations = playerObj.GetComponent<PlayerAnimations>();
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

    public void UpdatePosition(Position pos)
    {
        PlayerPosition =pos;
        // Compute Delta
        Vector3 newPos = new Vector3(pos.x, playerObj.transform.position.y, pos.y);
        TryGetPlayerAnimations();
        if (playerAnimations is not null)
        {
            playerAnimations.WalkTo(playerObj.transform.position, newPos);
        }
        playerObj.transform.position = new Vector3(pos.x, playerObj.transform.position.y, pos.y);
    }
}
