using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller on our travel point to provide better control
public class TravelPointController : MonoBehaviour {

    //Whether or not the travel point is valid.
    public bool isValid = true;

    //The object layer we have to manipulate
    public GameObject TARGET;

    //Travel point manager

    private void Awake()
    {
        GameManagerPointer.Instance.TRAVEL_POINT_MANAGER.TRAVEL_POINT_LIST.Add(this);
    }


    // Update is called once per frame
    void Update () {
        if(TARGET != null)
        {
            if (isValid && TARGET.layer != 8)
            {
                TARGET.layer = 8;
            }
            else if (!isValid && TARGET.layer == 8)
            {
                TARGET.layer = 0;
            }
        }
     
	}
}
