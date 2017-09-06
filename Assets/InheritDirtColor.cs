using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherit color from the dirt and apply to this obj
public class InheritDirtColor : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(GetComponent<Renderer>().material != transform.parent.GetComponent<Renderer>().material)
        {
            GetComponent<Renderer>().material = transform.parent.GetComponent<Renderer>().material;
        }
	}
}
