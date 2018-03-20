using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChefDialogue : MonoBehaviour {
    public string[] ingredients;
    public DisplayDialogue chef;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("debug o chef dialogue was pressed");
            chef.ActivateChefDialogue(ingredients);
        }
	}
}
