using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

/// <summary>
/// Interactable class that will allow us to select this entry.
/// </summary>
public class InteractSelectEntry : InteractableCustom {

    public BusEntryManager BEM;

    void Awake()
    {
        if (BEM == null)
        {
            BEM = transform.parent.GetComponent<BusEntryManager>();
        }
    }

    public override void Use(Hand hand)
    {
        //Debug.Log("setting to true.");
        BEM.SelectEntry(GetComponent<SingleEntryManager>());
    }
}
