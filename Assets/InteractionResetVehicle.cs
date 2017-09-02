using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Takes a vehicle and repositions it on use.
//Combine with InteractionTravelFixed to move things when out of site into a more favorable position
public class InteractionResetVehicle : InteractableCustom {

    //The vehicle in question
    public GameObject Vehicle;

    //The position
    public Vector3 position;

    //The rotation
    public Vector3 rotation;

    //On use, move the vehicle to the position and rotation.
    public override void Use(Hand hand)
    {
        Vehicle.transform.position = position;
        Vehicle.transform.eulerAngles = rotation;
    }

}
