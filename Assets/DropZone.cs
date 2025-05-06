using UnityEngine;


public class DropZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private PlayerInventory inventory;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.HeldItemCount > 0)
            {
                inventory.dropItems++;
                Debug.Log("Dropped an item. Total dropped: " + inventory.dropItems);
            }
            else
            {
                Debug.Log("No items to drop!");
            }
        }
    }
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInZone = false;
    }
}
