using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float speed = 3f;
    CharacterControllerMovement playerr;
    PlayerStats playerStats;

    private NavMeshAgent agent;

    void Start()
    {
        playerr = FindFirstObjectByType<CharacterControllerMovement>();
        playerStats = FindObjectOfType<PlayerStats>();

        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy!");
        }
    }

    void Update()
    {
        if (agent == null || player == null)
            return;

        float distance = Vector3.Distance(transform.position, playerr.transform.position);
      //  Debug.Log("Distance to player: " + distance);

        if (distance <= detectionRange)
        {
            agent.SetDestination(playerr.transform.position);
            Debug.Log("Setting destination to player.");

            if (distance <= attackRange)
            {
                // Add attack animation/logic here
                Debug.Log("Enemy attacking player!");
               // playerStats.dealDamage(1);
            }
        }
        else
        {
            agent.ResetPath(); // Stop moving if out of range
            Debug.Log("Out of detection range, stopping.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy attacking player in collision!");
         playerStats.dealDamage(1);
    }
}
