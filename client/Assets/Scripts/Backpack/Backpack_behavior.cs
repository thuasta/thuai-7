using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Backpack_behavior : MonoBehaviour
{
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI text_playerid;
    public int[] itemsCounts;
    public string[] gun_names;
    public GameObject Backpack;
    public int playerid, maxPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Backpack = GameObject.Find("Backpack_bar");
        playerid = 1;
        maxPlayer = 3;
        itemsCounts= new int[5];
        texts = new TextMeshProUGUI[7];
        gun_names= new string[2];
        texts[0] = GameObject.Find("Backpack_bar/bullets").GetComponent<TextMeshProUGUI>();
        texts[1] = GameObject.Find("Backpack_bar/firstaid").GetComponent<TextMeshProUGUI>();
        texts[2] = GameObject.Find("Backpack_bar/bandage").GetComponent<TextMeshProUGUI>();
        texts[3] = GameObject.Find("Backpack_bar/energydrink").GetComponent<TextMeshProUGUI>();
        texts[4] = GameObject.Find("Backpack_bar/grenade").GetComponent<TextMeshProUGUI>();
        texts[5] = GameObject.Find("Backpack_bar/gun_1").GetComponent<TextMeshProUGUI>();
        texts[6] = GameObject.Find("Backpack_bar/gun_2").GetComponent<TextMeshProUGUI>();
        text_playerid = GameObject.Find("Backpack_bar/playerid").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangePlayer();
        UpdateItemCounts();
        UpdateGunNames();
        UpdateTexts();
        OpenorCloseBackpack();
    }


    void UpdateItemCounts()
    {
        //test
        switch (playerid)
        {
            case 1:
                for (int i = 0; i < itemsCounts.Length; i++)
                {
                    itemsCounts[i] = i;
                };break;
            case 2:
                for (int i = 0; i < itemsCounts.Length; i++)
                {
                    itemsCounts[i] = 3*i+2;
                }; break;
            case 3:
                for (int i = 0; i < itemsCounts.Length; i++)
                {
                    itemsCounts[i] = 4 * i + 5;
                }; break;
            default: break;
        }

    }
    void UpdateGunNames()
    {
        //test
        switch (playerid)
        {
            case 1:
                gun_names[0] = "ak47";
                gun_names[1] = "NONE";
                break;
            case 2:
                gun_names[0] = "s686";
                gun_names[1] = "NONE";
                break;
            case 3:
                gun_names[0] = "awp";
                gun_names[1] = "vector";
                break;
            default: break;
        }
    }
    void UpdateTexts()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            if (i < itemsCounts.Length) texts[i].text = itemsCounts[i].ToString();
            else texts[i].text = gun_names[i - itemsCounts.Length];
        }
        text_playerid.text = "Player:" + playerid.ToString();
    }
    void OpenorCloseBackpack()
    {
        bool _pressB = Input.GetKeyDown(KeyCode.B);
        if (_pressB)
        {
            if(!Backpack.activeSelf)  Backpack.SetActive(true);
            else Backpack.SetActive(false);
        }
    }
    void ChangePlayer()
    {
        bool _pressQ = Input.GetKeyDown(KeyCode.Q);
        bool _pressE = Input.GetKeyDown(KeyCode.E);
        if (_pressQ)
        {
            if(playerid==1) playerid = maxPlayer;
            else playerid -=1;
        }
        if (_pressE)
        {
            if (playerid == maxPlayer) playerid = 1;
            else playerid += 1;
        }
    }
}
