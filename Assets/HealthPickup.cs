using UnityEngine;
using TMPro;

public class HealthPickup : MonoBehaviour
{
    public int healthRestoration = 10;  // Amount of health restored when picked up
    PlayerStats playerStats;
    public int maxHealth = 100;
    private void Start()
    {
         maxHealth = 100;
        playerStats = FindObjectOfType<PlayerStats>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Health Collided wt Enemy ");
            playerStats.healthPack();
            Destroy(gameObject);
        }
    }

}
