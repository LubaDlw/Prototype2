using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Projectile behavior: moves forward and damages player on collision
[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    public int damage = 3;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerStats stats = collision.collider.GetComponent<PlayerStats>();
            if (stats != null)
                stats.dealDamage(damage);
        }
        // Optionally add impact effects here
        Destroy(gameObject);
    }
}
