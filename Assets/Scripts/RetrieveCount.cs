using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Gets count
public class RetrieveCount : MonoBehaviour {

    public PourObject POUR_OBJECT_SCRIPT;

	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = POUR_OBJECT_SCRIPT.COUNT + "";
	}
}
