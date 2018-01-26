using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just poinst to the player
public class PlayerPointer : MonoBehaviour {

    public GameObject PLAYER;

    //Singleton code.
    private static PlayerPointer _instance;

    public static PlayerPointer Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        //If there is already an instance and it isnt this.
        if (_instance != null && _instance != this)
        {         
            Destroy(gameObject);
        }
        else //If there isn't an instance OR it is this.
        {
            _instance = this;
            PLAYER = GameObject.Find("Player");
        }
    }

}
