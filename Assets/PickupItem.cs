using UnityEngine;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public GameObject pickupPrompt; // assign in Inspector
    private bool isPlayerNearby = false;
    private PlayerInventory inventory;
    public TMP_Text pickUptxt;

    void Start()
    {
        // cache reference
        inventory = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<PlayerInventory>();
    }

    
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.HeldItemCount == 0)
            {
                PickUp();
            }
            else
            {
                Debug.Log("Already holding an item. Drop it first!");
            }
        }
    }

    private void PickUp()
    {
        Debug.Log("Item picked up: " + gameObject.name);

        // increment total pickups
        inventory.itemPickedUp++;

        // hide prompt and destroy item
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            // only show prompt if not holding an item
            if (pickupPrompt != null && inventory.HeldItemCount == 0)
                pickupPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (pickupPrompt != null)
                pickupPrompt.SetActive(false);
        }
    }
}
