using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just poinst to the player
public class PlayerPointer : MonoBehaviour {

    public GameObject PLAYER;

    private void Awake()
    {
        PLAYER = GameObject.Find("Player");
    }

}
