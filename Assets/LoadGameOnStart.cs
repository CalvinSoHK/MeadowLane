using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Loads our game on start
public class LoadGameOnStart : MonoBehaviour {

   public void LoadGame()
    {
        SaveSystem.LoadData();
    }
}
