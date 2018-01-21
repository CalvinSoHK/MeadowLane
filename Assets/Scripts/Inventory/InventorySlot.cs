using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot {

    private string p_Name; //name of the item
    private string p_Category; // category which the item belongs to
    //(IF WE ARE KEEPING TRACK OF AN IMAGE SHOULD WE KEEP TRACK OF THE ABOVE AS A GAME OBJECT RATHER THAT STRING)
    private int p_TotalNum; //total number of this item in inventory
    private int p_Key; //key representing item
    private Image p_Icon;//itemIcon  
    private Image p_CategoryIcon;   

    /// <summary>
    /// Create a new object of type Inventoty Slot. 
    /// Custom  class meant to represent a type of an item and relevant information
    /// currently located in the player's inventory
    /// </summary>
    /// <param name="name"></param>
    /// <param name="category"></param>
    /// <param name="prefabRef"></param>
    /// <param name="key"></param>
    /// <param name="icon"></param>
    public InventorySlot(string name, string category, int key, Image icon, Image cIcon)
    {
        p_Name = name;
        p_Category = category;
        p_Key = key;
        p_Icon = icon;
        p_CategoryIcon = cIcon;
        p_TotalNum = 1;
    }

    /// <summary>
    /// getter for key value of item located in inventory
    /// </summary>
    public int Key
    {
        get
        {
            return p_Key;
        }
    }

    /// <summary>
    /// getter and setter for the total number of an individual item located in inventory
    /// </summary>
    public int TotalNum
    {
        get
        {
            return p_TotalNum;
        }
        set
        {
            this.p_TotalNum = value;
        }
    }

    public Image Icon
    {
        get
        {
            return p_Icon;
        }
    }

    public Image cIcon
    {
        get
        {
            return p_CategoryIcon;
        }
    }

    public string Category
    {
        get
        {
            return p_Category;
        }
    }

    public string Name
    {
        get
        {
            return p_Name;
        }
    }
}
