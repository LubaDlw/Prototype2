using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 9;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float speed = 3f;
    public int damageAmount = 2;
    
    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    
    [Header("References")]
    public Transform player;
    public GameObject bloodSplatterPrefab;
    
    [Header("Audio")]
    public AudioClip movementClip;
    public AudioClip collisionClip;
    public AudioClip deathClip;
    
    private int maxHealth;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private bool isMoving = false;
    private bool isDead = false;
    
    // Component references
    private CharacterControllerMovement playerMovement;
    private PlayerStats playerStats;
    
    // Add these for better movement control
    private Vector3 lastPlayerPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 2f; // Time before considering enemy "stuck"
    private Vector3 lastPosition;

    void Start()
    {
        maxHealth = health;
        playerMovement = FindFirstObjectByType<CharacterControllerMovement>();
        playerStats = FindObjectOfType<PlayerStats>();

        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Validate components
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy!");
        }
        else
        {
            // Configure NavMeshAgent for optimal performance
            ConfigureNavMeshAgent();
        }

        // Set up audio
        SetupAudio();
        
        // Initialize tracking variables
        lastPosition = transform.position;
        if (playerMovement != null)
            lastPlayerPosition = playerMovement.transform.position;
    }

    void ConfigureNavMeshAgent()
    {
        if (agent == null) return;
        
        agent.speed = speed;
        agent.acceleration = 12f; // Higher acceleration for snappier movement
        agent.angularSpeed = 450f; // Even faster turning
        agent.stoppingDistance = attackRange * 0.7f; // Closer stopping distance
        
        // These settings help prevent slow movement
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Faster processing
        agent.avoidancePriority = 50; // Medium priority
        
        // Ensure the agent isn't being slowed down by default settings
        agent.autoBraking = false; // This can cause slowdowns near destination
        
        Debug.Log($"NavMeshAgent configured - Speed: {agent.speed}, Acceleration: {agent.acceleration}");
    }

    void SetupAudio()
    {
        if (audioSource != null && movementClip != null)
        {
            audioSource.clip = movementClip;
            audioSource.loop = true;
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("Missing AudioSource on enemy: " + gameObject.name);
        }
    }

    void Update()
    {
        if (isDead || agent == null || playerMovement == null)
            return;

        HandleMovementAndAttack();
        HandleAudio();
        CheckIfStuck();
    }

    void HandleMovementAndAttack()
    {
        Vector3 playerPosition = playerMovement.transform.position;
        float distance = Vector3.Distance(transform.position, playerPosition);

        if (distance <= detectionRange)
        {
            // Always ensure agent is not stopped when chasing
            if (distance > attackRange)
            {
                agent.isStopped = false;
                
                // More frequent path updates for better tracking
                bool shouldUpdatePath = Vector3.Distance(lastPlayerPosition, playerPosition) > 0.5f || 
                                       !agent.hasPath || 
                                       agent.pathStatus != NavMeshPathStatus.PathComplete ||
                                       agent.remainingDistance < 0.5f; // Update if close to current destination

                if (shouldUpdatePath)
                {
                    // Set destination with slight prediction for moving targets
                    Vector3 predictedPosition = playerPosition;
                    
                    // Try to predict where player will be
                    if (playerMovement != null)
                    {
                        Vector3 playerVelocity = (playerPosition - lastPlayerPosition) / Time.deltaTime;
                        if (playerVelocity.magnitude > 1f) // Only predict if player is moving fast enough
                        {
                            predictedPosition += playerVelocity * 0.5f; // Predict 0.5 seconds ahead
                        }
                    }
                    
                    agent.SetDestination(predictedPosition);
                    lastPlayerPosition = playerPosition;
                }
                
                // Force speed if agent seems to be slowing down
                if (agent.velocity.magnitude < speed * 0.7f && agent.hasPath)
                {
                    // Agent might be slowing down unnecessarily, force it to maintain speed
                    agent.speed = speed * 1.1f; // Slightly boost speed temporarily
                }
                else
                {
                    agent.speed = speed; // Reset to normal speed
                }
            }
            else
            {
                // In attack range - handle combat
                agent.isStopped = true;
                
                // Face the player
                Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
                directionToPlayer.y = 0; // Keep on horizontal plane
                if (directionToPlayer != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 8f);
                }
                
                // Attack if cooldown is over
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            // Out of detection range - stop moving
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    void CheckIfStuck()
    {
        // Check if enemy is stuck (not moving for too long while trying to move)
        if (!agent.isStopped && agent.hasPath)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            
            if (distanceMoved < 0.1f) // Very small movement
            {
                stuckTimer += Time.deltaTime;
                
                if (stuckTimer >= stuckThreshold)
                {
                    // Try to unstuck by recalculating path or moving slightly
                    Debug.Log("Enemy appears stuck, recalculating path");
                    agent.ResetPath();
                    
                    // Add a small random offset to destination to avoid getting stuck in same spot
                    if (playerMovement != null)
                    {
                        Vector3 randomOffset = Random.insideUnitSphere * 2f;
                        randomOffset.y = 0;
                        Vector3 newDestination = playerMovement.transform.position + randomOffset;
                        agent.SetDestination(newDestination);
                    }
                    
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f; // Reset timer if moving normally
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        
        lastPosition = transform.position;
    }

    void HandleAudio()
    {
        if (audioSource == null || movementClip == null)
            return;

        // Check if enemy is moving (better detection)
        isMoving = !agent.isStopped && agent.velocity.magnitude > 0.1f && agent.hasPath;

        // Play or stop movement sound
        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void AttackPlayer()
    {
        if (playerStats != null)
        {
            Debug.Log($"Enemy attacking player for {damageAmount} damage!");
            playerStats.dealDamage(damageAmount);
            
            // Play collision sound
            if (audioSource != null && collisionClip != null)
            {
                audioSource.PlayOneShot(collisionClip);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.collider.CompareTag("Player"))
        {
            // Attack on collision (immediate damage)
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }

        if (collision.collider.CompareTag("projectile"))
        {
            Debug.Log("Enemy hit by projectile");
            
            // Show blood effect
            if (collision.contacts.Length > 0)
            {
                ShowBloodSplatter(collision.contacts[0].point);
            }
            
            // Take damage
            TakeDamage(3); // Standardized projectile damage
            
            // Destroy the projectile
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {health}/{maxHealth}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log("Enemy died!");

        // Update player stats
        if (playerStats != null)
        {
            playerStats.enemiesKilled++;
            playerStats.coins += 4; // Reward for killing an enemy
        }
        
        // Play death sound
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
        
        // Stop movement
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
        
        // Destroy after a brief delay (allows death sound to play)
        Destroy(gameObject, 0.5f);
    }

    void ShowBloodSplatter(Vector3 hitPoint)
    {
        if (bloodSplatterPrefab != null)
        {
            GameObject splatter = Instantiate(bloodSplatterPrefab, hitPoint, Quaternion.identity);
            
            // Auto-destroy blood splatter after some time
            Destroy(splatter, 5f);
        }
    }

    // Public methods for external use
    public bool IsAlive()
    {
        return !isDead;
    }

    public float GetHealthPercentage()
    {
        return (float)health / maxHealth;
    }

    public void SetWaveMultipliers(int waveNumber)
    {
        // Scale enemy based on wave number
        health = maxHealth + (waveNumber - 1) * 2;
        maxHealth = health;
        
        speed = 3f + (waveNumber * 0.3f);
        
        // Update NavMeshAgent speed properly and reconfigure for new wave
        if (agent != null)
        {
            agent.speed = speed;
            // Reconfigure other settings that might affect movement speed
            ConfigureNavMeshAgent();
        }
        
        // Increase detection range slightly
        detectionRange = 10f + (waveNumber * 0.5f);
        
        Debug.Log($"Enemy configured for wave {waveNumber}: Health={health}, Speed={speed:F1}");
    }

    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw stopping distance
        if (agent != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        }
    }
}