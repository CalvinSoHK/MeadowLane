using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  Inventory_Manager {

    static TextAsset playerInventorySave; //Text Asset Loaded and Saved to know what the player's inventory is between scenes/loads
    public static List<string> Category = new List<string>(new string[] { "Produce", "Tools", "Deco", "Gifts", "KeyItems", "Misc" }); //all categories
    public static List<InventorySlot>[] CategorySlots = new List<InventorySlot>[6]; //all items within each category
    public static int currentCategoryIndex = 0, currentCategorySlotsIndex = 0; //current index for the diplay of category and item.
    public static Dictionary<int, GameObject> InventoryItemInScene = new Dictionary<int, GameObject>(); //referene to all inventory items in scenes
    public static Dictionary<int, int> InventorySeedCount = new Dictionary<int, int>(); //reference the seeds for individual seed-boxes currently in inventory

    //Init the slots
    public static void InitPlayerInventory()
    {
        for(int i = 0; i < Category.Count; i++)
        {
            CategorySlots[i] = new List<InventorySlot>();
        }
    }

    //Load inventory from save data
    public static void LoadPlayerInventory(string DATA)
    {
        Debug.Log(DATA);
        string[] INPUT = DATA.Split('\n'), TEMP;
        GameObject OBJ;

        //Read each line from input
        //Length is less one because the less index in the array is blank since every line ends in \n.
        for (int i = 0; i < INPUT.Length - 1; i++)
        {
            //Split each line by spaces
            TEMP = INPUT[i].Split(' ');
            //If we have a length greater than one than we are a dispenser
            if (TEMP.Length > 1)
            {
                //Load the object. Add it to inventory, and adjust values afterwards.
                OBJ = Resources.Load(TEMP[0], typeof(GameObject)) as GameObject;
                BaseItem BASE = OBJ.GetComponent<BaseItem>();
                AddItemToInventory(BASE);

                //If it is a container,change the seed count
                if (BASE.hasTag(BaseItem.ItemTags.Container))
                {
                    InventorySeedCount[BASE.KEY] = int.Parse(TEMP[1]);

                }   //If it is a produce, change the totalNum
                else if(BASE.hasTag(BaseItem.ItemTags.Produce))
                {
                    CategorySlots[0][checkItemInvetorySlot(BASE.KEY, 0)].TotalNum = int.Parse(TEMP[1]);
                }
            }
            else
            {
                //For everything else, just add the item to inventory
                Debug.Log(TEMP[0]);
                OBJ = Resources.Load(TEMP[0], typeof(GameObject)) as GameObject;
                Debug.Log(OBJ);
                AddItemToInventory(OBJ.GetComponent<BaseItem>());
            }
        }

      

        //do stuff with the textFile here...
        /*for(int i = 0; i < Category.Count; i++)
        {
            CategorySlots[i] = new List<InventorySlot>();
        }*/
    }

    public static void SavePlayerInventory()
    {
        //Save string
        string DATA = "";

        //do stuff with the player inventory here.
        //Go through each category
        for(int i = 0; i < CategorySlots.Length; i++)
        { 
            //Go through all items in that category
            for(int j = 0; j < CategorySlots[i].Count; j++)
            {
                //Produce, save count as well
                if(i == 0)
                {
                    DATA += CategorySlots[i][j].Category + "/" + CategorySlots[i][j].Name + " " + CategorySlots[i][j].TotalNum + "\n";
                }
                else if(i == 1)
                {
                    if (InventorySeedCount.ContainsKey(CategorySlots[i][j].Key))
                    {
                        DATA += CategorySlots[i][j].Category + "/" + CategorySlots[i][j].Name + " " + InventorySeedCount[CategorySlots[i][j].Key] + "\n";
                    }
                    else
                    {
                        DATA += CategorySlots[i][j].Category + "/" + CategorySlots[i][j].Name + "\n";
                    }     
                }
                else
                {
                    DATA += CategorySlots[i][j].Category + "/" + CategorySlots[i][j].Name + "\n";
                }
            }
        }

        //Save to the arrays
        SaveSystem.SaveTo(SaveSystem.SaveType.Inventory, "/Inventory\n" + DATA + "/");
    }

    /// <summary>
    /// Adds an item to the player's inventory
    /// </summary>
    /// <param name="itemInfo"></param>
     public static void AddItemToInventory(BaseItem itemInfo)
    {
        //CategorySlots[0] = new List<InventorySlot>();
       // Debug.Log(CategorySlots[0]);
        int catergoryIndex = checkItemCategoryIndex(itemInfo.CATEGORY.Trim()); //get the index of the category for which the item will be placed in
        int inventorySlotIndex = checkItemInvetorySlot(itemInfo.KEY, catergoryIndex); //get the index of the item within the category list (if it is already there)
        if(inventorySlotIndex == -1) //this item is not in the inventory yet
        {
            
            if (itemInfo.hasTag(BaseItem.ItemTags.Container)) //check if the item added to inventory is a container
            {
                //Debug.Log("Number of seeds added to inv: " + itemInfo.gameObject.GetComponent<PourObject>().COUNT);
                InventorySeedCount.Add(itemInfo.KEY, itemInfo.gameObject.GetComponent<PourObject>().COUNT); //add the seeds to the key value (dictionary)
            }
            //Get the path for the icon of this item
            Sprite ICON = Resources.Load(itemInfo.CATEGORY + "/" + itemInfo._NAME, typeof(Sprite)) as Sprite;
            Sprite CAT_ICON = Resources.Load("CategoryIcons/" + itemInfo.CATEGORY, typeof(Sprite)) as Sprite;

            //add the item in the category at the end of the list
            CategorySlots[catergoryIndex].Add(new InventorySlot(itemInfo._NAME, itemInfo.CATEGORY, itemInfo.KEY, ICON, CAT_ICON));
        }else //item type is already in inventory
        {
            //check if the object added already existed in iventory, but was taken out by the player (does not apply for produce)
            if (InventoryItemInScene.ContainsKey(itemInfo.KEY))
            {
                if (itemInfo.hasTag(BaseItem.ItemTags.Container)) //check if the item added to inventory is a container
                {
                    InventorySeedCount[itemInfo.KEY] += itemInfo.gameObject.GetComponent<PourObject>().COUNT; //add the seeds to the key value (dictionary)
                }
                InventoryItemInScene.Remove(itemInfo.KEY); //remove that item from the dictionary
                CategorySlots[catergoryIndex][inventorySlotIndex].TotalNum += 1;
            }
            else
            {
                if (itemInfo.hasTag(BaseItem.ItemTags.Container)) //check if the item added to inventory is a container
                {
                    InventorySeedCount[itemInfo.KEY] += itemInfo.gameObject.GetComponent<PourObject>().COUNT; //add the seeds to the key value (dictionary)
                    return;
                }
                //increase the total number of specific item in inventory by 1
                CategorySlots[catergoryIndex][inventorySlotIndex].TotalNum += 1;
            }
            
        }
    }

    /// <summary>
    /// After item is spawned from inventory, removes that item instance from inventory if it is produce
    /// </summary>
    /// <param name="itemInfo"></param>
    public static void RemoveItemFromInventory(InventorySlot itemInfo)
    {
        itemInfo.TotalNum -= 1; //reduce the total number of that slot object
        if(currentCategoryIndex == 0 && itemInfo.TotalNum == 0) //check if that specific object belongs to the produce category and whether there are none left.
        {
            CategorySlots[currentCategoryIndex].Remove(itemInfo); //remove that item from the list
        }
    }

    /// <summary>
    /// Removes all items from the given category slot
    /// </summary>
    /// <param name="INDEX"></param>
    public static void RemoveAllItemsFromCategory(int INDEX)
    {
        CategorySlots[INDEX].Clear();
    }

    /// <summary>
    /// Adds all items from a list of base items back into the inventory
    /// </summary>
    /// <param name="LIST"></param>
    public static void AddAllItemsFromList(List<BaseItem> LIST)
    {
        foreach(BaseItem ITEM in LIST)
        {
            AddItemToInventory(ITEM);
        }
    }

    /// <summary>
    /// finds the index of the specific category within the static category list
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static int checkItemCategoryIndex(string category)
    {
        for(int i = 0; i < Category.Count; i++)
        {
            if (category.Equals(Category[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Checks if item that needs to be added to inventory already has a reference within the list.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int checkItemInvetorySlot(int key, int index)
    {
        for (int i = 0; i < CategorySlots[index].Count; i++)
        {
            if (key == CategorySlots[index][i].Key)
            {
                
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// returns the item which has already been spawned from the player's inventory to connect it back to the player's hand
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static GameObject MoveItemToHandOfPlayer(int key)
    {
        return InventoryItemInScene[key];
    }

    public static void RemoveInventoryItemsFromScenes()
    {
        //get all the keys from the ditionanary
        //use a for loop to go through that list
        //add the objects back into the inventory
        //destroy the object from the scene
    }
    public static void AddItemToDictionary(int key, GameObject value)
    {
        InventoryItemInScene.Add(key, value);
    }

    /// <summary>
    /// Function that gives us the entire inventory information of a given category
    /// </summary>
    /// <param name="CATEGORY_INDEX"></param>
    /// <returns></returns>
    public static List<InventorySlot> GetCategory(string CATEGORY)
    {
        //Get index of the category given
        int INDEX = checkItemCategoryIndex(CATEGORY);

        //Check if there is at least something in the category
        if(INDEX  != -1 && CategorySlots[INDEX].Count > 0)
        {
            return CategorySlots[INDEX];
        }
        return null;
    }

    /// <summary>
    /// getst he amount of seeds for that specific container.
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <returns></returns>
    public static int getSeeds(BaseItem itemInfo)
    {
        int temp = InventorySeedCount[itemInfo.KEY];
        InventorySeedCount[itemInfo.KEY] = 0;
        return temp;
    }
    public static int getSeeds(int key)
    {
        
        return InventorySeedCount[key];
    }
}

static class ListExtensions
{
    static void MoveItemToFrontOfList<T>(this List<T> list, int index)
    {
        T item = list[index];
        list.RemoveAt(index);
        list.Insert(0, item);
    }
}
