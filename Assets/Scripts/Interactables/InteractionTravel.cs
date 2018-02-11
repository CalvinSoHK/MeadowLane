using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script that, on use, lets the player travel through this object.
public class InteractionTravel : InteractableCustom {

    //Destination of player prefab
    public Transform destination;

    //Whether or not it should flip the player on travel
    public bool FLIP = false;

    //Whether or not we want it to anchor
    public bool ANCHOR = false;

    //The travel point we are managing
    public TravelPointController TPC;

    TravelPointManager TPM;

    //Override use function to move the player rig
    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {
            if (ANCHOR)
            {
                Camera.main.GetComponent<ScreenTransitionImageEffect>().MovePlayer(destination,
                    hand.transform.parent.parent, false, ANCHOR);
            }
            else
            {
                Camera.main.GetComponent<ScreenTransitionImageEffect>().MovePlayer(destination,
                hand.transform.parent.parent, false, ANCHOR);
            }

            destination.GetComponent<TravelPointController>().isValid = false;

            if(TPM == null)
            {
                TPM = GameManagerPointer.Instance.TRAVEL_POINT_MANAGER;
            }

            if(TPC == null)
            {
                TPC = destination.GetComponent<TravelPointController>();
            }

            TPM.ResetValid();
            TPM.SetIndex(TPM.GetIndex(TPC));
        }
        base.Use(hand);

    }
}
