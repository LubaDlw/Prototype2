using UnityEngine;
using System.Collections;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public GameObject pickupPrompt; // assign in Inspector
    private bool isPlayerNearby = false;
    private PlayerInventory inventory;
    public TMP_Text pickUptxt;
    public TMP_Text messageText;

    void Start()
    {
        // cache reference
        inventory = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<PlayerInventory>();
    }

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
                ShowReturnMessage();
            }
        }
    }

    private void PickUp()
    {
        Debug.Log("Item picked up: " + gameObject.name);
        ShowReturnMessage();

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
