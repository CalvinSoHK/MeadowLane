﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Seed base class.
[RequireComponent(typeof(InteractionPickup))]
public class SeedItem : BaseItem {

    //Prefab reference to the plant that is planted upon contact with tilled earth
    public GameObject plantPrefab;

    //Offset of the plant when planting
    public Vector3 prefabPosOffset;

    //Offset for rotation when planting
    public Vector3 prefabRotOffset;

    public bool isPlanted = false;
    int tempCount = 0;
    //Plant the seed into the given farmBlock
	public virtual void PlantSeed(Transform farmBlock)
    {
        //Calculate rotOffset
        Quaternion qualOffset = Quaternion.Euler(prefabRotOffset);
        Quaternion calcRot = new Quaternion(
            farmBlock.rotation.x + qualOffset.x,
            farmBlock.rotation.y + qualOffset.y,
            farmBlock.rotation.z + qualOffset.z,
            farmBlock.rotation.w + qualOffset.w);
       
        //"Plant" the plant. Apply offsets. Make child of farm block
        GameObject plant = Instantiate(plantPrefab, farmBlock.position + prefabPosOffset, calcRot, farmBlock);
        plant.GetComponent<PlantBase>().Init();

        //Set variables on farmBlock
        farmBlock.GetComponent<FarmBlockInfo>().PLANT = plant.transform;

        //Destroy gameObject after planting the plant
        Destroy(gameObject);
    }

    //OnCollisionEnter, plant seed on contact.
    private void OnCollisionEnter(Collision collision)
    {
        //Retrieve the collided object
        //Debug.Log("on collision enter count: " + tempCount);
        tempCount += 1;
        GameObject target = collision.collider.gameObject;

        //Check if its a farm block
        if (!isPlanted && target.GetComponent<FarmBlockInfo>())
        {
            //Check if the ground has already been tilled
            if (target.GetComponent<FarmBlockInfo>().TILLED && !target.GetComponent<FarmBlockInfo>().HAS_SEED)
            {
                //Use the function to plant the seed there.
                //After this the function destroys the object so nothing comes after this.
                isPlanted = true;
                PlantSeed(target.transform);
                target.GetComponent<FarmBlockInfo>().HAS_SEED = true;
                
            }
            else //If not, sit as a normal object.
            {
                //Debug.Log("Can't plant, not tilled or already has seed." + gameObject.name);
            }
           
        }
    }
}
