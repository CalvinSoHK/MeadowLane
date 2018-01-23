using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Bus controller script. Places the bus at key locations when given a location and moves in a direction.
//NOTE: Script doesnt work if the target is Vector3.zero.
public class Bus_Controller : MonoBehaviour {

    //List of the locations we can go to
    public List<string> LocationList;
    public List<Scene> Transition_List;
    public List<Vector3> FinalPositions_List;

    //Bus's movement speed
    float MAX_SPEED = 5f;
    float CURRENT_SPEED;

    //Target location
    Vector3 DESTINATION_POS= Vector3.zero;

    void Update()
    {

       
       
    }

    //Function that acts as though we are taking the bus to given location. Transition type denotes
    //First we figure out which location we are going to. The index of that corresponds to the other indexes.
    public void MoveTo(string DESTINATION, BusEntryManager.TransitionType TRANSITION)
    {
        //Find the index of the given location
        int index = LocationList.IndexOf(DESTINATION);

        //If index is -1, its not in the list and we have an error, else do the right thing.
        if(index != -1)
        {
            DESTINATION_POS = FinalPositions_List[index];
            SceneManager.LoadScene(Transition_List[index].name, LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("ERROR: Invalid index. Location doesn't exist on list. Check spelling on bus terminal and in bus.");
        }
    }
}
