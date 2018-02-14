using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shop UI entry manager.
public class ShopEntryManager : BasicEntryManager {

    //Whether or not we want the items to be delivered next day or given immediately.
    public bool IS_DELIVERY;

    public PaymentBasketManager BASKET;

    public GameObject ENTRY_PREFAB;
    GameObject TEMP;

    public GameObject TOTAL_OBJ;

    DeliveryManager DM;

    private void Awake()
    {
        DM = GameManagerPointer.Instance.DELIVERY_MANAGER;
        TOTAL_OBJ.GetComponent<SingleEntryManager>().NAME = "Total";
    }

    // Update is called once per frame
    void Update () {
        //Maintain total
      
        TOTAL_OBJ.GetComponent<SingleEntryManager>().PRICE_TEXT.text = " " + CalculateTotal();
	}


    /// <summary>
    /// Delivers all items in our basket into the delivery service.
    /// </summary>
    public void DeliverItems()
    {
        //For all entries
        foreach(GameObject OBJ in BASKET.OBJECT_LIST)
        {
           if (IS_DELIVERY)
           {
                    //If delivery for tomorrow
                if(DM == null)
                {
                    DM = GameManagerPointer.Instance.DELIVERY_MANAGER;
                }
                DM.AddItem(OBJ);
           }
           else
           {
                    //else add it now
                Inventory_Manager.AddItemToInventory(OBJ.GetComponent<BaseItem>());
           }   
        }
        for(int i = ENTRY_LIST.Count - 1; i >= 0; i--)
        {
            Destroy(ENTRY_LIST[i].gameObject);
        }
        ENTRY_LIST.Clear();
    }

    /// <summary>
    /// Calculates the total value of items in our cart.
    /// </summary>
    /// <returns></returns>
    public override int CalculateTotal()
    {
        int TOTAL = 0;
        foreach (SingleEntryManager ENTRY in ENTRY_LIST)
        {
            TOTAL += ENTRY.COUNT * ENTRY.PRICE;
        }
        return TOTAL;
    }

    /// <summary>
    /// Helper function that checks to see if the obj is already in the ENTRY_LIST
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool HasEntry(BaseItem obj)
    {
        foreach(SingleEntryManager ENTRY in ENTRY_LIST)
        {
            if(ENTRY.NAME.Trim().Equals(obj._NAME.Replace("_", " ").Trim()))
            {
                if (!obj.hasTag(BaseItem.ItemTags.Container))
                {
                    return true;
                }
                else if(obj.GetComponent<PourObject>().COUNT == ENTRY.CONTAINER_COUNT)
                {
                    return true;
                }
               
            }
        }

        return false;
    }

    /// <summary>
    /// Helper function that returns the index of an obj in the ENTRY_LIST
    /// Returns -1 if it can't find it.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetIndexOf(BaseItem obj)
    {
        foreach (SingleEntryManager ENTRY in ENTRY_LIST)
        {
            if (ENTRY.NAME.Equals(obj._NAME.Replace("_", " ")))
            {
                if (!obj.hasTag(BaseItem.ItemTags.Container))
                {
                    return ENTRY_LIST.IndexOf(ENTRY);
                }
                else if (obj.GetComponent<PourObject>().COUNT == ENTRY.CONTAINER_COUNT)
                {
                    return ENTRY_LIST.IndexOf(ENTRY);
                }

            }
        }
        return -1;
    }

    /// <summary>
    /// Helper function that adds an entry to the list if the obj isn't already in the ENTRY_LIST
    /// If it is already in the list, it increments that entry's count by 1.
    /// </summary>
    /// <param name="obj"></param>
    public void AddItem(BaseItem obj)
    {
        if (HasEntry(obj))
        {
            ENTRY_LIST[GetIndexOf(obj)].COUNT++;
        }
        else
        {
            //Filter the obj name so it doesnt have underscores
            string NAME_INPUT = obj._NAME;
            NAME_INPUT = NAME_INPUT.Replace('_', ' ');

            TEMP = Instantiate(ENTRY_PREFAB, transform);
            TEMP.GetComponent<SingleEntryManager>().NAME = NAME_INPUT;
            TEMP.GetComponent<SingleEntryManager>().COUNT = 1;
            TEMP.GetComponent<SingleEntryManager>().PRICE = obj._VALUE;
            if (obj.hasTag(BaseItem.ItemTags.Container))
            {
                TEMP.GetComponent<SingleEntryManager>().CONTAINER_COUNT = obj.GetComponent<PourObject>().COUNT;
            }
            ENTRY_LIST.Add(TEMP.GetComponent<SingleEntryManager>());
        }
    }

    /// <summary>
    /// Helper function that removes an item from the prospective list.
    /// If there is only 1, remove it from the list.
    /// If there is more than 1, decrement COUNT by 1.
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveItem(BaseItem obj)
    {
        //Just check its in there
        if (HasEntry(obj))
        {
            //Get the index.
            int index = GetIndexOf(obj);

            //Decrement count
            ENTRY_LIST[index].COUNT--;

            //IF less than or equal to zero, remove completely.
            if(ENTRY_LIST[index].COUNT <= 0)
            {
                Destroy(ENTRY_LIST[index].gameObject);
                ENTRY_LIST.RemoveAt(index);
            }
        }
        else
        {
            Debug.Log("ERROR: Tried to remove an object that isn't already in the list.");
        }
    }
}

public class Entry
{
    public int COUNT, PRICE;
    public string NAME;
    public string CATEGORY;


    public Entry(int COUNT_T, int PRICE_T, string NAME_T, string CATEGORY_T)
    {
        CATEGORY = CATEGORY_T;
        COUNT = COUNT_T;
        PRICE = PRICE_T;
        NAME = NAME_T;
    }

    public bool Equals(Entry OBJ)
    {
        //Count isn't the same because that's how many of that object we have not how many seeds are in it.
        if(OBJ.NAME.Equals(NAME.Replace('_', ' ')) && OBJ.PRICE == PRICE && OBJ.CATEGORY.Equals(CATEGORY))
        {
            return true;
        }
        return false;
    }

    public bool Equals(BaseItem ITEM)
    {
        if(ITEM._NAME.Replace('_', ' ').Trim().Equals(NAME.Trim()) && ITEM._VALUE == PRICE && ITEM.CATEGORY.Trim().Equals(CATEGORY.Trim()))
        {
            return true;
        }
        //Debug.Log("Item name: " + ITEM._NAME + "    other item name: " + NAME);
        return false;
    }
}
