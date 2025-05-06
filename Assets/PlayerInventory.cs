using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PlayerStats playerStats;
   // [HideInInspector]
    public int itemPickedUp = 0;
   // [HideInInspector]
    public int dropItems = 0;

    /// <summary>
    /// How many items the player is currently holding.
    /// </summary>
    /// 
    void start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<PlayerStats>();
    }
    public int HeldItemCount
    {
        get { return itemPickedUp - dropItems; }
    }

    void update()
    {
        if (dropItems >= 5)
        {
            Debug.log("ENEMY DROPP ALL ITEMS - ENEMY WINS")
                // Start couritine for  scientist cooking ingredients 
                // animation
                //WIn logic
        }
    }
}
