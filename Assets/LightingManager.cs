using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Plays with our lighting
public class LightingManager : MonoBehaviour {

    public AdjustToTime SUN;

    //Looks for a sun in the world if its possible
    private void Update()
    {
        if(SUN == null)
        {
            //Debug.Log("Looking for sun since it is null.");
            //SUN = GameObject.Find("Sun").GetComponent<AdjustToTime>();
            if (GameObject.Find("Sun") != null)
            {
                Debug.Log("Assigning array");
                SUN = GameObject.Find("Sun").GetComponent<AdjustToTime>();
            }
        }    
    }

    public void UpdateLight()
    {
        if(SUN != null)
        {
            SUN.ChangeRotation();
        }     
    }
}
