using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour {
    public GameObject wateringCan;
    bool ignoreCollision = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void callIgnoreCollision(GameObject theWateringCan)
    {
        Physics.IgnoreCollision(theWateringCan.GetComponent<Collider>(), GetComponent<Collider>());
        ignoreCollision = true;
    }
    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider.name);
        if (collision.gameObject.GetComponent<FarmBlockInfo>())
        {
            if (collision.gameObject.GetComponent<FarmBlockInfo>().waterCount <= collision.gameObject.GetComponent<FarmBlockInfo>().waterMax)
            {
                collision.gameObject.GetComponent<FarmBlockInfo>().waterCount += 10;
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponent<WaterDrop>())
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        else
        {
            if (ignoreCollision)
            {
                Destroy(gameObject);
            }
        }
    }
    public void OnCollisionStay(Collision collision)
    {
        //Debug.Log(collision.collider.name);
        if (collision.gameObject.GetComponent<FarmBlockInfo>())
        {
            if (collision.gameObject.GetComponent<FarmBlockInfo>().waterCount <= collision.gameObject.GetComponent<FarmBlockInfo>().waterMax)
            {
                collision.gameObject.GetComponent<FarmBlockInfo>().waterCount += 10;
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponent<WaterDrop>())
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
        else
        {
            if (ignoreCollision)
            {
                Destroy(gameObject);
            }
        }
    }
    
}
