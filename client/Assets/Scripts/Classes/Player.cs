using System.Collections.Generic;
using Thubg.Messages;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player
{
    public int Id;
    public string Name;
    public int Health;
    public ArmorTypes Armor;
    public float Speed;
    public FirearmTypes Firearm;
    public Dictionary<string, int> Inventory;
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

    public void Attack(Position targetPosition)
    {
        FaceTo(targetPosition);
        playerAnimations.SetFiring();
        
    }

    public void UseMedicine()
    {
        playerAnimations.SetDrinking();
    }

    public void FaceTo(Position pos)
    {
        if (pos.x == playerObj.transform.position.x && pos.y == playerObj.transform.position.z)
        {
            return;
        }
        Vector3 direction = new Vector3(pos.x, playerObj.transform.position.y, pos.y) - playerObj.transform.position;
        playerObj.transform.forward = direction;
    }

    public void UpdatePosition(Position pos)
    {
        PlayerPosition = pos;
        // Compute Delta
        Vector3 newPos = new Vector3(pos.x, playerObj.transform.position.y, pos.y);
        FaceTo(pos);
        TryGetPlayerAnimations();
        playerAnimations?.WalkTo(playerObj.transform.position, newPos);
        playerObj.transform.position = newPos;
    }
}
