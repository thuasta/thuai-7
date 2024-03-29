//==============================================================
// Demo Buttons
//==============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public void Button1()
    {
        HealthSystem.Instance.TakeDamage(10f); // Take damage 10 points
    }
    public void Button2()
    {
        HealthSystem.Instance.HealDamage(10f); // Heal damage 10 points
    }
    public void Button3()
    {
        HealthSystem.Instance.UseMana(10f); // Decrease mana 10 points
    }
    public void Button4()
    {
        HealthSystem.Instance.RestoreMana(10f); // Increase mana 10 points
    }
    public void Button5()
    {
        HealthSystem.Instance.SetMaxHealth(10f); // Add 10 % to max health
    }
    public void Button6()
    {
        HealthSystem.Instance.SetMaxMana(10f); // Add 10 % to max mana
    }
}
