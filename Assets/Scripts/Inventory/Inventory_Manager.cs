using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  Inventory_Manager {

    static TextAsset playerInventorySave; //Text Asset Loaded and Saved to know what the player's inventory is between scenes/loads
    public static List<string> Category = new List<string>(new string[] { "Produce", "Tools", "Deco", "Gifts", "KeyItems", "Misc" }); //all categories
    public static List<InventorySlot>[] CategorySlots = new List<InventorySlot>[6]; //all items within each category
    static int currentCategoryIndex = 0, currentCategorySlotsIndex = 0; //current index for the diplay of category and item.

    public static void LoadPlayerInventory()
    {
        //do stuff with the textFile here...
    }
    public static void SavePlayerInventory()
    {
        //do stuff with the player inventory here.
    }

    public static void AddItemToInventory(BaseItem itemInfo)
    {
        Category.FindIndex(itemInfo.)
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
