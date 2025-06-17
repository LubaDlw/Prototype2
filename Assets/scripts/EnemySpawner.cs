using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;


public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;    // Enemy prefab to be spawned
    public int[] waveEnemyCounts = new int[] { 3, 5, 8, 12, 15, 20, 25, 30 }; // Number of enemies for each wave
    public float[] spawnDelays = new float[] { 2f, 1.5f, 1f, 0.8f, 0.6f, 0.5f, 0.4f, 0.3f }; // Delay between individual enemy spawns
    public TMP_Text countdown4Waves;

    [Header("Spawn Locations")]
    public Transform[] spawnPoints;   // Array of Transforms where enemies will spawn

    [Header("Wave Integration")]
    public PlayerStats playerStats; // Reference to PlayerStats to get current wave info
    
    [Header("Spawn Settings")]
    public bool spawnContinuously = true; // Whether to spawn enemies throughout the wave
    public float continuousSpawnRate = 3f; // How often to spawn enemies during a wave (in seconds)
    public int maxEnemiesAlive = 15; // Maximum number of enemies alive at once
    
    private int currentWave = 0;
    private bool isSpawning = false;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private Coroutine currentSpawnCoroutine;

    void Start()
    {
        // Start with initial countdown
        StartCoroutine(InitialCountdown());
    }

    void Update()
    {
        // Clean up destroyed enemies from the list
        aliveEnemies.RemoveAll(enemy => enemy == null);
        
        // Check if we should be spawning based on PlayerStats wave system
        if (playerStats != null)
        {
            int playerCurrentWave = playerStats.GetCurrentWave();
            bool isWaveActive = playerStats.IsWaveActive();
            
            // If wave has changed, update our spawning
            if (playerCurrentWave != currentWave)
            {
                currentWave = playerCurrentWave;
                OnWaveChanged();
            }
            
            // Spawn enemies if wave is active and we should be spawning
            if (isWaveActive && spawnContinuously && !isSpawning && ShouldSpawnMore())
            {
                StartSpawning();
            }
            else if (!isWaveActive && isSpawning)
            {
                StopSpawning();
            }
        }
    }

    IEnumerator InitialCountdown()
    {
        int countdownTime = 10;
        while (countdownTime > 0)
        {
            countdown4Waves.text = $"1st Wave in {countdownTime} seconds...";
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        
        countdown4Waves.text = "";
        currentWave = 1; // Ready for first wave
    }

    void OnWaveChanged()
    {
        Debug.Log($"EnemySpawner: Wave changed to {currentWave}");
        
        // Stop current spawning
        StopSpawning();
        
        // Start spawning for new wave after a brief delay
        StartCoroutine(DelayedWaveStart());
    }

    IEnumerator DelayedWaveStart()
    {
        yield return new WaitForSeconds(1f); // Brief delay after wave starts
        
        if (playerStats != null && playerStats.IsWaveActive())
        {
            // Spawn initial enemies for this wave
            SpawnInitialEnemies();
            
            // Start continuous spawning if enabled
            if (spawnContinuously)
            {
                StartSpawning();
            }
        }
    }

    void SpawnInitialEnemies()
    {
        int enemiesToSpawn = GetEnemyCountForWave(currentWave);
        int initialSpawn = Mathf.Min(enemiesToSpawn / 2, 5); // Spawn half the wave's enemies initially, max 5
        
        Debug.Log($"Spawning {initialSpawn} initial enemies for wave {currentWave}");
        
        for (int i = 0; i < initialSpawn; i++)
        {
            SpawnSingleEnemy();
        }
    }

    void StartSpawning()
    {
        if (currentSpawnCoroutine == null)
        {
            isSpawning = true;
            currentSpawnCoroutine = StartCoroutine(ContinuousSpawning());
        }
    }

    void StopSpawning()
    {
        isSpawning = false;
        if (currentSpawnCoroutine != null)
        {
            StopCoroutine(currentSpawnCoroutine);
            currentSpawnCoroutine = null;
        }
    }

    IEnumerator ContinuousSpawning()
    {
        while (isSpawning && playerStats != null && playerStats.IsWaveActive())
        {
            if (ShouldSpawnMore())
            {
                SpawnSingleEnemy();
                
                // Use wave-appropriate spawn delay
                float delay = GetSpawnDelayForWave(currentWave);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                // Wait a bit before checking again
                yield return new WaitForSeconds(1f);
            }
        }
        
        isSpawning = false;
        currentSpawnCoroutine = null;
    }

    bool ShouldSpawnMore()
    {
        return aliveEnemies.Count < maxEnemiesAlive;
    }

    void SpawnSingleEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined!");
            return;
        }

        // Choose a random spawn point from the array
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = GetValidNavMeshPosition(spawnPoint.position);

        if (spawnPosition != Vector3.zero)
        {
            // Instantiate enemy at the valid NavMesh position
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            aliveEnemies.Add(enemy);

            // Configure the enemy based on current wave
            ConfigureEnemyForWave(enemy, currentWave);
        }
        else
        {
            Debug.LogWarning("No valid NavMesh position found near spawn point.");
        }
    }

    void ConfigureEnemyForWave(GameObject enemy, int wave)
    {
        // Configure NavMeshAgent
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // Increase speed slightly with each wave
            agent.speed = 3f + (wave * 0.2f);
        }

        // Configure EnemyAI if it exists
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            // Increase health slightly with each wave
            enemyAI.health = 9 + (wave - 1);
            enemyAI.speed = agent.speed;
        }
    }

    int GetEnemyCountForWave(int wave)
    {
        if (wave - 1 < waveEnemyCounts.Length)
        {
            return waveEnemyCounts[wave - 1];
        }
        else
        {
            // For waves beyond the array, scale up
            return waveEnemyCounts[waveEnemyCounts.Length - 1] + ((wave - waveEnemyCounts.Length) * 3);
        }
    }

    float GetSpawnDelayForWave(int wave)
    {
        if (wave - 1 < spawnDelays.Length)
        {
            return spawnDelays[wave - 1];
        }
        else
        {
            // For waves beyond the array, get faster but not too fast
            return Mathf.Max(0.2f, spawnDelays[spawnDelays.Length - 1] - ((wave - spawnDelays.Length) * 0.05f));
        }
    }

    Vector3 GetValidNavMeshPosition(Vector3 origin)
    {
        // Sample a position on the NavMesh near the given origin
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origin, out hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            // Return Vector3.zero if no valid position is found
            return Vector3.zero;
        }
    }

    // Public methods for external control
    public void ForceSpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnSingleEnemy();
        }
    }

    public int GetAliveEnemyCount()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
        return aliveEnemies.Count;
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in aliveEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        aliveEnemies.Clear();
    }

    // Debug method
    public void LogSpawnerStatus()
    {
        Debug.Log($"Current Wave: {currentWave}, Is Spawning: {isSpawning}, Alive Enemies: {GetAliveEnemyCount()}");
    }
}