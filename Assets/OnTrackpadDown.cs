using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

public class OnTrackpadDown : MonoBehaviour {

    //The hand we're attached to.
    Hand hand;

    //The event we will call on menu press
    public UnityEvent OnTrackPadDown;

    void Start()
    {
        hand = GetComponent<Hand>();

    }

    void Update()
    {
        if (hand.GetTrackpadDown())
        {
            OnTrackPadDown.Invoke();
        }
    }

}
