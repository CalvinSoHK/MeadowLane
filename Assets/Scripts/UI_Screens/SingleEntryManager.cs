using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages a single entry and what it is displaying
public class SingleEntryManager : MonoBehaviour {

    //Holds all the textboxes we should manage
    public Text COUNT_TEXT, NAME_TEXT, PRICE_TEXT;

    //Statistics to keep track of
    [HideInInspector]
    public int COUNT, PRICE, CONTAINER_COUNT;
    public string NAME;

    //Bool to change the values inside or not
    public bool CHANGE_PROPERTIES = true;

	// Update is called once per frame
	void Update () {
        
        //if it isn't null, update everything
		if(COUNT_TEXT != null)
        {
            //If our count is greater than zero apply it
            if(COUNT > 0)
            {
                COUNT_TEXT.text = " " + COUNT;
            }         
        }
        if (CHANGE_PROPERTIES)
        {
            if (PRICE_TEXT != null)
            {
                PRICE_TEXT.text = " " + PRICE;
            }

            if (NAME_TEXT != null)
            {
                NAME_TEXT.text = " " + NAME;
            }
        }
       
	}
}
