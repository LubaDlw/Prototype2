using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform portalA;
    public Transform portalB;

    private Transform currentPortal = null;

    void Update()
    {
        if (currentPortal != null && Input.GetKeyDown(KeyCode.E))
        {
            // Check which portal you're at, and teleport to the other
            if (currentPortal == portalA)
                transform.position = portalB.position;
            else if (currentPortal == portalB)
                transform.position = portalA.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == portalA || other.transform == portalB)
        {
            currentPortal = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == currentPortal)
        {
            currentPortal = null;
        }
    }
}
