using UnityEngine;
using TMPro;

public class DropZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private PlayerInventory inventory;
    public TMP_Text dropZonetxt;
    //private DropZone dropZone;
    private PickupItem pickUpItem;
    private PlayerStats playerStats;

    public TMP_Text resource1Txt;
    

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<PlayerInventory>();
                              
        playerStats = GameObject.FindObjectOfType<PlayerStats>();

        //dropZone = FindObjectofType<DropZone>();
        pickUpItem = FindObjectOfType<PickupItem>();
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
                pickUpItem.messageText.text = " ";
                Debug.Log("Dropped an item. Total dropped: " + inventory.dropItems);
                dropZonetxt.text = inventory.dropItems + "/5";
                
if (playerStats != null)
        {
            playerStats.hasCollectedResourcePack = true;
        }

  resource1Txt.color = Color.green;
                // dropZonetxt.text = "Dropped an item. Total dropped: " + inventory.dropItems.ToString();
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
