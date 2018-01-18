using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Bus controller script. Places the bus at key locations when given a location and moves in a direction.
//NOTE: Script doesnt work if the target is Vector3.zero.
public class Bus_Controller : MonoBehaviour {

    //List of the locations we can go to
    public List<string> LocationList;
    public List<Vector3> InitialPositions_List;
    public List<Vector3> FinalPositions_List;

    //Bus's movement speed
    float MAX_SPEED = 5f;
    float CURRENT_SPEED;

    //Target location
    Vector3 TARGET = Vector3.zero, REF_VELOCITY
       ,DIR_TO;

    void Update()
    {

        if(TARGET != Vector3.zero)
        {
            transform.position = Vector3.SmoothDamp(transform.position, transform.position + DIR_TO * CURRENT_SPEED, ref REF_VELOCITY, 1f);
            transform.LookAt(TARGET);
            //When we are within one meter of the target, stop moving.
            if ((transform.position - TARGET).magnitude < 1){
                TARGET = Vector3.zero;
            }
            else if((transform.position - TARGET).magnitude < MAX_SPEED * 5)
            {
                CURRENT_SPEED = (transform.position - TARGET).magnitude / 5;
            }
            else
            {
                CURRENT_SPEED = MAX_SPEED;
            }
        }

       
    }

    //Function that acts as though we are taking the bus to given location.
    //First we figure out which location we are going to. The index of that corresponds to the other indexes.
    public void MoveTo(string HOME, string LOCATION)
    {
        //Find the index of the given location
        int index = LocationList.IndexOf(LOCATION);

        //If index is -1, its not in the list and we have an error, else do the right thing.
        if(index != -1)
        {
            transform.position = InitialPositions_List[index];
            TARGET = FinalPositions_List[index];
            DIR_TO = (FinalPositions_List[index] - InitialPositions_List[index]).normalized;
        }
        else
        {
            Debug.Log("ERROR: Invalid index. Location doesn't exist on list. Check spelling on bus terminal and in bus.");
        }
    }
}
