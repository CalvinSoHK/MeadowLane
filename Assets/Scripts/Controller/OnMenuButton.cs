using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

//On menu button, call the event.
public class OnMenuButton : MonoBehaviour {

    //The hand we're attached to.
    Hand hand;

    //The event we will call on menu press
    public UnityEvent OnMenuButtonPress;

    void Start()
    {
        hand = GetComponent<Hand>();
      
    }

    void Update()
    {
        if (hand.GetMenuButtonDown())
        {
            OnMenuButtonPress.Invoke();
        }
    }


}
