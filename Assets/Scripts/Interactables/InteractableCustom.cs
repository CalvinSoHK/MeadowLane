using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

//Custom version of Interactable for our game.
public class InteractableCustom : Interactable{

    //UI Image prefab that will come up when you hover over something
    public GameObject UI_PREFAB;

    //The scene prefab
    GameObject IN_SCENE_PREFAB;

    //The image prefab that is in the world
    GameObject IMG;

    //UI Image's offset from where you ray cast the UI image to be.
    public Vector3 UI_OFFSET = Vector3.zero;

    //UI Image's distance from object towards player on initial spawn
    public float DIST_TO_PLAYER = 0.5f;

    //Bool to disable this interactables UI from showing
    public bool SHOW_UI = false;

    private void Awake()
    {
        if(UI_PREFAB != null)
        {
            IN_SCENE_PREFAB = Instantiate(UI_PREFAB, (transform.position), Quaternion.identity, transform);
        }
       
    }

    //Overrideable use function for all interactable objects
    public virtual void Use(Hand hand)
    {
        //Despawn the UI on use
        HideUI();
    }

    //Function that is used to display the UI image for the player
    public void DisplayUI(Transform PLAYER)
    {
        //If we haven't had a UI image spawned yet AND we have a UI prefab
        if(UI_PREFAB != null && !IN_SCENE_PREFAB.activeSelf)
        {
            //Calculate the direction towards the player
            Vector3 DIR_TO_PLAYER = (PLAYER.position - transform.position);

            //Calculate the rotation to face the player
            Quaternion ROT = Quaternion.LookRotation(DIR_TO_PLAYER);

            //Spawn the image at our position plus offset at the calculated rot, in our transform.
            IN_SCENE_PREFAB.transform.position = transform.position;
            IN_SCENE_PREFAB.transform.rotation = ROT;
            IN_SCENE_PREFAB.GetComponent<UI_Controller>().TARGET = PLAYER;
            IN_SCENE_PREFAB.GetComponent<UI_Controller>().FACE_TARGET = PLAYER;
            IN_SCENE_PREFAB.GetComponent<UI_Controller>().IS_SHOWING = true;
            IN_SCENE_PREFAB.GetComponent<UI_Controller>().OFFSET = UI_OFFSET;
            IN_SCENE_PREFAB.SetActive(true);
        }
    }

    //Function that is used to hide the UI image from the player
    public void HideUI()
    {
        //If we have UI showing, destroy it.
        if(IN_SCENE_PREFAB != null)
        {
            if (IN_SCENE_PREFAB.activeSelf)
            {
                //Set target to home to go backwards
                IN_SCENE_PREFAB.GetComponent<UI_Controller>().TARGET = IN_SCENE_PREFAB.GetComponent<UI_Controller>().HOME;
                IN_SCENE_PREFAB.GetComponent<UI_Controller>().IS_SHOWING = false;
            }
        }
        
    }
}
