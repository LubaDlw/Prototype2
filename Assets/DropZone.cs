using UnityEngine;
using TMPro;

public class DropZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private PlayerInventory inventory;
    private PlayerStats playerStats;
    public TMP_Text dropZonetxt;
    public TMP_Text pickedUptxt;

    public TMP_Text resource1Txt;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        playerStats = GameObject.FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.HeldItemCount > 0)
            {
                inventory.dropItems++;
               // resource1Txt.text = " ";
                Debug.Log("Dropped an item. Total dropped: " + inventory.dropItems);
                pickedUptxt.text = " " ;
                dropZonetxt.text = inventory.dropItems + "/5";
                
                // Update mission progress
                if (playerStats != null)
                {
                    playerStats.hasCollectedResourcePack = true;
                }

                // Update Resource 1 UI text to green
                resource1Txt.color = Color.green;
                if(inventory.dropItems == 4)
                {
playerStats.collectItemMission5.color = Color.green;
                }
                
                

                // Additional logic for next steps if needed (e.g., mission progress)
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
