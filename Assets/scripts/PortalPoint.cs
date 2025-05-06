using UnityEngine;
using System.Collections;

public class PortalPoint : MonoBehaviour
{
    [Tooltip("Drag the other portal's Transform here")]
    public Transform destinationPortal;

    [Tooltip("Seconds to wait before re-teleport is allowed")]
    public float teleportCooldown = 0.5f;

    // Internal flag to prevent immediate bounce-back
    [HideInInspector]
    public bool canTeleport = true;

    private PortalPoint destinationScript;

    void Awake()
    {
        // Cache the other portal's script so we can disable it too
        destinationScript = destinationPortal.GetComponent<PortalPoint>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only teleport the player when allowed
        if (!canTeleport) return;

        // Make sure your Player has the tag "Player"
        if (collision.collider.CompareTag("Player"))
            
        {
            StartCoroutine(TeleportRoutine(collision.collider.transform));
        }
    }

    private IEnumerator TeleportRoutine(Transform player)
    {
        // disable both portals temporarily
        canTeleport = false;
        if (destinationScript != null)
            destinationScript.canTeleport = false;

        // move the player
        player.position = destinationPortal.position;

        // optional: match rotation
        // player.rotation = destinationPortal.rotation;

        // wait before re-enabling
        yield return new WaitForSeconds(teleportCooldown);

        canTeleport = true;
        if (destinationScript != null)
            destinationScript.canTeleport = true;
    }
}
