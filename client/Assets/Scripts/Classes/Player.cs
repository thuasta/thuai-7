using System.Collections.Generic;

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
    public BeamAnimations beamAnimations;
    public GameObject playerObj;
    public GameObject beam;
    public GameObject playertitle;
    public Color lineColor;

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
        ShowGunFire(targetPosition);
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
        playerObj.transform.forward = Quaternion.AngleAxis(30, Vector3.up) * direction;
    }

    public void ShowGunFire(Position pos)
    {
        Vector3 endPoint = new Vector3(pos.x, playerObj.transform.position.y, pos.y);
        beam.transform.forward = endPoint - playerObj.transform.position + beam.transform.localPosition;
        beamAnimations.Blink(PlayerAnimations.AttackTime);
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
        Debug.Log(newPos);
    }
    public void CreateTitleUI()
    {
        GameObject canvastitle = playertitle;
        canvastitle.name = "Player_title";

        GameObject titletext = canvastitle.transform.Find("Text").gameObject;
    }
}
