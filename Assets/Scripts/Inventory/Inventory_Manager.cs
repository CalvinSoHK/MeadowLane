using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  Inventory_Manager {

    //Variables specifically usedd for the regular player inventory
    public static List<string> Category = new List<string>(new string[] { "Produce", "Tools", "Deco", "Gifts", "KeyItems", "Misc" }); //all categories
    public static List<InventorySlot>[] CategorySlots = new List<InventorySlot>[6]; //all items within each category
    public static int currentCategoryIndex = 0, currentCategorySlotsIndex = 0; //current index for the diplay of category and item.
    public static Dictionary<int, GameObject> InventoryItemInScene = new Dictionary<int, GameObject>(); //referene to all inventory items in scenes
    public static Dictionary<int, int> InventorySeedCount = new Dictionary<int, int>(); //reference the seeds for individual seed-boxes currently in inventory

    //Variables used for the furniture inventory
    public static List<string> FurnitureCategory = new List<string>(new string[] { "Chair", "Table", "Storage", "Utility", "Floor", "Electronic", "Plants", "Trophies", "Mounted", "Misc" }); //all furniture categories
    public static List<InventorySlot>[] FurnitureCategorySlots = new List<InventorySlot>[10]; //all furniture items within each category
    public static int currentFurnitureCategoryIndex = 0, currentFurnitureCategorySlotsIndex = 0; //current index for the diplay of furniture category and item.

    /// <summary>
    /// Init the category slots in the player inventory 
    /// </summary>
    public static void InitPlayerInventory()
    {
        for(int i = 0; i < Category.Count; i++)
        {
            CategorySlots[i] = new List<InventorySlot>();
        }
        for(int i = 0; i < FurnitureCategory.Count; i++)
        {
            FurnitureCategorySlots[i] = new List<InventorySlot>();
        }
    }

    /// <summary>
    /// Init the furniture category slots in the player inventory
    /// </summary>
    public static void InitPlayerFurnitureInventory()
    {
        for(int i = 0; i < FurnitureCategory.Count; i++)
        {
            FurnitureCategorySlots[i] = new List<InventorySlot>();
        }
    }

    //Load inventory from save data
    public static void LoadPlayerInventory(string DATA)
    {
        //Debug.Log(DATA);
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
                AddItemToInventory(BASE, Category, CategorySlots);

                //If it is a container,change the seed count
                if (BASE.hasTag(BaseItem.ItemTags.Container))
                {
                    InventorySeedCount[BASE.KEY] = int.Parse(TEMP[1]);

                }   //If it is a produce, change the totalNum
                else if(BASE.hasTag(BaseItem.ItemTags.Produce))
                {
                    CategorySlots[0][checkItemInvetorySlot(BASE.KEY, 0, CategorySlots)].TotalNum = int.Parse(TEMP[1]);
                }
            }
            else
            {
                //For everything else, just add the item to inventory
                //Debug.Log(TEMP[0]);
                OBJ = Resources.Load(TEMP[0], typeof(GameObject)) as GameObject;
                //Debug.Log(OBJ);
                AddItemToInventory(OBJ.GetComponent<BaseItem>(), Category, CategorySlots);
            }
        }

      

        //do stuff with the textFile here...
        /*for(int i = 0; i < Category.Count; i++)
        {
            CategorySlots[i] = new List<InventorySlot>();
        }*/
    }

    /// <summary>
    /// save the player inventory
    /// </summary>
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
                else if(i == 1) //we are looking at the tools
                {
                    if (InventorySeedCount.ContainsKey(CategorySlots[i][j].Key)) //if this is a container
                    {
                        DATA += CategorySlots[i][j].Category + "/" + CategorySlots[i][j].Name + " " + InventorySeedCount[CategorySlots[i][j].Key] + "\n"; //add the seed cound to the data needed to be saved
                    }
                    else //if it is just a regular tool
                    {
                        DATA += CategorySlots[i][j].Category + "/" + CategorySlots[i][j].Name + "\n";  //add the tool info to the data to be saved 
                    }     
                }
                else //every other category item
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
     public static void AddItemToInventory(BaseItem itemInfo, List<string> currentCategories, List<InventorySlot>[] currentInventory)
    {
        //CategorySlots[0] = new List<InventorySlot>();
       // Debug.Log(CategorySlots[0]);
        int categoryIndex = checkItemCategoryIndex(itemInfo.CATEGORY.Trim(), currentCategories); //get the index of the category for which the item will be placed in
        int inventorySlotIndex = checkItemInvetorySlot(itemInfo.KEY, categoryIndex, currentInventory); //get the index of the item within the category list (if it is already there)
        if(inventorySlotIndex == -1) //this item is not in the inventory yet
        {
            
            if (itemInfo.hasTag(BaseItem.ItemTags.Container)) //check if the item added to inventory is a container
            {
                //Debug.Log("Number of seeds added to inv: " + itemInfo.gameObject.GetComponent<PourObject>().COUNT);
                InventorySeedCount.Add(itemInfo.KEY, itemInfo.gameObject.GetComponent<PourObject>().COUNT); //add the seeds to the key value (dictionary)
            }
            //Get the path for the icon of this item
            Sprite ICON = null;
            Sprite CAT_ICON = null;
            if (itemInfo.hasTag(BaseItem.ItemTags.Decoration))
            {
                ICON = Resources.Load("Deco/" + itemInfo.CATEGORY + "/" + itemInfo._NAME, typeof(Sprite)) as Sprite;
                
            }else
            {
                ICON = Resources.Load(itemInfo.CATEGORY + "/" + itemInfo._NAME, typeof(Sprite)) as Sprite;                
            }
            CAT_ICON = Resources.Load("CategoryIcons/" + itemInfo.CATEGORY, typeof(Sprite)) as Sprite;

            //add the item in the category at the end of the list
            currentInventory[categoryIndex].Add(new InventorySlot(itemInfo._NAME, itemInfo.CATEGORY, itemInfo.KEY, ICON, CAT_ICON));
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
                currentInventory[categoryIndex][inventorySlotIndex].TotalNum += 1;
            }
            else
            {
                if (itemInfo.hasTag(BaseItem.ItemTags.Container)) //check if the item added to inventory is a container
                {
                    InventorySeedCount[itemInfo.KEY] += itemInfo.gameObject.GetComponent<PourObject>().COUNT; //add the seeds to the key value (dictionary)
                    return;
                }
                //increase the total number of specific item in inventory by 1
                currentInventory[categoryIndex][inventorySlotIndex].TotalNum += 1;
            }
            
        }
    }

    /// <summary>
    /// After item is spawned from inventory, removes that item instance from inventory if it is produce
    /// </summary>
    /// <param name="itemInfo"></param>
    public static void RemoveItemFromInventory(InventorySlot itemInfo, List<InventorySlot>[] currentInventory, BaseItem currentItem)
    {
        itemInfo.TotalNum -= 1; //reduce the total number of that slot object
        if((currentItem.hasTag(BaseItem.ItemTags.Decoration) || currentCategoryIndex == 0)&& itemInfo.TotalNum == 0) //check if that specific object belongs to the produce category or if its furniture and whether there are none left.
        {
            currentInventory[currentCategoryIndex].Remove(itemInfo); //remove that item from the list
        }
    }

    /// <summary>
    /// Removes all items from the given category slot
    /// </summary>
    /// <param name="INDEX"></param>
    public static void RemoveAllItemsFromCategory(int INDEX, List<InventorySlot>[] currentInventory)
    {
        currentInventory[INDEX].Clear();
    }

    /// <summary>
    /// Adds all items from a list of base items back into the inventory
    /// </summary>
    /// <param name="LIST"></param>
    public static void AddAllItemsFromList(List<BaseItem> LIST, List<string> currentCategories, List<InventorySlot>[] currentInventory)
    {
        foreach(BaseItem ITEM in LIST)
        {
            AddItemToInventory(ITEM, currentCategories, currentInventory);
        }
    }

    /// <summary>
    /// finds the index of the specific category within the static category list
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static int checkItemCategoryIndex(string category, List<string> categoryArray)
    {
        for(int i = 0; i < categoryArray.Count; i++)
        {
            if (category.Equals(categoryArray[i]))
            {
                return i;
            }
        }
        Debug.Log("There was an error finding the category: " + category + " within the inventory category array. Please check the spelling");
        return -1;
    }


    /// <summary>
    /// Checks if item that needs to be added to inventory already has a reference within the list.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int checkItemInvetorySlot(int key, int index, List<InventorySlot>[] currentInventory)
    {
        for (int i = 0; i < currentInventory[index].Count; i++)
        {
            if (key == currentInventory[index][i].Key)
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
    public static List<InventorySlot> GetCategory(string CATEGORY, List<string> categoryArray, List<InventorySlot>[] currentInventory)
    {
        //Get index of the category given
        int INDEX = checkItemCategoryIndex(CATEGORY, categoryArray);

        //Check if there is at least something in the category
        if(INDEX  != -1 && currentInventory[INDEX].Count > 0)
        {
            return currentInventory[INDEX];
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
    /// <summary>
    /// returns the current inventory we are dealing with
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <returns></returns>
    public static List<InventorySlot>[] getCurrentInventory(PlayerInventory.InventoryState currentInvState)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                return CategorySlots;
                //break;
            case PlayerInventory.InventoryState.Furniture:
                return FurnitureCategorySlots;
                //break;
        }
        Debug.Log("ERROR: Are we trying to access an inventory type that does not exist?");
        return null;
    }
    /// <summary>
    /// returns the current category index with the relevant inventory
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <returns></returns>
    public static int getCurrentCategoryIndex(PlayerInventory.InventoryState currentInvState)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                return currentCategoryIndex;
            //break;
            case PlayerInventory.InventoryState.Furniture:
                return currentFurnitureCategoryIndex;
                //break;
        }
        Debug.Log("ERROR: Are we trying to access an inventory type that does not exist?");
        return -1;
    }
    /// <summary>
    /// returns the current slot index within the relevant category and inventory
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <returns></returns>
    public  static  int getCurrentCategorySlotIndex(PlayerInventory.InventoryState currentInvState)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                return currentCategorySlotsIndex;
            //break;
            case PlayerInventory.InventoryState.Furniture:
                return currentFurnitureCategorySlotsIndex;
                //break;
        }
        Debug.Log("ERROR: Are we trying to access an inventory type that does not exist?");
        return -1;
    }

    /// <summary>
    /// adds to the category index to the relevant inventory based on given value
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <param name="value"></param>
    public static void changeIndex(PlayerInventory.InventoryState currentInvState, int value)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                currentCategoryIndex += value;
                break;
            case PlayerInventory.InventoryState.Furniture:
                currentFurnitureCategoryIndex += value;
                break;
        }

    }

    /// <summary>
    /// adds to the slot index to the relevant category and inventory based on given value
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <param name="value"></param>
    public static void changeIndexSlots(PlayerInventory.InventoryState currentInvState, int value)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                currentCategorySlotsIndex += value;
                break;
            case PlayerInventory.InventoryState.Furniture:
                currentFurnitureCategorySlotsIndex += value;
                break;
        }
              
    }

    /// <summary>
    /// update the category index to the relevant inventory based on given value
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <param name="value"></param>
    public static void setIndex(PlayerInventory.InventoryState currentInvState, int value)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                currentCategoryIndex = value;
                break;
            case PlayerInventory.InventoryState.Furniture:
                currentFurnitureCategoryIndex = value;
                break;
        }

    }

    /// <summary>
    /// update the slot index to the relevant ategory and inventory based on given value
    /// </summary>
    /// <param name="currentInvState"></param>
    /// <param name="value"></param>
    public static void setIndexSlots(PlayerInventory.InventoryState currentInvState, int value)
    {
        switch (currentInvState)
        {
            case PlayerInventory.InventoryState.Item:
                currentCategorySlotsIndex = value;
                break;
            case PlayerInventory.InventoryState.Furniture:
                currentFurnitureCategorySlotsIndex = value;
                break;
        }

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
