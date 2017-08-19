using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Same as travel, but moves the player itself into a fixed position. Like oculus rift.
public class InteractTravelFixed : InteractableCustom {

    //Destination of player prefab
    public Transform destination;

    //Override use function to move the player rig
    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {
            Debug.Log("Fixed Travel");
            Camera.main.GetComponent<ScreenTransitionImageEffect>().MovePlayer(destination,
                hand.gameObject.transform.root, true);
        }
    }
}
