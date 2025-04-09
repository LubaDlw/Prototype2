using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;    // Enemy prefab to be spawned
    public int[] waveEnemyCounts = new int[] { 3, 5, 10, 12, 12, 20 }; // Number of enemies for each wave
    public float[] waveDelays = new float[] { 0f, 30f, 30f, 60f, 60f, 60f }; // Delay before each wave

    [Header("Spawn Locations")]
    public Transform[] spawnPoints;   // Array of Transforms where enemies will spawn

    public PlayerStats playerStats; // Reference to PlayerStats to update wave notifications

    void Start()
    {
        // Start the wave spawning logic
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        for (int waveIndex = 0; waveIndex < waveEnemyCounts.Length; waveIndex++)
        {
            // Notify the player about the current wave
            if (playerStats != null)
            {
                playerStats.DisplayWaveNotification(waveIndex );
            }

            // Wait for the specified delay before spawning enemies for this wave
            if (waveDelays[waveIndex] > 0)
                yield return new WaitForSeconds(waveDelays[waveIndex]);

            // Spawn the enemies for this wave
            SpawnEnemies(waveEnemyCounts[waveIndex]);
        }
    }

    void SpawnEnemies(int numberOfEnemies)
    {
        // Loop to spawn the specified number of enemies
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Choose a random spawn point from the array
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = GetValidNavMeshPosition(spawnPoint.position);

            if (spawnPosition != Vector3.zero)
            {
                // Instantiate enemy at the valid NavMesh position
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Optionally configure the NavMeshAgent on the enemy
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed = 3f; // Example speed setting
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
        // Sample a position on the NavMesh near the given origin
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origin, out hit, 1f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            // Return Vector3.zero if no valid position is found
            return Vector3.zero;
        }
    }
}
