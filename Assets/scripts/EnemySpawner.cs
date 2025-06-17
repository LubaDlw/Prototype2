using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;             // Melee enemy prefab to be spawned
    public GameObject shooterEnemyPrefab;      // Shooter enemy prefab to be spawned
    [Tooltip("Chance to spawn a shooter enemy instead of a melee one on each spawn")]
    [Range(0f, 1f)] public float shooterSpawnProbability = 0.2f;

    public int[] waveEnemyCounts = new int[] { 3, 5, 8, 12, 15, 20, 25, 30 };
    public float[] spawnDelays = new float[] { 2f, 1.5f, 1f, 0.8f, 0.6f, 0.5f, 0.4f, 0.3f };
    public TMP_Text countdown4Waves;

    [Header("Spawn Locations")]
    public Transform[] spawnPoints;

    [Header("Wave Integration")]
    public PlayerStats playerStats;

    [Header("Spawn Settings")]
    public bool spawnContinuously = true;
    public float continuousSpawnRate = 3f;
    public int maxEnemiesAlive = 15;

    private int currentWave = 0;
    private bool isSpawning = false;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private Coroutine currentSpawnCoroutine;

    void Start()
    {
        StartCoroutine(InitialCountdown());
    }

    void Update()
    {
        aliveEnemies.RemoveAll(e => e == null);

        if (playerStats != null)
        {
            int playerCurrentWave = playerStats.GetCurrentWave();
            bool isWaveActive = playerStats.IsWaveActive();

            if (playerCurrentWave != currentWave)
            {
                currentWave = playerCurrentWave;
                OnWaveChanged();
            }

            if (isWaveActive && spawnContinuously && !isSpawning && ShouldSpawnMore())
                StartSpawning();
            else if (!isWaveActive && isSpawning)
                StopSpawning();
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
        currentWave = 1;
    }

    void OnWaveChanged()
    {
        Debug.Log($"EnemySpawner: Wave changed to {currentWave}");
        StopSpawning();
        StartCoroutine(DelayedWaveStart());
    }

    IEnumerator DelayedWaveStart()
    {
        yield return new WaitForSeconds(1f);
        if (playerStats != null && playerStats.IsWaveActive())
        {
            SpawnInitialEnemies();
            if (spawnContinuously)
                StartSpawning();
        }
    }

    void SpawnInitialEnemies()
    {
        int enemiesToSpawn = GetEnemyCountForWave(currentWave);
        int initialSpawn = Mathf.Min(enemiesToSpawn / 2, 5);
        Debug.Log($"Spawning {initialSpawn} initial enemies for wave {currentWave}");

        for (int i = 0; i < initialSpawn; i++)
            SpawnSingleEnemy();
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
                float delay = GetSpawnDelayForWave(currentWave);
                yield return new WaitForSeconds(delay);
            }
            else
                yield return new WaitForSeconds(1f);
        }

        isSpawning = false;
        currentSpawnCoroutine = null;
    }

    bool ShouldSpawnMore() => aliveEnemies.Count < maxEnemiesAlive;

    void SpawnSingleEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = GetValidNavMeshPosition(spawnPoint.position);
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("No valid NavMesh position found near spawn point.");
            return;
        }

        // Choose prefab type based on probability
        GameObject prefabToSpawn = (shooterEnemyPrefab != null && Random.value < shooterSpawnProbability)
            ? shooterEnemyPrefab
            : enemyPrefab;

        GameObject enemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        aliveEnemies.Add(enemy);

        // Configure stats based on wave
        ConfigureEnemyForWave(enemy, currentWave);
    }

    void ConfigureEnemyForWave(GameObject enemy, int wave)
    {
        // Configure NavMeshAgent if present
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.speed = 3f + (wave * 0.2f);

        // Configure EnemyAI (both melee and shooter inherit EnemyAI)
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.health = 9 + (wave - 1);
            enemyAI.speed = agent != null ? agent.speed : enemyAI.speed;
            enemyAI.SetWaveMultipliers(wave);
        }

        // Shooter enemies may have additional setup via their own script
    }

    int GetEnemyCountForWave(int wave)
    {
        if (wave - 1 < waveEnemyCounts.Length)
            return waveEnemyCounts[wave - 1];
        return waveEnemyCounts[waveEnemyCounts.Length - 1] + ((wave - waveEnemyCounts.Length) * 3);
    }

    float GetSpawnDelayForWave(int wave)
    {
        if (wave - 1 < spawnDelays.Length)
            return spawnDelays[wave - 1];
        return Mathf.Max(0.2f, spawnDelays[spawnDelays.Length - 1] - ((wave - spawnDelays.Length) * 0.05f));
    }

    Vector3 GetValidNavMeshPosition(Vector3 origin)
    {
        if (NavMesh.SamplePosition(origin, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            return hit.position;
        return Vector3.zero;
    }

    // Public control methods
    public void ForceSpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
            SpawnSingleEnemy();
    }

    public int GetAliveEnemyCount()
    {
        aliveEnemies.RemoveAll(e => e == null);
        return aliveEnemies.Count;
    }

    public void ClearAllEnemies()
    {
        foreach (var e in aliveEnemies)
            if (e != null)
                Destroy(e);
        aliveEnemies.Clear();
    }

    public void LogSpawnerStatus()
    {
        Debug.Log($"Current Wave: {currentWave}, Is Spawning: {isSpawning}, Alive Enemies: {GetAliveEnemyCount()}");
    }
}
