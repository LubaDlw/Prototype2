using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileForce = 15f;
    public int projectileCount = 50;

    public Text projectileText; // bulletCountTxt

    // Audio variables
    public AudioClip shootSound;  // Audio clip for the shooting sound
    public AudioSource audioSource;  // AudioSource component for playing the sound

    private void Start()
    {
        projectileCount = 50;
      //  audioSource = GetComponent<AudioSource>();  // Get the AudioSource component attached to the player
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource component found on the player.");
        }
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
            // Play the shoot sound when the player shoots
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);  // Play the shooting sound once
            }
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
            projectileText.text = projectileCount.ToString();
        }
    }
}
