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
    
    [Header("Patrol Settings")]
    public float patrolRadius = 15f; // How far from spawn point to patrol
    public float patrolWaitTime = 2f; // Time to wait at each patrol point
    public float patrolSpeed = 2f; // Speed when patrolling (slower than chase)
    
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
    
    // Patrol system variables
    private Vector3 spawnPoint;
    private Vector3 currentPatrolTarget;
    private bool isChasing = false;
    private float patrolWaitTimer = 0f;
    private bool isWaitingAtPatrol = false;
    
    // Add these for better movement control
    private Vector3 lastPlayerPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 2f;
    private Vector3 lastPosition;

    void Start()
    {
        maxHealth = health;
        playerMovement = FindFirstObjectByType<CharacterControllerMovement>();
        playerStats = FindObjectOfType<PlayerStats>();

        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        // Store spawn point for patrol system
        spawnPoint = transform.position;

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
            ConfigureNavMeshAgent();
        }

        SetupAudio();
        
        // Initialize tracking variables
        lastPosition = transform.position;
        if (playerMovement != null)
            lastPlayerPosition = playerMovement.transform.position;
            
        // Start patrolling immediately
        SetNewPatrolTarget();
    }

    void ConfigureNavMeshAgent()
    {
        if (agent == null) return;
        
        agent.speed = patrolSpeed; // Start with patrol speed
        agent.acceleration = 12f;
        agent.angularSpeed = 450f;
        agent.stoppingDistance = 1f; // Larger stopping distance for patrol points
        
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.avoidancePriority = 50;
        agent.autoBraking = true; // Enable for smoother patrol stops
        
        Debug.Log($"NavMeshAgent configured - Patrol Speed: {patrolSpeed}, Chase Speed: {speed}");
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
        if (isDead || agent == null)
            return;

        CheckPlayerDetection();
        HandleMovementBehavior();
        HandleAudio();
        CheckIfStuck();
    }

    void CheckPlayerDetection()
    {
        if (playerMovement == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerMovement.transform.position);
        bool playerInRange = distanceToPlayer <= detectionRange;

        // Switch between chase and patrol modes
        if (playerInRange && !isChasing)
        {
            // Start chasing
            isChasing = true;
            isWaitingAtPatrol = false;
            agent.speed = speed; // Switch to chase speed
            agent.stoppingDistance = attackRange * 0.7f;
            agent.autoBraking = false; // More aggressive movement when chasing
            Debug.Log("Enemy started chasing player");
        }
        else if (!playerInRange && isChasing)
        {
            // Stop chasing, return to patrol
            isChasing = false;
            agent.speed = patrolSpeed; // Switch back to patrol speed
            agent.stoppingDistance = 1f;
            agent.autoBraking = true; // Smoother patrol movement
            SetNewPatrolTarget(); // Get new patrol destination
            Debug.Log("Enemy lost player, returning to patrol");
        }
    }

    void HandleMovementBehavior()
    {
        if (isChasing)
        {
            HandleChasing();
        }
        else
        {
            HandlePatrolling();
        }
    }

    void HandleChasing()
    {
        if (playerMovement == null) return;

        Vector3 playerPosition = playerMovement.transform.position;
        float distance = Vector3.Distance(transform.position, playerPosition);

        if (distance > attackRange)
        {
            // Chase the player
            agent.isStopped = false;
            
            // Update path frequently when chasing
            bool shouldUpdatePath = Vector3.Distance(lastPlayerPosition, playerPosition) > 0.5f || 
                                   !agent.hasPath || 
                                   agent.pathStatus != NavMeshPathStatus.PathComplete ||
                                   agent.remainingDistance < 0.5f;

            if (shouldUpdatePath)
            {
                // Predict player movement for better chasing
                Vector3 predictedPosition = playerPosition;
                Vector3 playerVelocity = (playerPosition - lastPlayerPosition) / Time.deltaTime;
                if (playerVelocity.magnitude > 1f)
                {
                    predictedPosition += playerVelocity * 0.5f;
                }
                
                agent.SetDestination(predictedPosition);
                lastPlayerPosition = playerPosition;
            }
        }
        else
        {
            // In attack range
            agent.isStopped = true;
            
            // Face the player
            Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
            directionToPlayer.y = 0;
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

    void HandlePatrolling()
    {
        // Handle waiting at patrol points
        if (isWaitingAtPatrol)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f)
            {
                isWaitingAtPatrol = false;
                SetNewPatrolTarget();
            }
            return;
        }

        // Check if we've reached the patrol target
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            // Reached patrol point, start waiting
            isWaitingAtPatrol = true;
            patrolWaitTimer = patrolWaitTime;
            agent.isStopped = true;
        }
        else
        {
            // Continue moving to patrol target
            agent.isStopped = false;
        }
    }

    void SetNewPatrolTarget()
    {
        // Generate a random point within patrol radius of spawn point
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += spawnPoint;
        
        // Make sure the point is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            currentPatrolTarget = hit.position;
            agent.SetDestination(currentPatrolTarget);
            agent.isStopped = false;
            Debug.Log($"Enemy set new patrol target: {currentPatrolTarget}");
        }
        else
        {
            // If no valid point found, just move towards spawn point
            currentPatrolTarget = spawnPoint;
            agent.SetDestination(currentPatrolTarget);
            agent.isStopped = false;
            Debug.Log("Enemy patrolling back to spawn point");
        }
    }

    void CheckIfStuck()
    {
        // Only check for stuck when actually trying to move
        if (!agent.isStopped && agent.hasPath && !isWaitingAtPatrol)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            
            if (distanceMoved < 0.1f)
            {
                stuckTimer += Time.deltaTime;
                
                if (stuckTimer >= stuckThreshold)
                {
                    Debug.Log("Enemy appears stuck, finding new path");
                    agent.ResetPath();
                    
                    if (isChasing && playerMovement != null)
                    {
                        // When chasing, try a different approach to player
                        Vector3 randomOffset = Random.insideUnitSphere * 3f;
                        randomOffset.y = 0;
                        Vector3 newDestination = playerMovement.transform.position + randomOffset;
                        agent.SetDestination(newDestination);
                    }
                    else
                    {
                        // When patrolling, get a new patrol target
                        SetNewPatrolTarget();
                    }
                    
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
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

        // Check if enemy is moving (excluding waiting periods)
        isMoving = !agent.isStopped && agent.velocity.magnitude > 0.1f && agent.hasPath && !isWaitingAtPatrol;

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
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }

        if (collision.collider.CompareTag("projectile"))
        {
            Debug.Log("Enemy hit by projectile");
            
            if (collision.contacts.Length > 0)
            {
                ShowBloodSplatter(collision.contacts[0].point);
            }
            
            TakeDamage(3);
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

        if (playerStats != null)
        {
            playerStats.enemiesKilled++;
            playerStats.coins += 4;
        }
        
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
        
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
        
        Destroy(gameObject, 0.5f);
    }

    void ShowBloodSplatter(Vector3 hitPoint)
    {
        if (bloodSplatterPrefab != null)
        {
            GameObject splatter = Instantiate(bloodSplatterPrefab, hitPoint, Quaternion.identity);
            Destroy(splatter, 5f);
        }
    }

    // Public methods
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
        health = maxHealth + (waveNumber - 1) * 2;
        maxHealth = health;
        
        speed = 3f + (waveNumber * 0.3f);
        patrolSpeed = 2f + (waveNumber * 0.2f); // Scale patrol speed too
        
        if (agent != null)
        {
            // Update current speed based on current state
            if (isChasing)
                agent.speed = speed;
            else
                agent.speed = patrolSpeed;
        }
        
        detectionRange = 10f + (waveNumber * 0.5f);
        patrolRadius = 15f + (waveNumber * 1f); // Larger patrol area in later waves
        
        Debug.Log($"Enemy configured for wave {waveNumber}: Health={health}, Chase Speed={speed:F1}, Patrol Speed={patrolSpeed:F1}");
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw patrol radius around spawn point
        Gizmos.color = Color.green;
        Vector3 spawn = Application.isPlaying ? spawnPoint : transform.position;
        Gizmos.DrawWireSphere(spawn, patrolRadius);
        
        // Draw current patrol target
        if (Application.isPlaying && !isChasing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currentPatrolTarget, 1f);
            Gizmos.DrawLine(transform.position, currentPatrolTarget);
        }
    }
}