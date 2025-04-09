using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int health = 20;
    public int hunger;
    public int vitality;
    public int amount;


    public void dealDamage(int amount)
    {
        health -= amount;
    }
}
