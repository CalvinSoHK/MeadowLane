using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolItem : BaseItem {

    //Type of possible tools
    public enum ToolType { Hoe, Axe, Pickaxe, Sickle};

    //Type of the current tool
    public ToolType _TYPE = ToolType.Hoe;

    //Whether or not the prereq for using the tool has been filled.
    public bool isValid = false;

    //The function that checks for valid actions
    public virtual bool isToolValid()
    {
        bool returnBool = false;
        Debug.Log(gameObject.name + ": has no isToolValid function. Defaulted to false.");
        return returnBool;
    }

    //Function that resets isValid. Called when an action is used
    public virtual void ResetValid()
    {
        isValid = false;
    }

    //Function called to apply something to a gameobject
    public virtual void ApplyTool(GameObject obj)
    {

    }
}
