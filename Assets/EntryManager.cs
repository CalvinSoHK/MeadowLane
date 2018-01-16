using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages a single entry and what it is displaying
public class EntryManager : MonoBehaviour {

    //Holds all the textboxes we should manage
    public Text COUNT_TEXT, NAME_TEXT, PRICE_TEXT;

    //Statistics to keep track of
    public int COUNT, PRICE;
    public string NAME;

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

        if(PRICE_TEXT != null)
        {
            PRICE_TEXT.text = " " + PRICE;
        }

        if(NAME_TEXT != null)
        {
            NAME_TEXT.text = " " + NAME;
        }
	}
}
