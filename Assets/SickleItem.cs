using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hoe item script.
public class SickleItem: ToolItem
{
    //Minimum blade velocity to be valid.
    public float maxVelocity = 1;

    //The blade component
    public GameObject BLADE;

    //Velocity of the blade, where the hoe linker is attached.
    public float velocity;

    //Previous position
    private Vector3 previous;

    //Timer for checks
    private float timer;

    //Time interval checks
    public float intervals = 0.25f;

    private void Update()
    {
        if (GetComponent<InteractionPickup>() != null)
        {
            //Tell the linker to calculate is valid, then pass it up.
            if (GetComponent<InteractionPickup>().isHeld)
            {
                isValid = isToolValid();
            }
            else
            {
                isValid = false;
            }
        }

    }

    //Write new isToolValid function
    public override bool isToolValid()
    {
        if (timer < 0)
        {
            velocity = Vector3.Distance(new Vector3(BLADE.transform.position.x, 0, BLADE.transform.position.z), previous) * 100f;
            previous = new Vector3(BLADE.transform.position.x, 0, BLADE.transform.position.z);

            timer = intervals;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (velocity >= maxVelocity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Till the dirt
    public override void ApplyTool(GameObject obj)
    {
        //If the tool is valid.
        if (isValid)
        {
            if (obj.GetComponent<FarmBlockInfo>() != null)
            {
                if (obj.GetComponent<FarmBlockInfo>().PLANT != null)
                {
                    //Destroy(obj.GetComponent<FarmBlockInfo>().PLANT.gameObject);
                    //obj.GetComponent<FarmBlockInfo>().PLANT = null;
                    //obj.GetComponent<FarmBlockInfo>().TILLED = false;

                }
            }
            else if(obj.GetComponent<PlantBase>() != null)
            {
                obj.transform.parent.GetComponent<FarmBlockInfo>().DEAD = false;
                obj.transform.parent.GetComponent<FarmBlockInfo>().PLANT = null;
                obj.transform.parent.GetComponent<FarmBlockInfo>().TILLED = false;
                Destroy(obj);
            }
        }
    }


}
