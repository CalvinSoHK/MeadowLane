using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class GeneralStore : MonoBehaviour {

    public TextAsset storeItemReference;
    public string[] itemArray;
    //public string fileName;

	// Use this for initialization
	void Start () {
        if (storeItemReference != null)
        {
            itemArray = storeItemReference.text.Split(' ', '\n');
        }
               
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void sellItems(string fileName)
    {
        
    }
}
