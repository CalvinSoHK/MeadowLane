using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Custom version of Interactable for our game.
public class InteractableCustom : Interactable{

    //Overrideable use function for all interactable objects
	public virtual void Use(Hand hand)
    {
        //Default interaction is nothing. Debug something
        Debug.Log(gameObject.name + " Interactable: Has no use function");
    }
}
