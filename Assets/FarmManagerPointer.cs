using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmManagerPointer : MonoBehaviour {

    public FarmManager FM;

    public bool ENABLED = false, LOAD_ON_FIND = false;

	// Update is called once per frame
	void Update () {
		
        if(FM == null && ENABLED)
        {
            //If we can find it
            if(GameObject.Find("FarmManager") != null)
            {
                FM = GameObject.Find("FarmManager").GetComponent<FarmManager>();
                ENABLED = false;
                if (LOAD_ON_FIND)
                {
                    SaveSystem.LoadTempData();
                    LOAD_ON_FIND = false;
                }
              
            }
        }
	}
}
