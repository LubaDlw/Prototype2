using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;    // Enemy prefab to be spawned
    public int numberOfEnemies = 5;   // Number of enemies to spawn
    public float spawnRadius = 1f;    // Radius to sample NavMesh from the spawn point

    [Header("Spawn Locations")]
    public Transform[] spawnPoints;   // Array of Transforms where enemies will spawn

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        // Loop through all the spawn points
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Get a valid position on the NavMesh near the spawn point
            Vector3 spawnPosition = GetValidNavMeshPosition(spawnPoint.position);

            if (spawnPosition != Vector3.zero)
            {
                // Instantiate the enemy at the valid NavMesh position
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Get the NavMeshAgent component on the spawned enemy
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    // Optional: Set NavMeshAgent properties if needed (e.g., speed, acceleration)
                    agent.speed = 3f; // Example speed setting
                  //  Debug.Log("Enemy spawned at: " + spawnPosition);
                }
                else
                {
                    Debug.LogWarning("No NavMeshAgent found on the enemy prefab!");
                }
            }
            else
            {
                Debug.LogWarning("No valid NavMesh position found near spawn point.");
            }
        }
    }

    Vector3 GetValidNavMeshPosition(Vector3 origin)
    {
        // Sample position on the NavMesh within a specified radius of the origin
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origin, out hit, spawnRadius, NavMesh.AllAreas))
        {
            return hit.position; // Return the valid position on the NavMesh
        }
        else
        {
            // Return Vector3.zero if no valid position is found
            return Vector3.zero;
        }
    }
}
