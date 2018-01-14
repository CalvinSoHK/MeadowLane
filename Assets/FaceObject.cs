using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script that just causes this object to face the given object
public class FaceObject : MonoBehaviour {

    //Object to face
    public Transform TARGET;

    void Update()
    {
        if(TARGET != null)
        {
            transform.LookAt(TARGET);
        }
       
    }

}
