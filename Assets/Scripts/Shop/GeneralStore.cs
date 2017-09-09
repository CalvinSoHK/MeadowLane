using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class GeneralStore : MonoBehaviour {

    public TextAsset storeItemReference;
    public string[] itemArray;
    public int storeCredit;
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collider"))
        {
            Container tempContainer = collision.gameObject.GetComponent<Container>();
            //the container shouldn't be able to be picked up anymore.
            collision.gameObject.GetComponent<InteractionPickup>().enabled = false;
        }
    }
    void sellItems(string fileName)
    {
        
    }
}
