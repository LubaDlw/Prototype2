using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private int enemyHealth;
    public int health = 9;
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float speed = 3f;
    CharacterControllerMovement playerr;
    PlayerStats playerStats;
    public GameObject bloodSplatterPrefab;

    private NavMeshAgent agent;

    // Audio-related variables
    public AudioClip movementClip;  // Audio clip for enemy's movement sound
    public AudioClip collisionClip; // Audio clip for enemy collision sound
    private AudioSource audioSource;  // AudioSource component to play the sound
    private bool isMoving = false;  // To check if the enemy is moving

    void Start()
    {
        health = 9;
        playerr = FindFirstObjectByType<CharacterControllerMovement>();
        playerStats = FindObjectOfType<PlayerStats>();

        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component attached to the enemy

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy!");
        }

        // Set up the AudioSource for movement
        if (audioSource != null && movementClip != null)
        {
            audioSource.clip = movementClip;
            audioSource.loop = true;  // Set the movement sound to loop while the enemy is moving
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or movement clip on enemy.");
        }
    }

    void Update()
    {
        if (agent == null || player == null)
            return;

        float distance = Vector3.Distance(transform.position, playerr.transform.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(playerr.transform.position);
            Debug.Log("Setting destination to player.");

            if (distance <= attackRange)
            {
                // Add attack animation/logic here
                Debug.Log("Enemy attacking player!");
            }
        }
        else
        {
            agent.ResetPath(); // Stop moving if out of range
            Debug.Log("Out of detection range, stopping.");
        }

        // Check if the enemy is moving
        isMoving = agent.velocity.magnitude > 0.1f;

        // Play or stop the movement sound based on whether the enemy is moving
        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();  // Play the movement sound if the enemy is moving
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();  // Stop the movement sound if the enemy is not moving
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Collided WITH Player");
            playerStats.dealDamage(1);

            // Play the collision sound when colliding with the player
            if (audioSource != null && collisionClip != null)
            {
                audioSource.PlayOneShot(collisionClip);  // Play the collision sound once
            }
        }

        if (collision.collider.CompareTag("projectile"))
        {
            Debug.Log(" Collided with projectile in enemy script");
            ShowBloodSplatter(collision.contacts[0].point);
            health -= 6 / 2;
            Destroy(collision.gameObject);
        }

        if (health <= 0)
        {
            playerStats.enemiesKilled += 1;
            Destroy(gameObject);
        }
    }

    void ShowBloodSplatter(Vector3 hitPoint)
    {
        // Instantiate the blood splatter effect at the collision point
        if (bloodSplatterPrefab != null)
        {
            Instantiate(bloodSplatterPrefab, hitPoint, Quaternion.identity);
        }
    }
}
