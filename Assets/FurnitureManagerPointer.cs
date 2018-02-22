using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureManagerPointer : MonoBehaviour {

    public FurnitureManager FM;

    public bool ENABLED = false, LOAD_ON_FIND = false;

    // Update is called once per frame
    void Update()
    {

        if (FM == null && ENABLED)
        {
            //If we can find it
            if (GameObject.Find("FurnitureManager") != null)
            {
                FM = GameObject.Find("FurnitureManager").GetComponent<FurnitureManager>();
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
