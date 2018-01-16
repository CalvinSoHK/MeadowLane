using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script that uses a trigger collider to detect objects in a container.
public class SmartSensor : MonoBehaviour {

    //Reference to the empty objects that contain the element and count textboxes
    public GameObject ELEM_COUNT, ELEM_TYPE, ELEM_PRICE, ELEM_TOTAL;

    //Number of items to display
    int NUM_OF_ELEMENTS = 0;

    //The text label for the earnings
    public Text TOTAL_TEXT;

    //The total earnings as of now
    int TOTAL;

    //Lists that have our textboxes
    List<Text> COUNT_TEXT_LIST = new List<Text>(), TYPE_TEXT_LIST = new List<Text>(), PRICE_PER_TEXT_LIST = new List<Text>(), PRICE_TOTAL_TEXT_LIST = new List<Text>();

    //The container that we are displaying
    List<Container> CONTAINER_LIST = new List<Container>();

    //NOTE: this would be infinitely easier with a custom class.
    //A list for each of the elements. These lists represent the consolidated data from multiple containers.
    List<ELEMENT_INFO> ELEMENT_LIST = new List<ELEMENT_INFO>();

    //Start script 
    void Start()
    {
        //Initialize our lists.
        //Technically could be one list if they are the same length. But just in case.
        for(int i = 0; i < ELEM_COUNT.transform.childCount; i++)
        {
            COUNT_TEXT_LIST.Add(ELEM_COUNT.transform.GetChild(i).GetComponent<Text>());
        }

        for (int i = 0; i < ELEM_TYPE.transform.childCount; i++)
        {
            TYPE_TEXT_LIST.Add(ELEM_TYPE.transform.GetChild(i).GetComponent<Text>());
        }

        for (int i = 0; i < ELEM_PRICE.transform.childCount; i++)
        {
            PRICE_PER_TEXT_LIST.Add(ELEM_PRICE.transform.GetChild(i).GetComponent<Text>());
        }

        for (int i = 0; i < ELEM_TOTAL.transform.childCount; i++)
        {
            PRICE_TOTAL_TEXT_LIST.Add(ELEM_TOTAL.transform.GetChild(i).GetComponent<Text>());
        }


        ClearList();
    }

    private void Update()
    {
        //If nothing to display, clear the list.
        if(CONTAINER_LIST.Count == 0)
        {
            ClearList();
        }
        else //Calculate all the values we need to display
        {
            UpdateList();
            DisplayInfo();
        }
        TOTAL = GetTotal(ELEMENT_LIST);
        TOTAL_TEXT.text = TOTAL + "";
    }


    //Trigger enter
    void OnTriggerEnter(Collider col)
    {
        //If the object is an item
        if(col.gameObject.GetComponent<BaseItem>() != null)
        {
            //If this has the tag cont ainer
            if (col.gameObject.GetComponent<BaseItem>().hasTag(BaseItem.ItemTags.Container))
            {
                //Add to list.
                CONTAINER_LIST.Add(col.gameObject.transform.GetChild(0).GetComponent<Container>());
            }
          
        }
    }

    //On exit, remove the container from the list and recalculate.
    private void OnTriggerExit(Collider col)
    {
        //If the object is an item
        if (col.gameObject.GetComponent<BaseItem>() != null)
        {
            //If this has the tag cont ainer
            if (col.gameObject.GetComponent<BaseItem>().hasTag(BaseItem.ItemTags.Container))
            {
                //Add to list.
                RemoveContainer(col.gameObject.transform.GetChild(0).GetComponent<Container>());
            }

        }
    }

    /// <summary>
    /// Returns the total value of the containers in question.
    /// </summary>
    /// <returns></returns>
    public int GetTotal(List<ELEMENT_INFO> LIST)
    {
        int total = 0;

        foreach(ELEMENT_INFO INFO in LIST)
        {
            total += INFO.TOTAL_PRICE;
        }
        return total;
    }

    /// <summary>
    /// Helper function that updates all information pertaining to one container to a given list
    /// </summary>
    public void UpdateContainer(Container CONTAINER, List<ELEMENT_INFO> LIST)
    {
        //For all elements in a container
        for (int i = 0; i < CONTAINER.numberOfIndItems.Length; i++)
        {
            //The data values we need to add
            string TYPE = CONTAINER.possibleItems[(i + 1) * 3 - 3];
            int COUNT = CONTAINER.numberOfIndItems[i];
            int PRICE_PER = int.Parse(CONTAINER.possibleItems[(i + 1) * 3 - 2]);

            //If the possible item has at least 1 of it.
            if (CONTAINER.numberOfIndItems[i] != 0)
            {
                //If we haven't yet added the item to the TYPE_LIST
                if (!HasType(TYPE, LIST))
                {
                    //"Init" this product in our lists.
                    //Note: Price total is quivalent to our price per times count for now.
                    LIST.Add(new ELEMENT_INFO(TYPE, COUNT, PRICE_PER));
                }
                else//It contains the type we're trying to add, we should consolidate the info.
                {
                    int index = IndexOfType(TYPE, LIST);

                    //Then add to the valid lists
                    LIST[index].COUNT += COUNT;
                    LIST[index].TOTAL_PRICE = ELEMENT_LIST[index].COUNT * PRICE_PER;
                }
            }
        }
    }

    /// <summary>
    /// Helper function that updates the list ELEMENT_LIST to keep all information up to date.
    /// </summary>
    public void UpdateList()
    {
        //For each ELEMENT_INFO in our lists.
        foreach(ELEMENT_INFO INFO in ELEMENT_LIST)
        {
            INFO.COUNT = 0;
        }

        //Count the total for each element in each container 
        foreach (Container CONTAINER in CONTAINER_LIST)
        {
            UpdateContainer(CONTAINER, ELEMENT_LIST);
        }
    }

    //Display all pertinent info
    public void DisplayInfo()
    {
        //As many elements that are in our lists.
        foreach(ELEMENT_INFO INFO in ELEMENT_LIST)
        {
            SetElement(IndexOfType(INFO.TYPE, ELEMENT_LIST), INFO.TYPE, INFO.PRICE_PER + "" , INFO.COUNT, INFO.TOTAL_PRICE);
        }
    }


    //Helper function that removes a container from our list and does the necessary work
    void RemoveContainer(Container container)
    {
        //For every element, update its info, either removing it completely, or modifying the values
        for(int i = 0; i < container.numberOfIndItems.Length; i++)
        {
            //Only needs to do something if it has some items 
            if(container.numberOfIndItems[i] > 0)
            {
                //The data values we need to check for
                string TYPE = container.possibleItems[(i + 1) * 3 - 3];
                int COUNT = container.numberOfIndItems[i];
                int PRICE_PER = int.Parse(container.possibleItems[(i + 1) * 3 - 2]);

                //Get the index of that type of produce
                int index = IndexOfType(TYPE, ELEMENT_LIST);

                //Update the count. If it's zero, we should remove the entire item from all lists.
                ELEMENT_LIST[index].COUNT -= COUNT;
                if (ELEMENT_LIST[index].COUNT <= 0)
                {
                    ELEMENT_LIST.RemoveAt(index);
                }
                else //If some other container has that object, don't remove it, just update the total values.
                {
                    ELEMENT_LIST[index].TOTAL_PRICE = ELEMENT_LIST[index].PRICE_PER * ELEMENT_LIST[index].COUNT;
                }
            }
           
        }

        //Last step, remove it from the container list.
        CONTAINER_LIST.Remove(container);
    }

    //Helper function that clears the monitor
    void ClearList()
    {
        foreach(Text text in COUNT_TEXT_LIST)
        {
            text.text = "";
        }

        foreach(Text text in TYPE_TEXT_LIST)
        {
            text.text = "";
        }

        foreach (Text text in PRICE_PER_TEXT_LIST)
        {
            text.text = "";
        }

        foreach(Text text in PRICE_TOTAL_TEXT_LIST)
        {
            text.text = "";
        }

        ELEMENT_LIST.Clear();

        TOTAL_TEXT.text = "";
    }

    //Helper function that assigns values using an index of the array.
    //Note: Possible items is a string array, every group of three is one object.
    //      The order is, type, price, prefab path.
    //      i.e. {Apple, 9, Prefabs/Apple, Orange, 10, Prefabs/Orange}.
    void SetElement(int index, string type, string price_per, int count, int earnings)
    {
        COUNT_TEXT_LIST[index].text = count + "";
        TYPE_TEXT_LIST[index].text = type;
        PRICE_PER_TEXT_LIST[index].text = "$" + price_per;
        PRICE_TOTAL_TEXT_LIST[index].text = "= " + earnings;
    }
    
    //Helper function that gives a bool of whether or not the scale has something on it too sell.
    public bool HasContainer()
    {
        if(CONTAINER_LIST.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Function that helps check if the given element and type is in our list
    /// True if in it, false if not.
    /// </summary>
    /// <param name="TYPE"></param>
    /// <returns></returns>
    public bool HasType(string TYPE, List<ELEMENT_INFO> LIST)
    {
        foreach (ELEMENT_INFO INFO in LIST)
        {
            if (INFO.TYPE == TYPE)
            {
                return true;
            }
        }
        return false;
    } 

    /// <summary>
    /// Function that gives us the index of a given type of item.
    /// Returns -1 if it can't find the item.
    /// </summary>
    /// <param name="TYPE"></param>
    /// <returns></returns>
    public int IndexOfType(string TYPE, List<ELEMENT_INFO> LIST)
    {
        foreach(ELEMENT_INFO INFO in LIST)
        {
            if(INFO.TYPE == TYPE)
            {
                return ELEMENT_LIST.IndexOf(INFO);
            }
        }
        return -1;
    }

    /// <summary>
    /// Function that changes ownership of a container.
    /// </summary>
    /// <param name="OWNER"></param>
    public void ChangeOwnership(BaseItem.Owner OWNER)
    {
        foreach(Container CONTAINER in CONTAINER_LIST)
        {
            //If it isn't the same owner type, change it.
            if(CONTAINER.transform.parent.GetComponent<BaseItem>()._OWNER != OWNER)
            {
                CONTAINER.transform.parent.GetComponent<BaseItem>()._OWNER = OWNER;
            }
        }
    }

    /// <summary>
    /// Helper function that returns the price of everything under the given owner
    /// </summary>
    /// <param name="OWNER"></param>
    public int GetTotal(BaseItem.Owner OWNER)
    {
        //Total, the return variable
        int total = 0;

        //Temp list that we will do our calculations in
        List<ELEMENT_INFO> TEMP_LIST = new List<ELEMENT_INFO>();

        //For every containre in our list
        foreach (Container CONTAINER in CONTAINER_LIST)
        {
            //If it is the owner type we want
            if (CONTAINER.transform.parent.GetComponent<BaseItem>()._OWNER == OWNER)
            {
                //Add the container into the list
                UpdateContainer(CONTAINER, TEMP_LIST);
            }
        }

        //Sum up the totals
        foreach(ELEMENT_INFO INFO in TEMP_LIST)
        {
            total += INFO.TOTAL_PRICE;
        }

        return total;
    }

    /// <summary>
    /// Helper function that removes all items from containers owned by the given owner.
    /// </summary>
    /// <param name="OWNER"></param>  
    public void EmptyContainers(BaseItem.Owner OWNER)
    {
        foreach(Container CONTAINER in CONTAINER_LIST)
        {
            if(CONTAINER.transform.parent.GetComponent<BaseItem>()._OWNER == OWNER)
            {
                CONTAINER.EmptyContainer();
                ClearList();
            }
        }
    }


    //Helper function that empties all containers on the sensor, and all the lists as well.
    public void EmptyContainers()
    {
        foreach (Container container in CONTAINER_LIST)
        {
            container.EmptyContainer();
            ClearList();
        }
    }

    //Helper function that loads in items from the relevant containers into the delivery system.
    public void SendToDelivery(BaseItem.Owner OWNER)
    {
        //For every container under this owner
        foreach(Container CONTAINER in CONTAINER_LIST)
        {
            //If it is the same owner
            if(CONTAINER.transform.parent.GetComponent<BaseItem>()._OWNER == OWNER)
            {
                //For all container items
                for(int i = 0; i < CONTAINER.numberOfIndItems.Length; i++)
                {
                    //If there are any of this item to worry about
                    if(CONTAINER.numberOfIndItems[i] > 0)
                    {
                        //For all instances of the current possible item
                        for(int j = 0; j < CONTAINER.numberOfIndItems[i]; j++)
                        {
                            //Add one of that item to the delivery manager.
                            //DeliveryManager.Instance.AddItem(CONTAINER.possibleItems[i]);
                        }  
                    }
                }
            }
        }
        EmptyContainers();

    }

}


//Custom class that will help us keep track of multiple things of a type of element in a given collection
//
public class ELEMENT_INFO
{
    //The name of the element
    public string TYPE;

    //How many of the given element is in this container
    public int COUNT;

    //The price per of this element
    public int PRICE_PER;

    //Total price for all of the objects of this element
    public int TOTAL_PRICE;

    //Intialization function
    public ELEMENT_INFO(string TYPE_T, int COUNT_T, int PRICE_PER_T)
    {
        TYPE = TYPE_T;
        COUNT = COUNT_T;
        PRICE_PER = PRICE_PER_T;
        TOTAL_PRICE = COUNT * PRICE_PER;
    }

}