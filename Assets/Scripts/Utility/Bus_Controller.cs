using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Bus controller script. Places the bus at key locations when given a location and moves in a direction.
//NOTE: Script doesnt work if the target is Vector3.zero.
public class Bus_Controller : MonoBehaviour {

    //Target location
    Vector3 DESTINATION_POS= Vector3.zero;
    Quaternion DESTINATION_ROT = Quaternion.identity;

    //Moving bool
    bool isMoving = false, isBlinking = false, isLoaded = false;

    //Reference to the transition index
    int TRANSITION_INDEX = 0;

    //The VR camera
    GameObject VR_CAMERA;

    //For now use a timer in the transition screen. Remove when the other code is ready
    float timer = 8f;

    //Stop manager
    Bus_Stop_Manager MANAGER;

    void Start()
    {
        //Might be problematic if it changes.
        VR_CAMERA = GameObject.Find("VRCamera");
        MANAGER = Bus_Stop_Manager.Instance;
    }

    void Update()
    {

        //If we have finished loading
        if (isLoaded && VR_CAMERA.GetComponent<ScreenTransitionImageEffect>().GetCurrentState() == ScreenTransitionImageEffect.Gamestate.open)
        {
            isLoaded = false;
            //Debug.Log("Scene loaded");
            GameObject BUS_POINT = GameObject.Find("BusPoint");
            transform.rotation = BUS_POINT.transform.rotation;
            transform.position = BUS_POINT.transform.position;
            isMoving = true;

        }

        //If we should be moving
        if (isMoving)
        {
            transform.position += transform.forward * MANAGER.MAX_SPEED * Time.deltaTime;
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else if(!isBlinking)
            {
                Debug.Log("Blink");
                VR_CAMERA.GetComponent<ScreenTransitionImageEffect>().BlinkEyes();
                isBlinking = true;
            }
        }

        //If it finished blinking and we just finish blinking, move us to our final destination
        if(VR_CAMERA.GetComponent<ScreenTransitionImageEffect>().GetCurrentState() == ScreenTransitionImageEffect.Gamestate.open && isBlinking)
        {
            timer = 8f;
            isMoving = false;
            transform.position = DESTINATION_POS;
            transform.rotation = DESTINATION_ROT;
            SceneManager.UnloadSceneAsync(TRANSITION_INDEX + 1);
            isBlinking = false;
            Debug.Log("Blink finished");
        }
       
       
    }

    //Function called when transition is done loading
    //Has to take those inputs to be a function added to the scene loaded function.
    public void TransitionLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoaded = true;
        SceneManager.sceneLoaded -= TransitionLoaded;
        //Now load the destination scene. Not yet ready for that format but this is where it would be.
        //NOTE: Load the destination scene. Add a the function DestinationLoaded to sceneloaded.

    }

    //Function called when the destination is done loading
    //Has to take those inputs to be a function added to the scene loaded function.
    public void DestinationLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    //Function that acts as though we are taking the bus to given location. Transition type denotes
    //First we figure out which location we are going to. The index of that corresponds to the other indexes.
    public void MoveTo(string DESTINATION)
    {
        //Find the index of the given location
        int index = MANAGER.STOP_LIST.IndexOf(DESTINATION);
        Debug.Log(index + DESTINATION);

        //Get the transition type from the destination
        BusEntryManager STOP = Bus_Stop_Manager.Instance.GetBusStop(DESTINATION);
        TRANSITION_INDEX = (int)STOP.TRANSITION_TO;

        //If index is -1, its not in the list and we have an error, else do the right thing.
        if(index != -1)
        {
            //Get the position of our final destination
            DESTINATION_POS = STOP.BUS_ARRIVAL.transform.position;
            DESTINATION_ROT = STOP.BUS_ARRIVAL.transform.rotation;

            //Load the scene
            SceneManager.LoadSceneAsync(TRANSITION_INDEX + 1, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += TransitionLoaded;
        }
        else
        {
            Debug.Log("ERROR: Invalid index. Location doesn't exist on list. Check spelling on bus terminal and in bus.");
        }
    }
}
