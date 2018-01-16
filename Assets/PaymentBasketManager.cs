using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaymentBasketManager : MonoBehaviour {

    //All objects in this basket
    List<GameObject> OBJECT_LIST;

    void OnTriggerEnter(Collider other)
    {
        //If we are a base item.
        if(other.GetComponent<BaseItem>() != null)
        {

        }
    }
}
