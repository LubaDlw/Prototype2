using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
   // [HideInInspector]
    public int itemPickedUp = 0;
   // [HideInInspector]
    public int dropItems = 0;

    /// <summary>
    /// How many items the player is currently holding.
    /// </summary>
    public int HeldItemCount
    {
        get { return itemPickedUp - dropItems; }
    }
}
