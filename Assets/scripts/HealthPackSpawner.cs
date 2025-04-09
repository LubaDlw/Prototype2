using System.Collections;
using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    [Header("Pack Settings")]
    public GameObject healthPackPrefab;   // Reference to the health pack prefab
    public GameObject ammoPackPrefab;     // Reference to the ammo pack prefab
    public Transform[] spawnLocations;    // Array of spawn locations
    public float spawnInterval = 30f;     // Time interval in seconds between spawns

    private void Start()
    {
        // Start the spawning process
        StartCoroutine(SpawnPacks());
    }

    IEnumerator SpawnPacks()
    {
        // Continuously spawn health or ammo packs at intervals
        while (true)
        {
            // Choose a random spawn location from the array
            Transform spawnLocation = spawnLocations[Random.Range(0, spawnLocations.Length)];

            // Randomly decide whether to spawn a health pack or ammo pack
            GameObject packToSpawn = Random.Range(0, 2) == 0 ? healthPackPrefab : ammoPackPrefab;

            // Instantiate the chosen pack at the chosen location
            Instantiate(packToSpawn, spawnLocation.position, spawnLocation.rotation);

            // Wait for the specified interval before spawning the next pack
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
