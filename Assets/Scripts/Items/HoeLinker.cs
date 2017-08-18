using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//InfoLinker for hoe tools
public class HoeLinker : InfoLinker {

    //Velocity of the blade, where the hoe linker is attached.
    public float velocity;

    //Previous y position
    private float previousY = 0;

    //Timer for checks
    private float timer;

    //Time interval checks
    public float intervals = 0.25f;

    private void Update()
    {
        if(timer < 0)
        {
            velocity = (transform.position.y - previousY) * 100f;
            previousY = transform.position.y;

            //Debug.Log(velocity);
            timer = intervals;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        
    }
}
