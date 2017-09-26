using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Base item class with all the overrideable functions.
public class BaseItem : MonoBehaviour {

    //Name of the item
    public string _NAME = "BaseItem";

    //Monetary value of the item
    public int _VALUE = 0;

    //Tags that the item can have
    // Can apply more than one tag
    [System.Flags]
    public enum ItemTags
    {
        Key = 0x01,
        Farm = 0x02,
        Fishing = 0x04,
        Produce = 0x08,
        AnimalFood = 0x10,
        Decoration = 0x20,
        Tool = 0x40,
        Seed = 0x80,
        Container = 0x100
        //Wearable = 0x200,
        //Money = 0x400,
        //Dateable = 0x800
    }

    [SerializeField]
    [EnumTag]
    public ItemTags _TAGS = 0;

    //Image that is shown in the inventory
    public Image _THUMBNAIL;

    //Offset of item when held by hand
    public Vector3 offset;

    //Rotation offset
    public Vector3 rotOffset;

    //Helper functions below

    //Delete item. Insert any effects you want for all items to have for default deletion.
    public virtual void DeleteItem()
    {
        Debug.Log("Delete item function called. Using defualt from : " + _NAME + " | " + gameObject.name);
        Destroy(gameObject);
    }

    //Display info. Shows a box on top of the item with relevant info.
    public virtual void DisplayInfo()
    {
        Debug.Log("Display info function called. Using default from : " + _NAME + " | " + gameObject.name);
    }

    //Use function. Default does nothing but print the obj's name.
    public virtual void Use()
    {
        Debug.Log("Use function called. Using default from : " + _NAME + " | " + gameObject.name);
    }

    //Helper function to manipulate tags
    public bool hasTag(ItemTags tag)
    {
        return (_TAGS & tag) != 0;
    }

    //Probably won't need to call these from other scripts.
    protected void removeTag(ItemTags tagsToRemove)
    {
        _TAGS = _TAGS & ~(tagsToRemove);
    }

    protected void addTag(ItemTags tagsToAdd)
    {
        _TAGS |= tagsToAdd;
    }
}
