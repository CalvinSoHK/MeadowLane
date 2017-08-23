﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Info for a single farm block.
public class FarmBlockInfo : MonoBehaviour {

    //The plant in the farm block
    //If null, no plant.
    public Transform PLANT;

    //States of the block
    public bool WATERED = false, TILLED = false, INFERTILE = false;

    //Coordinate of the block in this plot
    public Vector2 coordinate;

	// Use this for initialization
	void Start () {
        //Parse coordinate from the name.
        string name = transform.name;

        int x = int.Parse(name.Substring(1, 1));
        int y = int.Parse(name.Substring(3, 1));

        coordinate = new Vector2(x, y);
	}

    void Update()
    {
        //Modify appearance of the earth if it gets any bools changed.
        if (TILLED)
        {
            GetComponent<Renderer>().material = Resources.Load("Materials/FarmBlock/Dirt_Tilled", typeof(Material)) as Material;
        }
    }

    //Check on collision for possible bool changes.
    private void OnCollisionEnter(Collision collision)
    {
        //The obj that collided with us.
        GameObject obj = collision.collider.gameObject;
        //Debug.Log(obj.name);
        //If the collided object has an infolinker...
        if(obj.GetComponent<InfoLinker>() != null)
        {
            //Check info for tool type
            Transform mainObj = obj.GetComponent<InfoLinker>().infoObj;

            //If a hoe, till the earth.
            if(mainObj.GetComponent<ToolItem>()._TYPE == ToolItem.ToolType.Hoe &&
                mainObj.GetComponent<ToolItem>().isValid)
            {
                TILLED = true;
            }
        }
    }

    //Check on collision for possible bool changes.
    private void OnCollisionStay(Collision collision)
    {
        //The obj that collided with us.
        GameObject obj = collision.collider.gameObject;
        //Debug.Log(obj.name);
        //If the collided object has an infolinker...
        if (obj.GetComponent<InfoLinker>() != null)
        {
            //Check info for tool type
            Transform mainObj = obj.GetComponent<InfoLinker>().infoObj;

            //If a hoe, till the earth.
            if (mainObj.GetComponent<ToolItem>()._TYPE == ToolItem.ToolType.Hoe &&
                mainObj.GetComponent<ToolItem>().isValid)
            {
                TILLED = true;
            }
        }
    }
}
