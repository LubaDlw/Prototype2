using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;    
    public Transform firePoint;         
    public float projectileForce = 20f;    

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Default: Left Mouse Button or Ctrl
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileForce;
            }
        }
        else
        {
            Debug.LogWarning("Missing projectilePrefab or firePoint reference.");
        }
    }
}
