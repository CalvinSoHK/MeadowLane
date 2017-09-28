using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//On use, swap two objects positions with each other. 
public class InteractionSwapObjects : InteractableCustom{

    //Swaps these two objects on use
    public Transform obj1, obj2;

    //Override use key to swap both objects
    public override void Use(Hand hand)
    {
        StartCoroutine(SwapAfter(2));
    }

    //Use a coroutine (temp fix) to swqap after a delay, s seconds
    public IEnumerator SwapAfter(int s)
    {
            yield return new WaitForSeconds(s);
            //Holder variables so we can swap the two
            Transform tempTransform = obj1.parent;
            Vector3 tempPosition = obj1.localPosition;
            Quaternion tempRotation = obj1.localRotation;

            //Swap obj1 into obj2
            obj1.parent = obj2.parent;
            obj1.localPosition = obj2.localPosition;
            obj1.localRotation = obj2.localRotation;

            //Swap obj2 into temp
            obj2.parent = tempTransform;
            obj2.localPosition = tempPosition;
            obj2.localRotation = tempRotation;

    }
}
