using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  Inventory_Manager {

    static TextAsset playerInventorySave; //Text Asset Loaded and Saved to know what the player's inventory is between scenes/loads
    public static List<string> Category = new List<string>(new string[] { "Produce", "Tools", "Deco", "Gifts", "KeyItems", "Misc" }); //all categories
    public static List<InventorySlot>[] CategorySlots = new List<InventorySlot>[6]; //all items within each category
    public static int currentCategoryIndex = 0, currentCategorySlotsIndex = 0; //current index for the diplay of category and item.
    public static Dictionary<int, GameObject> InventoryItemInScene = new Dictionary<int, GameObject>(); //referene to all inventory items in scenes

    public static void LoadPlayerInventory()
    {
        //do stuff with the textFile here...
        for(int i = 0; i < Category.Count; i++)
        {
            CategorySlots[i] = new List<InventorySlot>();
        }
    }
    public static void SavePlayerInventory()
    {
        //do stuff with the player inventory here.
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
            //add the item in the category at the end of the list
            CategorySlots[catergoryIndex].Add(new InventorySlot(itemInfo._NAME, itemInfo.CATEGORY, itemInfo.PREFAB_REF, itemInfo.KEY, itemInfo.ICON, itemInfo.CATEGORY_ICON));
        }else //item type is already in inventory
        {
            //increase the total number of specific item in inventory by 1
            CategorySlots[catergoryIndex][inventorySlotIndex].TotalNum += 1;
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
