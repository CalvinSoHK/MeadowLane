using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

//Fires an event on interact
public class InteractFireEvent : InteractableCustom{

    public UnityEvent OnInteractEvent = new UnityEvent();

    public override void Use(Hand hand)
    {
        OnInteractEvent.Invoke();
        base.Use(hand);
    }

}
