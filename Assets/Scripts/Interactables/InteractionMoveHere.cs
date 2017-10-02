using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Interaction script that just moves an object here on use.
public class InteractionMoveHere : InteractableCustom {

    //Given object that will be moved here on use
    public GameObject OBJ;

    //The parent we want to use.
    public Transform PARENT;

    //Pos offset on positioning here
    public Vector3 POS_OFFSET;

    //Rot offset on moving here
    public Vector3 ROT_OFFSET;


    //On use move the object here
    public override void Use(Hand hand)
    {
        //Defaults to the option given, else use just this object as parent.
        if(PARENT != null)
        {
            OBJ.transform.parent = PARENT;
        }
        else
        {
            OBJ.transform.parent = transform;
        }
      
        OBJ.transform.localPosition = POS_OFFSET;
        OBJ.transform.localRotation = Quaternion.Euler(ROT_OFFSET);
    }
}
