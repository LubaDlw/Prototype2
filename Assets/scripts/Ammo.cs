using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    //public int healthRestoration = 10;  // Amount of health restored when picked up
   // PlayerStats playerStats;
   // public int maxHealth = 100;
    PlayerShooting playershoot;
    private void Start()
    {
        //maxHealth = 100;
        //playerStats = FindObjectOfType<PlayerStats>();
        playershoot = FindObjectOfType<PlayerShooting>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ammo Collided wt Enemy ");
            //  playerStats.healthPack();
            playershoot.addAmmo();
            Destroy(gameObject);
        }
    }
}
