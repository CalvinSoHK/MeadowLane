using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OnTriggerStay, when velocity reaches zero, attach objects inside to the trunk.
public class OnTriggerAttach : MonoBehaviour {

    //Whether or not we need to disable ReturnToParent
    public bool RET_TO_PAR = false;

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

            //If it has ReturnToParent, if RET_TO_PAR is false, remove it
            if(other.GetComponent<ReturnToParent>() != null && !RET_TO_PAR)
            {
                Destroy(other.GetComponent<ReturnToParent>());
            }
        }
    }
}
