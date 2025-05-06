using UnityEngine;

/// <summary>
/// Attach this to any GameObject with an "Is Trigger" collider set up as a teleporter.
/// When the specified tag enters the trigger, it will be moved to the destination transform.
/// </summary>
public class Teleporter : MonoBehaviour
{
    [Tooltip("The Transform to teleport the target to")]
    public Transform teleportTarget;

    [Tooltip("Tag of the object(s) to teleport (e.g., 'Player')")]
    public string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Only teleport objects with the matching tag
        if (!other.CompareTag(targetTag)) return;

        // If the object has a CharacterController, disable it before moving
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        // Teleport position and rotation
        other.transform.position = teleportTarget.position;
        other.transform.rotation = teleportTarget.rotation;

        // If there is a Rigidbody, reset its velocity so it doesn't "fly off"
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Re-enable CharacterController
        if (cc != null)
            cc.enabled = true;
    }
}
