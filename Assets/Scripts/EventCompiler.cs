using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Compiles all events in our events folder into a series of text files.
/// </summary>
public class EventCompiler : MonoBehaviour {

    GameObject[] allPrefabEvents;
    EventClass[] allEventClass;
    Dictionary<int, List<EventClass>> SceneSpecificEventRef = new Dictionary<int, List<EventClass>>();
    Dictionary<int, EventClass> MainEventsForScenes = new Dictionary<int, EventClass>();


    // Use this for initialization
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void setTheEventTextFile()
    {
        allPrefabEvents = Resources.LoadAll("ENTER THE RELEVANT PATH", typeof(GameObject)) as GameObject[];//get all the prefab event game objects
        InitEventClass();
        OrguanizeSceneSpecificDictionary();


        /*FileStream FS = File.Open("ENTER THE RELEVANT PATH", FileMode.Create);

        using (StreamWriter SW = new StreamWriter(FS))
        {
            for(int i = 0; i < allPrefabEvents.Length; i++)
            {

            }

        }*/

    }

    /// <summary>
    /// Initialize the event class array to assign all of the prefabs' event class script into the event class array
    /// </summary>
    public void InitEventClass()
    {
        allEventClass = new EventClass[allPrefabEvents.Length]; //initialize the event class array
        for (int i = 0; i < allPrefabEvents.Length; i++) //assign all of the prefabs' event class script into the event class array
        {
            allEventClass[i] = allPrefabEvents[i].GetComponent<EventClass>();
        }
    }

    /// <summary>
    /// Finds all the events and orguanizes them by scene
    /// </summary>
    public void OrguanizeSceneSpecificDictionary()
    {
        for (int i = 0; i < allEventClass.Length; i++) //goes through all the events
        {
            if (SceneSpecificEventRef.ContainsKey(allEventClass[i].SCENE)) //checks if this the scene in which the current event happens has been accounted for
            {
                SceneSpecificEventRef.Add(allEventClass[i].SCENE, new List<EventClass>()); //create a new list within the dictionary with that scene as a key
                SceneSpecificEventRef[allEventClass[i].SCENE].Add(allEventClass[i]); //add the even to the list
            }
            else
            {
                SceneSpecificEventRef[allEventClass[i].SCENE].Add(allEventClass[i]); //just add the event to the already existing list for that scene
            }
        }
    }

    /// <summary>
    /// gets the most important event for each type within all individual locations
    /// </summary>
    public void GetMainEventsForScenes()
    {
        List<int> keyList = new List<int>(this.SceneSpecificEventRef.Keys); //gets a ref to all the keys from the dict that has all the events orguanized by scene
        for(int i = 0; i < keyList.Count; i++) //go through all the keys we have
        {
            for(int j = 0; j < SceneSpecificEventRef[keyList[i]].Count; j++) //nested loop to go through all the list for those dict keys
            {
                if (MainEventsForScenes.ContainsKey((int)SceneSpecificEventRef[keyList[i]][j].TYPE)) //if the this event is not in the impo dict then add it
                {
                    MainEventsForScenes[(int)SceneSpecificEventRef[keyList[i]][j].TYPE] = SceneSpecificEventRef[keyList[i]][j];
                }else //if there is already an item in the dict, we need to check if this new item is more important than the current one
                {
                    if(SceneSpecificEventRef[keyList[i]][j].OVERRIDEABLE == EventClass.OVERRIDEABLE_TYPE.NonOverrideable && MainEventsForScenes[(int)SceneSpecificEventRef[keyList[i]][j].TYPE].OVERRIDEABLE == EventClass.OVERRIDEABLE_TYPE.NonOverrideable) //if both events are not overridable
                    {
                        Debug.Log("ERROR, two non overridable events of the same type in same location: " + SceneSpecificEventRef[keyList[i]][j].DAY + " Day " + SceneSpecificEventRef[keyList[i]][j].STARTMONTH + " Month " + SceneSpecificEventRef[keyList[i]][j].SCENE + " SCENE"); //that should not happen, debug the error.
                    }
                }
            }
        }
    }
}
