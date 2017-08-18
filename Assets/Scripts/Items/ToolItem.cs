using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolItem : BaseItem {

    //Type of possible tools
    public enum ToolType { Hoe, Axe, Pickaxe, Scythe };

    //Type of the current tool
    public ToolType _TYPE = ToolType.Hoe;
}
