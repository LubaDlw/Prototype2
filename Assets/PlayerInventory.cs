using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public PlayerStats playerStats;

    public int itemPickedUp = 0;
    public int dropItems = 0;

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")
                                .GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            Debug.Log("the economy is doing very nice");
        }
    }

    public int HeldItemCount
    {
        get { return itemPickedUp - dropItems; }
    }

    void Update()
    {
        if (dropItems >= 5)
        {
            Debug.Log("ENEMY DROPPED ALL ITEMS - ENEMY WINS");

            // Call GameWin method from PlayerStats
            if (playerStats != null)
            {
                playerStats.GameWin();
            }

            // You can also trigger animations or coroutines here
        }
    }
}
