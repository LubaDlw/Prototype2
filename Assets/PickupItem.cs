using UnityEngine;
using System.Collections;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public GameObject pickupPrompt;         // UI prompt shown when near item
    private bool isPlayerNearby = false;

    private PlayerInventory inventory;      // Reference to PlayerInventory script
    private PlayerStats playerStats;        // Reference to PlayerStats script

    public TMP_Text pickUptxt;              // Text for pickup UI prompt
    public TMP_Text messageText;            // Message text for feedback
    //public TMP_Text resource1Txt;

    void Start()
    {
        // Cache references to required components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<PlayerInventory>();
        playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        // Check for pickup interaction
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.HeldItemCount == 0)
            {
                PickUp();
            }
            else
            {
                Debug.Log("Already holding an item. Drop it first!");
                ShowReturnMessage();
            }
        }
    }

    private void PickUp()
    {
        Debug.Log("Item picked up: " + gameObject.name);

        // Show pickup message
        ShowReturnMessage();
      
        //Ruturn to scientist logic

        // Update inventory count
        inventory.itemPickedUp++;

        // Inform PlayerStats for mission tracking
       /* if (playerStats != null)
        {
            playerStats.hasCollectedResourcePack = true;
        }*/

        // Hide pickup prompt
        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(false);
        }

        // Destroy the item from the scene
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            // Show prompt only if player is not holding something
            if (pickupPrompt != null && inventory.HeldItemCount == 0)
            {
                pickupPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Hide pickup prompt
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(false);
            }
        }
    }

    // Coroutine to show message for a few seconds
    public void ShowReturnMessage()
    {
        StartCoroutine(ShowMessageCoroutine("Item Picked Up, Return To Scientist", 2.5f));
    }

    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        messageText.gameObject.SetActive(false);
    }
}
