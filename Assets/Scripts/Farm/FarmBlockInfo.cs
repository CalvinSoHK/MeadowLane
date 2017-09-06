using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Info for a single farm block.
public class FarmBlockInfo : MonoBehaviour {

    //The plant in the farm block
    //If null, no plant.
    public Transform PLANT;

    //States of the block
    public bool WATERED = false, TILLED = false, FERTILE = false;

    //how much the block has been watered
    public float waterCount = 0;
    public float waterMax = 100;

    //Coordinate of the block in this plot
    public Vector2 coordinate;

    //Starting farm block color
    public Color originalFarmBlockColor,currentFarmBlockColor;

	// Use this for initialization
	void Start () {
        //keep record of original color
        originalFarmBlockColor = GetComponent<Renderer>().material.color;
        currentFarmBlockColor = originalFarmBlockColor;
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
            currentFarmBlockColor = GetComponent<Renderer>().material.color;
        }
        if(waterCount >= waterMax)
        {
            WATERED = true;
        }else
        {
            //Debug.Log("Is water count lower than max");
            GetComponent<Renderer>().material.color = Color.Lerp(currentFarmBlockColor, currentFarmBlockColor * (10/100), waterCount / 100);
        }
        if (WATERED)
        {
            //Debug.Log("Watered");
            GetComponent<Renderer>().material.color = originalFarmBlockColor * (10/100);
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
                if (!TILLED)
                {
                    mainObj.GetComponent<AudioSource>().Play();
                    TILLED = true;
                }
            }
        }
    }

    /*
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
    }*/

    //Day end function for farmBlock
    public void DayEnd()
    {
        if(PLANT != null)
        {
            PLANT.GetComponent<PlantBase>().DayEnd();
        }

        waterCount = 0;
        WATERED = false;

    }
}
