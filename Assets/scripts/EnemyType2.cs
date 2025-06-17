using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType2 : MonoBehaviour
{
    
    [Header("Type 2 Shooter Settings")]
    public GameObject projectilePrefab;            // Prefab for the projectile
    public Transform firePoint;                    // Where projectiles are spawned
    public float shootInterval = 3f;               // Time between shots
    public float shootRange = 15f;                 // Maximum distance to player
    public float projectileSpeed = 10f;            // Speed of projectile

    private EnemyAI enemyAI;
    private Transform playerTransform;
    private float shootTimer;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        playerTransform = enemyAI.player;
        shootTimer = Random.Range(0, shootInterval);
    }

    void Update()
    {
        if (playerTransform == null || enemyAI == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        // Only shoot if within range and not dead
        if (distance <= shootRange && enemyAI.IsAlive())
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                ShootAtPlayer();
                shootTimer = shootInterval;
            }
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Instantiate and launch projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (playerTransform.position - firePoint.position).normalized;
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * projectileSpeed;
        }
    }
}

