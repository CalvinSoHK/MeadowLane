using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that resides on the gameobject parent of all of the player's furniture in the house.
/// Can be edited by the player and needs to be saved. 
/// </summary>
public class FurnitureManager : MonoBehaviour {

    GameObject LOADER;

    string[] INPUT, LINE, VECTOR;
    Vector3 POS, ROT;

    public void SaveData()
    {
        string DATA = "";

        //Get all furniture under us
        BaseItem[] FURNITURE = transform.GetComponentsInChildren<BaseItem>();

        //For all of our furniture, save it to string
        for(int i = 0; i < FURNITURE.Length; i++)
        {
            DATA += FURNITURE[i].CATEGORY + "/" + FURNITURE[i]._NAME + " " + 
                transform.position.x + "," + transform.position.y + "," + transform.position.z + " " + 
                transform.eulerAngles.x + "," + transform.eulerAngles.y + "," + transform.eulerAngles.z + "\n";
        }

        SaveSystem.SaveTo(SaveSystem.SaveType.Decoration, "/Decoration\n" + DATA + "/\n");
    }

    public void SaveTempData()
    {
        string DATA = "";

        //Get all furniture under us
        BaseItem[] FURNITURE = transform.GetComponentsInChildren<BaseItem>();

        //For all of our furniture, save it to string
        for (int i = 0; i < FURNITURE.Length; i++)
        {
            DATA += FURNITURE[i].CATEGORY + "/" + FURNITURE[i]._NAME + " " +
                transform.position.x + "," + transform.position.y + "," + transform.position.z + " " +
                transform.eulerAngles.x + "," + transform.eulerAngles.y + "," + transform.eulerAngles.z + "\n";
        }

        SaveSystem.SaveTo(SaveSystem.TempType.Decoration, "/Decoration\n" + DATA + "/\n");
    }

    public void LoadData(string DATA)
    {
        INPUT = DATA.Split('\n');

        for(int i = 0; i < INPUT.Length; i++)
        {
            //Split the line up by spaces
            LINE = INPUT[i].Split(' ');

            //Split the pos vector and make it
            VECTOR = LINE[1].Split(',');
            POS = new Vector3(float.Parse(VECTOR[0]), float.Parse(VECTOR[1]), float.Parse(VECTOR[2]));

            //Split the rot vector and make it
            VECTOR = LINE[2].Split(',');
            ROT = new Vector3(float.Parse(VECTOR[0]), float.Parse(VECTOR[1]), float.Parse(VECTOR[2]));

            //Spawn the object in the right position and rot
            LOADER = Instantiate(Resources.Load(LINE[0], typeof(GameObject)) as GameObject, POS, Quaternion.Euler(ROT),  transform);            
        }
    }
}
