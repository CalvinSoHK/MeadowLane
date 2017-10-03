using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OnTriggerStay, when velocity reaches zero, attach objects inside to the trunk.
public class OnTriggerAttach : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        //If the other has a rigidbody,
        if(other.GetComponent<Rigidbody>() != null && other.GetComponent<InteractionPickup>() != null)
        {
            //If we are completely still
            if (other.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                other.GetComponent<InteractionPickup>().isAnchored = true;
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                other.transform.parent = transform;
            }
        }
    }
}
