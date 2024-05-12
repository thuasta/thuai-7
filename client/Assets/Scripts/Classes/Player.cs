using System.Collections.Generic;

using UnityEngine;
using static Thubg.Messages.CompetitionUpdate;

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
    public float FirearmRange;

    public void TryGetPlayerAnimations()
    {
        if (playerAnimations == null)
        {
            playerAnimations = playerObj.GetComponent<PlayerAnimations>();
        }
    }

    public void Attack(Position targetPosition, float range)
    {
        FaceTo(targetPosition);
        ShowGunFire(targetPosition, range);
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

    public void ShowGunFire(Position pos, float range)
    {
        Vector3 height = new Vector3(0, 1.8f, 0);
        Vector3 endPoint = new Vector3(pos.x, playerObj.transform.position.y, pos.y) + height;
        beam.transform.position = playerObj.transform.position + height ;
        MeshRenderer childComponent = beam.GetComponentInChildren<MeshRenderer>();
        childComponent.transform.localPosition = new Vector3(childComponent.transform.localPosition.x, childComponent.transform.localPosition.y, range/2 );
        childComponent.transform.localScale = new Vector3(childComponent.transform.localScale.x, childComponent.transform.localScale.y, range );

        beam.transform.LookAt(endPoint);
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
        //Debug.Log(newPos);
    }
    public void CreateTitleUI()
    {
        GameObject canvastitle = playertitle;
        canvastitle.name = "Player_title";

        GameObject titletext = canvastitle.transform.Find("Text").gameObject;
    }
}
