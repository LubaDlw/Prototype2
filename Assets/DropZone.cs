using UnityEngine;
using TMPro;

public class DropZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private PlayerInventory inventory;
    public TMP_Text dropZonetxt;
    
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<PlayerInventory>();
       // dropZonetxt.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.HeldItemCount > 0)
            {
                inventory.dropItems++;
              //  dropZonetxt.SetActive(true);
                Debug.Log("Dropped an item. Total dropped: " + inventory.dropItems);
                dropZonetxt.text = "Dropped an item. Total dropped: " + inventory.dropItems.ToString();
               // dropZonetxt.SetActive(false);
            }
            else
            {
                Debug.Log("No items to drop!");
            }
        }
    }

   // public IEnume
   

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
