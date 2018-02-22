using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Bus controller script. Places the bus at key locations when given a location and moves in a direction.
//NOTE: Script doesnt work if the target is Vector3.zero.
public class Bus_Controller : MonoBehaviour {

    //Moving bool
    bool isMoving = false, isBlinking = false, isTransitionLoaded = false, isSceneLoaded = false;

    //Reference to the transition index
    int TRANSITION_INDEX = 0;

    //The VR camera
    GameObject VR_CAMERA;
    string TRANSITION_SCENE;

    //For now use a timer in the transition screen. Remove when the other code is ready
    public float MIN_LOAD_TIME = 3f;
    float timer = 3f;

    //Stop manager
    Bus_Stop_Manager BSM;

    //Stop info for our target destination
    BusStopInfo NEW_STOP_INFO;

    //Bus point we are moving to.
    GameObject BUS_POINT;

    FarmManagerPointer FMP;

    void Start()
    {
        //Might be problematic if it changes.
        VR_CAMERA = GameObject.Find("VRCamera");
        BSM = GameManagerPointer.Instance.BUS_STOP_MANAGER;
        NEW_STOP_INFO = BSM.STOP_LIST[BSM.GetIndexOf("Player Home")];
        SaveSystem.ClearTempData();

    }

    void Update()
    {
        if(BSM == null)
        {
            BSM = GameManagerPointer.Instance.BUS_STOP_MANAGER;
        }

        //If we have finished loading
        if (isTransitionLoaded && VR_CAMERA.GetComponent<ScreenTransitionImageEffect>().GetCurrentState() == ScreenTransitionImageEffect.Gamestate.open)
        {
            //Set is loaded to false
            isTransitionLoaded = false;
            //Debug.Log("Scene loaded");

            //Place us on the bus in the right position
            BUS_POINT = GameObject.Find("BusPoint");
            transform.rotation = BUS_POINT.transform.rotation;
            transform.position = BUS_POINT.transform.position;
            timer = MIN_LOAD_TIME;

            //Start unloading the old scene
            SceneManager.UnloadSceneAsync(BSM.CURRENT_STOP.GetName());

            BSM.CURRENT_STOP = NEW_STOP_INFO;

            //When the old scene is unloaded fire the event old scene unloaded.
            SceneManager.sceneUnloaded += OldSceneUnloaded;

            isMoving = true;
        }

        //If we should be moving
        if (isMoving)
        {
            transform.position += transform.forward * BSM.MAX_SPEED * Time.deltaTime;
            timer -= Time.deltaTime;
        }

        if(timer <= 0 && isSceneLoaded)
        {
            //Debug.Log("Blink!");
            isBlinking = true;
            isSceneLoaded = false;
            VR_CAMERA.GetComponent<ScreenTransitionImageEffect>().BlinkEyes();
        }   

        //If it finished blinking and we just finish blinking, move us to our final destination
        if (VR_CAMERA.GetComponent<ScreenTransitionImageEffect>().GetCurrentState() == ScreenTransitionImageEffect.Gamestate.open && isBlinking)
        {
            isMoving = false;
            isBlinking = false;

            //Set the bus in the right position.
            //Find the transform we need to use
            Transform BUS_ARRIVAL = GameObject.Find("BusArrival").transform;
            transform.position = BUS_ARRIVAL.position;
            transform.rotation = BUS_ARRIVAL.rotation;

            //Unload the transition scene
            //Debug.Log(TRANSITION_SCENE);
            SceneManager.UnloadSceneAsync(TRANSITION_SCENE);
        }
    }

    //Function called when transition is done loading
    //Has to take those inputs to be a function added to the scene loaded function.
    public void TransitionLoaded(Scene scene, LoadSceneMode mode)
    {
        //Set isLoaded to true so we move the player to the bus
        isTransitionLoaded = true;


        //Remove this function from sceneLoaded event
        SceneManager.sceneLoaded -= TransitionLoaded;
    }

    //Function called when the old scene is done unloading
    //Needs those inputs to be added to the scene unloaded function
    public void OldSceneUnloaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= OldSceneUnloaded;

        //Start loading the new scene
        SceneManager.LoadSceneAsync(NEW_STOP_INFO.GetName(), LoadSceneMode.Additive);

        //When this scene is finished loading, fire off the destination loaded event.
        SceneManager.sceneLoaded += DestinationLoaded;
    }

    //Function called when the destination is done loading
    //Has to take those inputs to be a function added to the scene loaded function.
    public void DestinationLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= DestinationLoaded;

        //If we are going to player home find the farm manager.
        //Should be rewritten so we just call loadTempData and it is handled on that side.

        //SaveSystem.LoadTempData(NEW_STOP_INFO.GetName());

        if (NEW_STOP_INFO.GetName().Equals("PlayerHome"))
        {
            if (FMP == null)
            {
                FMP = GameManagerPointer.Instance.FARM_MANAGER_POINTER;
            }
            FMP.ENABLED = true;
            FMP.LOAD_ON_FIND = true;
        }
        else //If we aren't don't keep trying to find the manager.
        {
            if (FMP == null)
            {
                FMP = GameManagerPointer.Instance.FARM_MANAGER_POINTER;
            }
            FMP.ENABLED = false;
        }

        isSceneLoaded = true;
      
    }

    //Function that acts as though we are taking the bus to given location. Transition type denotes what transition scene to load.
    //First we figure out which location we are going to. The index of that corresponds to the other indexes.
    public void MoveTo(string DESTINATION)
    {
        //Find the index of the given location
        //int index = 0;
         int index = BSM.GetIndexOf(DESTINATION);
        //Debug.Log(index + DESTINATION);

        //Before replacing new stop info, use it to determine 
        //Save temp data if we need to
        Debug.Log(NEW_STOP_INFO.GetName());
        if (NEW_STOP_INFO.GetName().Equals("PlayerHome"))
        {
            Debug.Log("Save");
            if (FMP == null)
            {
                FMP = GameManagerPointer.Instance.FARM_MANAGER_POINTER;
            }
            FMP.FM.GetComponent<FarmManager>().SaveTempData();
        }
        SaveSystem.WriteTempData();


        //Get the stop info we want to go to
        NEW_STOP_INFO = BSM.STOP_LIST[index];

        //If index is -1, its not in the list and we have an error, else do the right thing.
        if(index != -1)
        {
            TRANSITION_SCENE = BSM.GetTransitionSceneName(NEW_STOP_INFO.TRANSITION);
            SceneManager.LoadSceneAsync(TRANSITION_SCENE, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += TransitionLoaded;
        }
        else
        {
            Debug.Log("ERROR: Invalid index. Location doesn't exist on list. Check spelling on bus terminal and in bus.");
        }
    }
}
