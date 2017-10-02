using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(CircularDriveCustom))]
//Custom interactactble for rotating use.   
//On standard button down on the OTHER hand, moves forward
//Enables rigidbody on move, disables on stop.
public class InteractableRotator : InteractableCustom {

    //The transform that we want to move
    public Transform vehicle = null;

    //Speed of the car
    public float speed;

    //Rotation of the wheel
    float yRotation = 0;

    //Ref for current velocity
    float currentVelocityRot;

    //Ref for Rigidbody
    public Rigidbody rb;

    //Ref for meshCollider
    public MeshCollider mc;

    //Write use function
    public override void Use(Hand hand)
    {
        //Nothing should happen.
    }

    void Update()
    {
        //Get the computed angle.
        yRotation = GetComponent<CircularDriveCustom>().outAngle;
        //Debug.Log(yRotation);

        //If the driving wheel is currently engaged.
        if (GetComponent<CircularDriveCustom>().isUse)
        {
            //Retrieve other hand
            Hand otherHand = GetComponent<CircularDriveCustom>().drivingHand.otherHand;


            //If we get the other controller trigger
            if (otherHand.GetStandardInteractionButton())
            {
                mc.enabled = false;
                rb.isKinematic = false;
                //Calculate target forward vector based on yRotation
                Quaternion targetRot = vehicle.rotation;
                targetRot = Quaternion.AngleAxis(yRotation, vehicle.up) * targetRot;

                //Always lerp towards targetRot
                vehicle.rotation = Quaternion.Lerp(vehicle.rotation, targetRot, 0.01f);

                //If we move forward
                vehicle.GetComponent<Rigidbody>().MovePosition(vehicle.position + vehicle.forward * speed);
            }
            else
            {
                rb.isKinematic = true;
                mc.enabled = true;
            }
        }

    }
}