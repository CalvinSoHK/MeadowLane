using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

//Customizing stuff
public class HomeCustomizationManager : MonoBehaviour {

    //Whether or not we are currently customizing
    public bool currentlyCustomizingHome = false;

    //Whether or not the item we're using is NOT in the scene yet
    public bool inScene = false;

    //What state we are in within the customization state machine
    public enum CustomizeState { Idle, Selected, Stop}
    public CustomizeState currentCustomizeState;

    //How long we have been in a given state
    float stateTimer = 0.0f;

    //The selected object and its hologram reference object
    GameObject currentlySelectedObject, hologramRefToSelectedObject;

    //Array of materials that we will constantly be using. Init here for memory optimization
    Material[] hologramMaterial = new Material[2];
    Material PREV_MATERIAL;

    HandInputs INPUT = new HandInputs();
    HandInputs OFF_INPUT = new HandInputs();

    //Both hands
    public Hand hand1, hand2;
    PlayerInputManager PIM;

    //Where the phone is showing
    public enum UseState { None, Hand1, Hand2 };
    public UseState SHOW_STATE= UseState.None;

    //Vibration enum
    public enum VibrationHand { Both, Hand1, Hand2 };

    //Ray for our raycasting
    Ray HAND_POINT = new Ray();
    RaycastHit HIT = new RaycastHit();

    //Save the old layer just in case
    int PREV_LAYER = -1;

    public bool INVENTORY_CALL = false;

    //Layermask for our raycast
    public LayerMask PLACEMENT_LAYERMASK, RAYCAST_LAYERMASK;

    public enum ROTATION_SNAPS { Zero, Five, Thirty, FortyFive, Ninety };
    public ROTATION_SNAPS ROT_SNAP = ROTATION_SNAPS.Zero;
    float SNAP = 1;
    Vector3 NEW_ROT = Vector3.zero;

    public enum GRID_SNAPS { None, One };
    public GRID_SNAPS GRID_SNAP = GRID_SNAPS.None;

    public GameObject FurnitureUI;
    public Text FurnitureRotation;
    public Image FurnitureGrid;
    float furnitureGridUIAlpha = 147;
    Color tmp;
    public Vector3 furnitureUIOffset, furnitureUIRotationOffset;

    // Use this for initialization
    void Start () {
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
        PIM = GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.GetComponent<PlayerInputManager>();
    }
	
	// Update is called once per frame
	void Update () {

        //Retrieve hand inputs
        RetrieveHandInputs();

        if (currentlyCustomizingHome)
        {
            switch (currentCustomizeState)
            {
                case CustomizeState.Idle:
                    //Do nothing. Just wait.
                    
                    break;

                case CustomizeState.Selected: //Make the object follow our raycast location
                    //NOTE: Selected object is assigned in OnTriggerRaycast.
                    setFurnitureUIHand();
                    AssignRaycast();

                    //If we don't have a selectable object, its an error and move to the stop state.
                    if (inScene && currentlySelectedObject == null)
                    {
                        Debug.Log("Error: Don't actually have a selected object.");
                        SetCurrentHomeState(CustomizeState.Stop);
                    }
                    else if(!inScene && hologramRefToSelectedObject == null)
                    {
                        Debug.Log("Error: Don't actually have a hologram object.");
                        SetCurrentHomeState(CustomizeState.Stop);
                    }
                    else
                    {
                        //Assign the right ray to raycast
                        AssignRaycast();

                        //As long as we're not none, and thus hand-pointer is not null.
                        if(SHOW_STATE != UseState.None)
                        {
                            //If we hit a point
                            if (Physics.Raycast(HAND_POINT, out HIT, 1000f, PLACEMENT_LAYERMASK))
                            {
                                hologramRefToSelectedObject.GetComponent<CheckIfColliding>().TOO_FAR = false;
                                if(GRID_SNAP == GRID_SNAPS.None)
                                {
                                    hologramRefToSelectedObject.transform.position = HIT.point;
                                }
                                else if(GRID_SNAP == GRID_SNAPS.One)
                                {
                                    //Snaps to the Second decimal place, as in every tenth centimeter
                                    hologramRefToSelectedObject.transform.position =
                                        new Vector3(Mathf.Round(HIT.point.x * 10f) / 10f,
                                        HIT.point.y,
                                          Mathf.Round(HIT.point.z * 10f) / 10f);
                                }
                               
                            }
                            else
                            {
                                hologramRefToSelectedObject.GetComponent<CheckIfColliding>().TOO_FAR = true;
                            }

                            //Calculate our rotational snap
                            if (ROT_SNAP == ROTATION_SNAPS.Zero)
                            {
                                SNAP = 1f;
                            }
                            else if(ROT_SNAP == ROTATION_SNAPS.Five)
                            {
                                SNAP = 5f;
                            }
                            else if(ROT_SNAP == ROTATION_SNAPS.Thirty)
                            {
                                SNAP = 30f;
                            }
                            else if(ROT_SNAP == ROTATION_SNAPS.FortyFive)
                            {
                                SNAP = 45f;
                            }
                            else if(ROT_SNAP == ROTATION_SNAPS.Ninety)
                            {
                                SNAP = 90f;
                            }

                            if (INPUT.RIGHT && INPUT.TRACKPAD_DOWN)
                            {
                                NEW_ROT = hologramRefToSelectedObject.transform.eulerAngles;
                                NEW_ROT.y = Mathf.Round((NEW_ROT.y + SNAP) / SNAP) * SNAP;
                                hologramRefToSelectedObject.transform.rotation = Quaternion.Euler(NEW_ROT);
                            }
                            else if(INPUT.LEFT && INPUT.TRACKPAD_DOWN)
                            {
                                NEW_ROT = hologramRefToSelectedObject.transform.eulerAngles;
                                NEW_ROT.y = Mathf.Round((NEW_ROT.y - SNAP) / SNAP) * SNAP;
                                hologramRefToSelectedObject.transform.rotation = Quaternion.Euler(NEW_ROT);
                            }
                            else if (INPUT.TRACKPAD_DOWN && !INVENTORY_CALL)
                            {
                                //Assign the right layer back to the og object regardless of if it was moved or not
                                currentlySelectedObject.layer = PREV_LAYER;
                                if (!inScene)
                                {
                                    Inventory_Manager.AddItemToInventory(hologramRefToSelectedObject.GetComponent<BaseItem>(),
                                        Inventory_Manager.FurnitureCategory, Inventory_Manager.FurnitureCategorySlots);
                                }
                                Destroy(hologramRefToSelectedObject.gameObject);
                                SetCurrentHomeState(CustomizeState.Stop);
                            }
                            else if (INPUT.TRACKPAD_DOWN)
                            {
                                INVENTORY_CALL = false;
                            }
                            Debug.Log(NEW_ROT);

                            //Press down on trackpad to attempt to place it down.
                            //Debug.Log(INPUT.TRIGGER_DOWN);
                            if (INPUT.TRIGGER)
                            {
                                //Debug.Log("Trigger down.");
                                if (hologramRefToSelectedObject.GetComponent<CheckIfColliding>().IS_VALID)
                                {
                                    //Debug.Log("Valid. Placing.");
                                    placeObject();
                                    SetCurrentHomeState(CustomizeState.Stop);
                                }
                            }

                          
                        }
                    }
                    break;

                case CustomizeState.Stop:
                    FurnitureUI.SetActive(false);
                    EnableRaycasting();

                    SetCurrentHomeState(CustomizeState.Idle);
                    break;
            }
        }
	}
    /*
    /// <summary>
    /// set whether or not we are currently customizing the home
    /// should be activated on press of the top vive controller button
    /// </summary>
    /// <param name="customized"></param>
    public void CustomizeHome()
    {
        if (currentlyCustomizingHome) // if the player is already in the customize state then turn it off
        {
            currentlyCustomizingHome = false;

        }else // other set it to true
        {
            currentlyCustomizingHome = true;
        } 
    }*/

    /// <summary>
    /// getter to see if we are currently customizing the home
    /// </summary>
    /// <returns></returns>
    public bool GetCustomizeState()
    {
        return currentlyCustomizingHome;
    }

    //Re-enable being able to interact with things
    public void EnableRaycasting()
    {
        //Assign the right ray to raycast
        if (SHOW_STATE == UseState.Hand1)
        {
            hand1.GetComponent<OnTriggerRaycast>().ENABLED = true;
        }
        else if (SHOW_STATE == UseState.Hand2)
        {
            hand2.GetComponent<OnTriggerRaycast>().ENABLED = true;
        }
    }

    //Assigns raycast to the right hand based on show state.
    public void AssignRaycast()
    {
        //Assign the right ray to raycast
        if (SHOW_STATE == UseState.Hand1)
        {
            HAND_POINT = new Ray(hand1.transform.position, hand1.transform.forward);
        }
        else if (SHOW_STATE == UseState.Hand2)
        {
            HAND_POINT = new Ray(hand2.transform.position, hand2.transform.forward);
        }
    }

    //Receives all inputs
    public void RetrieveHandInputs()
    {
        if (PIM.isMode(PlayerInputManager.InputMode.Edit))
        {
            if (SHOW_STATE == UseState.Hand1)
            {
                //Debug.Log("Hand 1");
                INPUT.CopyValues(PIM.HAND1);
            }
            else if (SHOW_STATE == UseState.Hand2)
            {
                //Debug.Log("Hand 2");
                INPUT.CopyValues(PIM.HAND2);
            }
            else
            {
                //Debug.Log("Clear values");
                INPUT.ClearValues();
            }
        }
        else
        {
            //Debug.Log("Clear values default");
            INPUT.ClearValues();
        }
    }

    public void SetCurrentHomeState(CustomizeState state)
    {
        currentCustomizeState = state;
        stateTimer = Time.time;
    }

    public void SetUseState(UseState state)
    {
        SHOW_STATE = state;
    }

    public float getElapsedTime()
    {
        return Time.time - stateTimer;
    }

    public void selectObject(GameObject og, bool in_scene)
    {
        inScene = in_scene;

        if (hologramRefToSelectedObject != null) //if the hologram ref is not equal to null
        {
            Destroy(hologramRefToSelectedObject); //destroy the previous hologram object
        }

        if (inScene) //If this object is already in the scene
        {
            //Disable raycasting
            hand1.GetComponent<OnTriggerRaycast>().ENABLED = false;
            hand2.GetComponent<OnTriggerRaycast>().ENABLED = false;

            currentlySelectedObject = og; //set the ref to the currently referenced object

            hologramRefToSelectedObject = Instantiate(currentlySelectedObject); //create the hologram version of the currently selected object

            //Make the original object the UI layer so we can adjust its position in the same place
            PREV_LAYER = currentlySelectedObject.layer;
            currentlySelectedObject.layer = 5;
        }
        else //If this object is from inventory
        {
            //Disable raycasting
            hand1.GetComponent<OnTriggerRaycast>().ENABLED = false;
            hand2.GetComponent<OnTriggerRaycast>().ENABLED = false;

            currentlySelectedObject = null;
            hologramRefToSelectedObject = og;//Hologram is the object itself
            PREV_MATERIAL = hologramRefToSelectedObject.GetComponent<MeshRenderer>().material;
            PREV_LAYER = hologramRefToSelectedObject.layer;
            hologramRefToSelectedObject.layer = 5;

        }

        //Get the hologram object ready
        hologramMaterial = new Material[2]; // get two new empty material slots
        hologramMaterial[0] = Resources.Load("Materials/Transparent", typeof(Material)) as Material; // load the hologram and highlight materials 
        hologramMaterial[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
        hologramRefToSelectedObject.GetComponent<Renderer>().materials = hologramMaterial; //assign the new materials to the hologram object
        hologramRefToSelectedObject.AddComponent<CheckIfColliding>(); //Adds the script checkifcolliding.

        if (inScene)
        {
            hologramRefToSelectedObject.GetComponent<CheckIfColliding>().IgnoreCollision(currentlySelectedObject);
        } 

        //Makes the collider compatible with the rigidbody.
        if (hologramRefToSelectedObject.GetComponent<MeshCollider>() != null)
        {
            hologramRefToSelectedObject.GetComponent<MeshCollider>().convex = true;
            hologramRefToSelectedObject.GetComponent<MeshCollider>().isTrigger = true;
        }

        //Change layer of hologram so we don't accidentally hit it with our raycast during placement
        hologramRefToSelectedObject.layer = 5;

        //Make the hologram have a rigidbody so its on trigger calls work. Kinematic so it isnt pushed around by things.
        hologramRefToSelectedObject.AddComponent<Rigidbody>();
        hologramRefToSelectedObject.GetComponent<Rigidbody>().isKinematic = true;
        FurnitureUI.SetActive(true);
        setFurnitureUIHand();       
        SetCurrentHomeState(CustomizeState.Selected); //Set the current state to the selected one
    }

    public void placeObject()
    {
        if (inScene) //If we are already in the scene, move the real object to the right place.
        {
            currentlySelectedObject.layer = PREV_LAYER;
            currentlySelectedObject.transform.position = hologramRefToSelectedObject.transform.position;
            currentlySelectedObject.transform.rotation = hologramRefToSelectedObject.transform.rotation;
            Destroy(hologramRefToSelectedObject.gameObject);
            currentlySelectedObject = null;
        }
        else //If we aren't in the scene, we need to revert changes.
        {
            //Reset the material
            hologramMaterial = new Material[1];
            hologramMaterial[0] = PREV_MATERIAL;
            hologramRefToSelectedObject.GetComponent<MeshRenderer>().materials = hologramMaterial;

            //Remove checkifcolliding
            Destroy(hologramRefToSelectedObject.GetComponent<CheckIfColliding>());

            //Remove rigidbody
            Destroy(hologramRefToSelectedObject.GetComponent<Rigidbody>());

            //Revert collider changes
            hologramRefToSelectedObject.GetComponent<MeshCollider>().isTrigger = false;
            hologramRefToSelectedObject.GetComponent<MeshCollider>().convex = false;

            //Revert layer change
            hologramRefToSelectedObject.layer = PREV_LAYER;

            hologramRefToSelectedObject = null;
        }  
    }

    public void cancelObject()
    {
        if (inScene)
        {
            currentlySelectedObject.layer = PREV_LAYER;
            Destroy(hologramRefToSelectedObject.gameObject);
            currentlySelectedObject = null;
        }
        else
        {
            //Add it back to inventory
            Inventory_Manager.AddItemToInventory(hologramRefToSelectedObject.GetComponent<BaseItem>(), 
                Inventory_Manager.FurnitureCategory, Inventory_Manager.FurnitureCategorySlots);

            //Remove it from scene
            Destroy(hologramRefToSelectedObject);
            currentlySelectedObject = null;
        }
        SetCurrentHomeState(CustomizeState.Idle);
    }

    /// <summary>
    /// Handles the Furniture UI that snaps the location and rotation
    /// </summary>
    public void FurnitureUIHandler(Hand currentHand)
    {
        FurnitureUI.GetComponent<RectTransform>().anchoredPosition3D = currentHand.transform.position + furnitureUIOffset;
        FurnitureUI.GetComponent<RectTransform>().eulerAngles = currentHand.transform.rotation.eulerAngles + furnitureUIRotationOffset;
        if (OFF_INPUT.Left && OFF_INPUT.TrackPadDown)
        {
            if(ROT_SNAP == ROTATION_SNAPS.Zero)
            {
                ROT_SNAP = ROTATION_SNAPS.Ninety;
            }else
            {
                ROT_SNAP -= 1;
            }
            
        }else if (OFF_INPUT.Right && OFF_INPUT.TrackPadDown)
        {
            if (ROT_SNAP == ROTATION_SNAPS.Ninety)
            {
                ROT_SNAP = ROTATION_SNAPS.Zero;
            }
            else
            {
                ROT_SNAP += 1;
            }
        }
        if (OFF_INPUT.Up && OFF_INPUT.TrackPadDown)
        {
            if (GRID_SNAP == GRID_SNAPS.One)
            {
                GRID_SNAP = GRID_SNAPS.None;
            }
            else
            {
                GRID_SNAP -= 1;
            }

        }
        else if (OFF_INPUT.Down && OFF_INPUT.TrackPadDown)
        {
            if (GRID_SNAP == GRID_SNAPS.None)
            {
                GRID_SNAP = GRID_SNAPS.One;
            }
            else
            {
                GRID_SNAP += 1;
            }
        }
        switch (ROT_SNAP)
        {
            case ROTATION_SNAPS.Ninety:
                FurnitureRotation.text = "90°";
                break;
            case ROTATION_SNAPS.FortyFive:
                FurnitureRotation.text = "45°";
                break;
            case ROTATION_SNAPS.Thirty:
                FurnitureRotation.text = "30°";
                break;
            case ROTATION_SNAPS.Five:
                FurnitureRotation.text = "5°";
                break;
            case ROTATION_SNAPS.Zero:
                FurnitureRotation.text = "0°";
                break;
        }
        switch (GRID_SNAP)
        {
            case GRID_SNAPS.None:
                tmp = FurnitureGrid.color;
                furnitureGridUIAlpha = 144f;
                tmp.a = furnitureGridUIAlpha;
                FurnitureGrid.color = tmp;
                break;

            case GRID_SNAPS.One:
                tmp = FurnitureGrid.color;
                furnitureGridUIAlpha = 255f;
                tmp.a = furnitureGridUIAlpha;
                FurnitureGrid.color = tmp;
                break;
        }
    }
    public void setFurnitureUIHand()
    {
        if (SHOW_STATE == UseState.Hand1)
        {
            FurnitureUIHandler(hand2);
        }
        else if (SHOW_STATE == UseState.Hand2)
        {
            FurnitureUIHandler(hand1);
        }
    }
}
