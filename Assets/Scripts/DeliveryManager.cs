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

    //List of indexes 
    public List<int> USED_INDEXES;
 
    //Master text file that has informatino we need.
    public TextAsset ITEMS_MASTER_LIST;

    //The string array of every item in the game and its information.
    public string[] ITEMS_LIST;

    //The array that tells us how much of an item we have. Index is the same as items_list.
    public int[] NUMBER_OF_LIST;

    void Start()
    {
        //Initialize the inventory array for the item, and their total numbers
        if (ITEMS_MASTER_LIST != null)
        {
            ITEMS_LIST = ITEMS_MASTER_LIST.text.Split(' ', '\n');
            NUMBER_OF_LIST = new int[ITEMS_LIST.Length / 3];
        }
    }
     

    //Helper function to add to the delivery list the item reference.
    public void AddItem(string ITEM_REF)
    {
        //Item name retrieval and index retrieval
        string ITEM_NAME = ITEM_REF;
        int INDEX = GET_INDEX(ITEM_NAME);

        //Add the used index into the list if it isn't already there
        if (!USED_INDEXES.Contains(INDEX))
        {
            USED_INDEXES.Add(INDEX);
        }

        //Increment the number of that item we have
        NUMBER_OF_LIST[INDEX]++;

    }

    //Helper function that returns and removes an object from the list. Starts from oldest to newest.
    //Returns null if there is an error.
    public GameObject RemoveItem()
    {
        //Get the index of the oldest item to be added to the delivery service
        int INDEX = USED_INDEXES[0];

        //Decrease the number of NUMBER OF in that index
        NUMBER_OF_LIST[INDEX]--;

        //If we reached zero, remove that element from the used_indexes list
        if(NUMBER_OF_LIST[INDEX] == 0)
        {
            USED_INDEXES.RemoveAt(0);
        }

        //Get the prefab reference to that item
        return Resources.Load(ITEMS_LIST[INDEX + 2]) as GameObject;      
    }

    //Function that removes all items from the list until nothing remains, and returns it all as a GameObject array.
    public List<GameObject> GetAllDeliveries()
    {
        //The list we need to populate and return
        List<GameObject> RETURN_OBJ = new List<GameObject>();

        //While there are still used_indexes to parse
        while(USED_INDEXES.Count > 0)
        {
            RETURN_OBJ.Add(RemoveItem());
        }

        return RETURN_OBJ;
    }

    //Function that manages deliveries being put into the right place
    public void ManageDeliveries(List<GameObject> LIST)
    {
        //For every obj
        foreach(GameObject OBJ in LIST)
        {

        }
    }

    /// <summary>
    /// Returns the index of the object in the ITEMS_LIST Array
    /// Returns negative one if it isn't in there.
    /// </summary>
    /// <param name="InvObj"></param>
    /// <returns></returns>
    public int GET_INDEX(string OBJ_NAME)
    {
        //Debug.Log(InvObj);
        for (int i = 0; i < ITEMS_LIST.Length; i += 3)
        {
            if (ITEMS_LIST[i].Trim().Equals(OBJ_NAME.Trim()))
            {
                return i / 3;
            }
        }
        return -1;
    }



}
