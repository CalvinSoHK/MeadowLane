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
    public Text TOTAL;

    //Lists that have our textboxes
    List<Text> COUNT_TEXT_LIST = new List<Text>(), TYPE_TEXT_LIST = new List<Text>(), PRICE_PER_TEXT_LIST = new List<Text>(), PRICE_TOTAL_TEXT_LIST = new List<Text>();

    //The container that we are displaying
    public List<Container> CONTAINER_LIST = new List<Container>();

    //A list for each of the elements. These lists represent the consolidated data from multiple containers.
    public List<int> COUNT_LIST = new List<int>(), PRICE_PER_LIST = new List<int>(), PRICE_TOTAL_LIST = new List<int>();
    public List<string> TYPE_LIST = new List<string>();

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
            UpdateLists();
            DisplayInfo();
        }
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

    //Update list values;
    public void UpdateLists()
    {
        //First set the counts to zero for everyone
        for(int i = 0; i < COUNT_LIST.Count; i++)
        {
            COUNT_LIST[i] = 0;
        }

        //For all the containers...
        foreach (Container container in CONTAINER_LIST)
        {
            //For all their elements...
            for (int i = 0; i < container.numberOfIndItems.Length; i++)
            {
                //If the possible item has at least 1 of it.
                if (container.numberOfIndItems[i] != 0)
                {
                    //The data values we need to add
                    string TYPE = container.possibleItems[(i + 1) * 3 - 3];
                    int COUNT = container.numberOfIndItems[i];
                    int PRICE_PER = int.Parse(container.possibleItems[(i + 1) * 3 - 2]);

                    //If we haven't yet added the item to the TYPE_LIST
                    if (!TYPE_LIST.Contains(TYPE))
                    {
                        //"Init" this product in our lists.
                        //Note: Price total is quivalent to our price per times count for now.
                        TYPE_LIST.Add(TYPE);
                        COUNT_LIST.Add(COUNT);
                        PRICE_PER_LIST.Add(PRICE_PER);
                        PRICE_TOTAL_LIST.Add(PRICE_PER * COUNT);
                    }
                    else//It contains the type we're trying to add, we should consolidate the info.
                    {
                        //First, get the index of the product in our list.
                        int index = TYPE_LIST.IndexOf(TYPE);

                        //Then add to the valid lists
                        COUNT_LIST[index] += COUNT;
                        PRICE_TOTAL_LIST[index] = COUNT_LIST[index] * PRICE_PER;
                    }
                }
            }
        }
    }

    //Display all pertinent info
    public void DisplayInfo()
    {
        //As many elements that are in our lists.
        for(int i = 0; i < TYPE_LIST.Count; i++)
        {
            SetElement(i, TYPE_LIST[i], PRICE_PER_LIST[i] + "" , COUNT_LIST[i], PRICE_TOTAL_LIST[i]);
        }
    }

    //Helper function that removes a container from our list and does the necessary work
    void RemoveContainer(Container container)
    {
        Debug.Log("Remove");
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
                int index = TYPE_LIST.IndexOf(TYPE);

                //Update the count. If it's zero, we should remove the entire item from all lists.
                COUNT_LIST[index] -= COUNT;
                if (COUNT_LIST[index] <= 0)
                {
                    TYPE_LIST.RemoveAt(index);
                    COUNT_LIST.RemoveAt(index);
                    PRICE_PER_LIST.RemoveAt(index);
                    PRICE_TOTAL_LIST.RemoveAt(index);
                }
                else //If some other container has that object, don't remove it, just update the total values.
                {
                    PRICE_TOTAL_LIST[index] = COUNT_LIST[index] * PRICE_PER_LIST[index];
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

        TOTAL.text = "";
    }

    //Helper function that assigns values using an index of the array. It will also return the value earned per call
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
}
