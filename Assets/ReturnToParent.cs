using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Saves the parent at the beginning.
[RequireComponent(typeof(InteractionItem))]
public class ReturnToParent : MonoBehaviour {

    //Whether or not this is enabled
    public bool ENABLED = true;

    //Transform of parent in the beginning
    public Transform OG_PARENT;

    //InteractionItem, check if its held.
    InteractionItem ITEM;

    //Original position and rot
    Vector3 OG_POS;
    Quaternion OG_ROT;

    //Time to  wait when let go of to return to parent
    public float Wait_Time = 5f;

    //Internal bool to see if coroutine is running
    private bool RUNNING = false;

    void Start()
    {
        OG_PARENT = transform.parent;
        ITEM = GetComponent<InteractionItem>();
        OG_POS = transform.localPosition;
        OG_ROT = transform.localRotation;
    }

    void Update()
    {
        if (ENABLED)
        {
            if (!ITEM.isHeld && !RUNNING && transform.parent != OG_PARENT)
            {
                StartCoroutine(ReturnAfterDelay(Wait_Time));
                RUNNING = true;
            }
        }

        if(ITEM.isHeld && RUNNING)
        {
            RUNNING = false;
        }
    }

    IEnumerator ReturnAfterDelay(float s)
    {
        float timer = 0;
        while(timer < s)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (!RUNNING)
            {
                timer = s;
            }
        }
       
        if (!ITEM.isHeld && RUNNING)
        {
            transform.parent = OG_PARENT;
            transform.localPosition = OG_POS;
            transform.localRotation = OG_ROT;
        }
        RUNNING = false;
    }
}
