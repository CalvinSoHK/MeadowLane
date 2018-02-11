using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractionTravelToBus : InteractionTravel {


    private void Awake()
    {
        ANCHOR = true;
        //Debug.Log(Bus_Stop_Manager.Instance.name);
        destination = GameManagerPointer.Instance.BUS_STOP_MANAGER.BUS.gameObject.transform.Find("TravelPoint");

        //Debug.Log(destination.name);
        TPC = destination.GetComponent<TravelPointController>();
    }

    public override void Use(Hand hand)
    {
        base.Use(hand);
    }
}
