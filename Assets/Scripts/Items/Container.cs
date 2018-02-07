using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {

    //public Hashtable itemKeys = new Hashtable();
    public TextAsset allItems;
    public string[] possibleItems;
    public int[] numberOfIndItems;
    public int maxItems, currentNumItems;
    public string currentItem;
    bool resetAllValues;

    //Owner of this container
    public BaseItem.Owner OWNER;

	// Use this for initialization
	void Start () {
        //Initialize the inventory array for the item, and their total numbers
		if(allItems != null)
        {
            possibleItems = allItems.text.Split(' ', '\n');
            numberOfIndItems = new int[possibleItems.Length/3];
            /*for( int i = 0; i < possibleItems.Length; i++)
            {
                numberOfIndItems[i] = 0;
            }*/

        }
	}
	
	// Update is called once per frame
	void Update () {
        OWNER = transform.parent.GetComponent<BaseItem>()._OWNER;
	}

    void OnTriggerEnter(Collider collision)
    {
        //If the object is an Item, AND is interactable 
        if (collision.gameObject.GetComponent<BaseItem>() != null && collision.gameObject.layer == 8)
        {
            //If containable, try to put it in
            if (collision.gameObject.GetComponent<BaseItem>().CONTAINABLE)
            {
                //If this is the players container, put it in our inventory
                if (OWNER == BaseItem.Owner.Player && collision.gameObject.GetComponent<BaseItem>()._OWNER == OWNER)
                {
                    Debug.Log("Firing");
                    Inventory_Manager.AddItemToInventory(collision.gameObject.GetComponent<BaseItem>());
                    Destroy(collision.gameObject);
                }
                else //Normal behavior for other containers. Try to save what this container currently has.
                {
                    //Retrieve the relevant info into a variable for ease of reading
                    currentItem = collision.gameObject.GetComponent<BaseItem>()._NAME;
                    int tempIndex = getIndex(currentItem);
                    //Debug.Log(tempIndex);

                    //If we are already maxed out in this continer OR the owner is not the same as the container
                    if (currentNumItems >= maxItems || collision.gameObject.GetComponent<BaseItem>()._OWNER != OWNER)
                    {
                        //Eject the object from the container
                        ejectFromContainer(collision.gameObject);
                    }
                    else if (tempIndex != -1) //If the object is in the list of possible items to store NOTE: if it is negative one then it can't be stored.
                    {
                        //Increase the number of items for that item, destroy the physical object, keep track of total container items.
                        numberOfIndItems[tempIndex] += 1;
                        Destroy(collision.gameObject);
                        currentNumItems += 1;
                    }
                    else //We somehow made a mistake and the item isn't in the container. Mostly for debug. Prevents some crashing.
                    {
                        Debug.Log("Wrong Index");
                        ejectFromContainer(collision.gameObject);
                    }
                }
            }
            else //if it shouldn't be put in your inventory because it is not containable
            {
                //Eject the object from the container
                ejectFromContainer(collision.gameObject);
            }
        }
        else //The object we tried to throw in isn't a base item and isn't in the right layer.
        {
            //Debug.Log("Incorrect Item: Isn't a BaseItem AND/OR isn't in the right layer 8");
        }

    }

    //eject the object from the container
    public void ejectFromContainer(GameObject theObject)
    {
        //Set the owner to the same as the container
        //theObject.GetComponent<BaseItem>()._OWNER = OWNER;

        //PUsh the object up
        Vector3 thrust = new Vector3(Random.Range(-90.0f, 90.0f), 300, Random.Range(-90.0f, 90.0f));
        theObject.GetComponent<Rigidbody>().AddForce(Vector3.Scale(new Vector3(1,1,1), thrust));
       
    }
    /// <summary>
    /// Returns the index of the object in the possibleItems array
    /// Returns negative one if it isn't in there.
    /// </summary>
    /// <param name="InvObj"></param>
    /// <returns></returns>
    public int getIndex(string InvObj)
    {
        //Debug.Log(InvObj);
        for(int i = 0; i < possibleItems.Length; i+=3)
        {
            if (possibleItems[i].Trim().Equals(InvObj.Trim()))
            {
                Debug.Log(i);
                return i/3;
            }
        }
        return -1;
    }

    //Helper function that will "empty" the container.
    //Goes through the numIndItems and just sets them all to zero.
    public void EmptyContainer()
    {
        for(int i = 0; i < numberOfIndItems.Length; i++)
        {
            numberOfIndItems[i] = 0;
        }
        currentNumItems = 0;
    }



}
