using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manager that keeps track of items that need to be delivered to the player and sends it to the player first thing in a day.
public class DeliveryManager : MonoBehaviour
{
    //Singleton code.
    private static DeliveryManager _instance;

    public static DeliveryManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    //The string array of every item in the game and its information.
    public List<GameObject> ITEMS_LIST;

    //Helper function to add to the delivery list the item reference.
    public void AddItem(GameObject ITEM_REF)
    {
        ITEMS_LIST.Add(ITEM_REF);

    }

    //Helper function that returns and removes an object from the list. Starts from oldest to newest.
    //Returns null if there is an error.
    public GameObject RemoveItem()
    {
        GameObject TEMP = ITEMS_LIST[0];
        ITEMS_LIST.RemoveAt(0);

        //Get the prefab reference to that item
        return TEMP;
    }

    //Function that removes all items from the list until nothing remains, and returns it all as a GameObject array.
    public List<GameObject> GetAllDeliveries()
    {
        List<GameObject> RETURN_LIST = ITEMS_LIST;
        ITEMS_LIST.Clear();
        return RETURN_LIST;
    }

    //Function that manages deliveries being put into the right place
    public void ManageDeliveries()
    {
        //For every obj
        foreach(GameObject OBJ in ITEMS_LIST)
        {
            Inventory_Manager.AddItemToInventory(OBJ.GetComponent<BaseItem>());
        }
    }


}
