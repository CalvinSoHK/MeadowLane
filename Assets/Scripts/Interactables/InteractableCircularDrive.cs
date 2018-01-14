using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractableCircularDrive : InteractableCustom {

    //Circular drive linked to this interact
    public CircularDriveCustom DRIVE;

    //On use follow the circular drive linked
    public override void Use(Hand hand)
    {
        DRIVE.FollowHand(hand);
    }

}
