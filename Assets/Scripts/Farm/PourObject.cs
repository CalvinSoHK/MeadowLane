using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionItem))]
public class PourObject : MonoBehaviour
{
    //This objects rotation, and the position we want to spawn objects
    private Vector3 OBJ_ROT, OBJ_SPAWN_POS;

    //The object we want to spawn, and the gameobject's transform we want to spawn objects on
    public GameObject SPAWN_OBJ, SPAWN_POINT;

    //The rotation of the object on spawn
    public Vector3 SPAWN_ROT;

    //Time between spawns, and the limits for triggering a spawn from rot up or down
    public float SPAWN_INTERVAL, ROT_UP_TRIGGER = 130, ROT_DOWN_TRIGGER = 40;

    //Internal spawn timer.
    private float SPAWN_TIMER;

    //Whether or not it should infinitely spawn the object
    public bool INFINITE = false;

    //Count of ammo if not infinite
    public int COUNT = 5;
  
    // Use this for initialization
    void Start()
    {
        resetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the rotation values
        OBJ_ROT = transform.eulerAngles;

        //The spawn pos is the SPAWN_POINT transform position
        OBJ_SPAWN_POS = SPAWN_POINT.transform.position;

        //objectRotation.z *= 100;
        //Check the rotation of the obj to see if we need to instantiate our object
        //315, 275
        if (OBJ_ROT.z < ROT_UP_TRIGGER && OBJ_ROT.z > ROT_DOWN_TRIGGER && GetComponent<InteractionItem>().isHeld && GetComponent<BaseItem>()._OWNER == BaseItem.Owner.Player)
        {
            if (getElapsedTime() > SPAWN_INTERVAL && COUNT > 0)
            {
                //Temp reference
                GameObject temp;

                //Spawn object
                temp = Instantiate(SPAWN_OBJ, OBJ_SPAWN_POS + new Vector3(0.015f, Random.Range(-0.045f, 0.015f), Random.Range(-0.025f, 0.035f)), Quaternion.Euler(SPAWN_ROT));

                //Ignore physics
                Physics.IgnoreCollision(GetComponent<Collider>(), temp.GetComponent<Collider>());

                //Reset spawn timer
                resetTimer();

                //Reduce count by 1
                COUNT--;
            }

        }
    }

    public void resetTimer()
    {
        SPAWN_TIMER = Time.time;
    }
    public float getElapsedTime()
    {
        return Time.time - SPAWN_TIMER;
    }
}
