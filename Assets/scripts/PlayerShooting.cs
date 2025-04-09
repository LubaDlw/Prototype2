using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;    
    public Transform firePoint;         
    public float projectileForce = 15f;
    public int projectileCount = 50;

    public Text projectileText; // bulletCountTxt

    //public GameObject smokePrefab;

    private void Start()
    {
        projectileCount = 50;
    }

    public void addAmmo()
    {
        if (projectileCount < 40)
        {
            projectileCount += 10;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && projectileCount > 0) // Default: Left Mouse Button or Ctrl
        {
            Shoot();
        }

        UpdateProjectileUI();
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            projectileCount -= 1;
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileForce;
            }

            //if (smokePrefab != null)
            //{
            //    Instantiate(smokePrefab, firePoint.position, firePoint.rotation);
            //}
        }
        else
        {
            Debug.LogWarning("Missing projectilePrefab or firePoint reference.");
        }
    }

    void UpdateProjectileUI()
    {
        if (projectileText != null)
        {
            projectileText.text =  projectileCount.ToString();
        }
    }
}
