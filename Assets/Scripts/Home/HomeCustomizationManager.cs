using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeCustomizationManager : MonoBehaviour {
    public bool currentlyCustomizingHome = false;
    public enum CustomizeState { Idle, Selected, Moving, Stop}
    public CustomizeState currentCustomizeState;
    float stateTimer = 0.0f;
    GameObject currentlySelectedObject, hologramRefToSelectedObject;
    Material[] hologramMaterial = new Material[2];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (currentlyCustomizingHome)
        {
            switch (currentCustomizeState)
            {
                case CustomizeState.Idle:

                    break;

                case CustomizeState.Selected: //is this obselete??????

                    break;

                case CustomizeState.Moving:

                    break;

                case CustomizeState.Stop:

                    break;
            }
        }
	}

    /// <summary>
    /// set whether or not we are currently customizing the home
    /// </summary>
    /// <param name="customized"></param>
    public void CustomizeHome(bool customized)
    {
        if (currentlyCustomizingHome)
        {
            currentlyCustomizingHome = false;

        }else
        {

        }
        
    }
    /// <summary>
    /// getter to see if we are currently customizing the home
    /// </summary>
    /// <returns></returns>
    public bool CustomizingHome()
    {
        return currentlyCustomizingHome;
    }

    public void SetCurrentHomeState(CustomizeState state)
    {
        currentCustomizeState = state;
        stateTimer = Time.time;
    }

    public float getElapsedTime()
    {
        return Time.time - stateTimer;
    }

    public void selectObject(GameObject og)
    {
        currentlySelectedObject = og; //set the ref to the currently referenced object
        if(hologramRefToSelectedObject != null) //if the hologram ref is not equal to null
        {
            Destroy(hologramRefToSelectedObject); //destroy the previous hologram object
        }
        hologramRefToSelectedObject = Instantiate(currentlySelectedObject); //create the hologram version of the currently selected object
        hologramRefToSelectedObject.SetActive(false); // turn of the new object
        hologramMaterial = new Material[2]; // get two new empty s
        hologramMaterial[0] = Resources.Load("Materials/Transparent", typeof(Material)) as Material;
        hologramMaterial[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
        hologramRefToSelectedObject.GetComponent<Renderer>().materials = hologramMaterial;

    }
}
