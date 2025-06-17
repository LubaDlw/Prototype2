using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType1 : MonoBehaviour
{
    [Header("Type 1 Specific Settings")]
    public int bonusDamage = 1; // Extra damage this enemy type deals
    public float speedMultiplier = 1.2f; // This enemy type is slightly faster
    public Color enemyColor = Color.red; // Visual distinction
    
    private PlayerStats playerStats;
    private EnemyAI enemyAI;
    private Renderer enemyRenderer;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        enemyAI = GetComponent<EnemyAI>();
        enemyRenderer = GetComponent<Renderer>();
        
        ConfigureEnemyType();
    }

    void ConfigureEnemyType()
    {
        // Configure this specific enemy type
        if (enemyAI != null)
        {
            // Increase damage
            enemyAI.damageAmount += bonusDamage;
            
            // Increase speed
            enemyAI.speed *= speedMultiplier;
            
            // Apply wave scaling if player stats available
            if (playerStats != null)
            {
                int currentWave = playerStats.GetCurrentWave();
                enemyAI.SetWaveMultipliers(currentWave);
                
                // Type 1 specific scaling
                enemyAI.health += currentWave; // Extra health for Type 1
            }
        }
        
        // Change visual appearance
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = enemyColor;
        }
        
        Debug.Log($"EnemyType1 configured: Damage={enemyAI?.damageAmount}, Speed={enemyAI?.speed:F1}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Type 1 enemies could have special collision behavior
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("EnemyType1 special attack triggered!");
            
            // Could add special effects, sounds, or abilities here
            // For example: poison damage, knockback, etc.
        }
    }

    // Special ability that could be called periodically
    public void SpecialAbility()
    {
        // Example: Brief speed boost
        if (enemyAI != null)
        {
            StartCoroutine(SpeedBoost());
        }
    }

    private IEnumerator SpeedBoost()
    {
        float originalSpeed = enemyAI.speed;
        enemyAI.speed *= 1.5f;
        
        // Update NavMeshAgent speed
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = enemyAI.speed;
        }
        
        yield return new WaitForSeconds(3f);
        
        // Restore original speed
        enemyAI.speed = originalSpeed;
        if (agent != null)
        {
            agent.speed = enemyAI.speed;
        }
    }

    // Method to be called when this enemy type is spawned
    public void OnSpawned(int waveNumber)
    {
        Debug.Log($"EnemyType1 spawned in wave {waveNumber}");
        
        // Type 1 specific spawn behavior
        if (waveNumber >= 3)
        {
            // Start with special ability active for higher waves
            SpecialAbility();
        }
    }
}