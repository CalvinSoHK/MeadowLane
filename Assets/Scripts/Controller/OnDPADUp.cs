using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

public class OnDPADUp : MonoBehaviour
{

    //The hand we're attached to.
    Hand hand;

    //The event we will call on menu press
    public UnityEvent OnDPAD_UP;

    void Start()
    {
        hand = GetComponent<Hand>();

    }

    void Update()
    {
        if (hand.GetTrackpadPressUp() && hand.GetTrackpadDown())
        {
            OnDPAD_UP.Invoke();
        }
    }

}
