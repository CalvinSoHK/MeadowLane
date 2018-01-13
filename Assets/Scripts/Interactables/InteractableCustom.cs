using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

//Custom version of Interactable for our game.
public class InteractableCustom : Interactable{

    //UI Image prefab that will come up when you hover over something
    public GameObject UI_PREFAB;

    //The image prefab that is in the world
    Image IMG;

    //UI Image's offset from where you ray cast the UI image to be.
    Vector3 UI_OFFSET = Vector3.zero;

    //Bool to disable this interactables UI from showing
    public bool SHOW_UI = true;

    //Overrideable use function for all interactable objects
	public virtual void Use(Hand hand)
    {
        //Default interaction is nothing. Debug something
        Debug.Log(gameObject.name + " Interactable: Has no use function");
    }

    //Function that is used to display the UI image for the player
    public void DisplayUI(Vector3 PLAYER_POSITION)
    {
        //If we haven't had a UI image spawned yet
        if(IMG == null && SHOW_UI)
        {
            //Calculate the rotation to face the player
            Quaternion ROT = Quaternion.LookRotation(PLAYER_POSITION - transform.position + UI_OFFSET);

            //Spawn the image at our position plus offset at the calculated rot, in our transform.
            IMG = Instantiate(UI_PREFAB, transform.position + UI_OFFSET, ROT, transform).GetComponent<Image>();
        }
    }

    //Function that is used to hide the UI image from the player
    public void HideUI()
    {
        //If we have UI showing, destroy it.
        if(IMG != null)
        {
            Destroy(IMG.gameObject);
        }
    }
}
