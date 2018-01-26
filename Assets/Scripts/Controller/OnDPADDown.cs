using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

public class OnDPADDown : MonoBehaviour
{

    //The hand we're attached to.
    Hand hand;

    //The event we will call on menu press
    public UnityEvent OnDPAD_DOWN;

    void Start()
    {
        hand = GetComponent<Hand>();

    }

    void Update()
    {
        if (hand.GetTrackpadPressDown() && hand.GetTrackpadDown())
        {
            OnDPAD_DOWN.Invoke();
        }
    }

}
