using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the plot under this manager.
//Gives utility functions to check adjacencies and other things.
public class PlotManager : MonoBehaviour {

    //Array of all elements in plot
    public GameObject[] plotBlocks;
    //public GameObject[] testBlocks;

    //Used many times
    Vector3 TEMP;
    string[] VECTOR;

    //Size of the given plot
    public int length = 0;

	// Use this for initialization
	void Start () {

        //Set size of array to number of children.
        plotBlocks = new GameObject[transform.childCount];

        //Fill the array.
        for(int i = 0; i < transform.childCount; i++)
        {
            plotBlocks[i] = transform.GetChild(i).gameObject;
        }

        //For now, all plots are squares, and so the sqrt of the length of array is 
        //length of square.
        length = (int)Mathf.Sqrt(plotBlocks.Length);
	}
	
    //Gives us the string save info for the given block
    public string GetSaveInfo(GameObject BLOCK)
    {
        string DATA = "";
        FarmBlockInfo FBI = BLOCK.GetComponent<FarmBlockInfo>();
      
        if(FBI.PLANT != null && !FBI.DEAD)
        {
            PlantBase PB = FBI.PLANT.GetComponent<PlantBase>();
            DATA += PB.NAME + " " + FBI.TILLED + " " + PB.TIME_TO_NEXT + " " + PB.GetProduceNumber() + " " + PB.DAYS_TO_DIE + " " + 
                PB.transform.localScale.x + "," + PB.transform.localScale.y + "," + PB.transform.localScale.z + " " +
                PB.transform.eulerAngles.x + "," + PB.transform.eulerAngles.y + "," + PB.transform.eulerAngles.z + "\n";
        }
        else if (FBI.DEAD)
        {
            //Will always load default dead dirt mound. Won't use other prefabs.
            DATA += "DEAD\n";
        }
        else
        {
            DATA += "EMPTY\n";
        }
        return DATA;
    }

    //Should set values based on the given string at the given X,Y coordinate
    //Doesn't account for, random sizing, random rotations
    public void SetBlockTo(string DATA, int INDEX)
    {
        string[] INPUT = DATA.Split(' ');
        //Debug.Log(gameObject + " " + INDEX + " " + DATA);
        FarmBlockInfo FBI = plotBlocks[INDEX].GetComponent<FarmBlockInfo>();
        FBI.TILLED = bool.Parse(INPUT[1]);

        //Extract rotation and instantiate it with that rotation
        VECTOR = INPUT[6].Split(',');
        TEMP = new Vector3(float.Parse(VECTOR[0]), float.Parse(VECTOR[1]), float.Parse(VECTOR[2]));

        //Instantiate the plant and save its referene to the script.
        //Debug.Log(INPUT[0]);
        PlantBase PB = (Instantiate(Resources.Load("Plants/" + INPUT[0],  typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.Euler(TEMP), FBI.transform).GetComponent<PlantBase>());
        PB.PlantObj(PB.gameObject, FBI.transform);

        //Change the scale of the object
        VECTOR = INPUT[5].Split(',');
        TEMP = new Vector3(float.Parse(VECTOR[0]), float.Parse(VECTOR[1]), float.Parse(VECTOR[2]));
        PB.transform.localScale = TEMP;
        TEMP = Vector3.zero;

        //Assign all important variables, then actually init the plant so it has our values applied.
        PB.TIME_TO_NEXT = int.Parse(INPUT[2]);
        PB.PRODUCE_NUMBER = int.Parse(INPUT[3]);
        PB.DAYS_TO_DIE = int.Parse(INPUT[4]);
        PB.Init();
        
    }

    //Retrieves a block given coordinates from the array
    //Null if no possible block.
    GameObject RetrieveBlock(Vector2 coordinate)
    {
        GameObject returnObj = null;

        for(int i = 0; i < plotBlocks.Length; i++)
        {
            if(plotBlocks[i].GetComponent<FarmBlockInfo>().coordinate == coordinate)
            {
                returnObj = plotBlocks[i];
            }
        }
        return returnObj;
    }

    //Returns all adjacent blocks to a block. 
    //If true, gives diagonals as well. Else no diagonals.
    GameObject[] GetAdjacentBlocks(GameObject block, bool diagonals)
    {
        //Init return variable
        List<GameObject> blocks = new List<GameObject>();

        //For all viable coordinates, add the block to the array.
        Vector2 origin = block.GetComponent<FarmBlockInfo>().coordinate;
        
        //If the next north coordinate is within the max.
        if(origin.y + 1 < length)
        {
            blocks.Add(RetrieveBlock(origin + new Vector2(0, 1)));
        }
        //If the next south coordinate is within the min.
        if(origin.y - 1 >= 0)
        {
            blocks.Add(RetrieveBlock(origin + new Vector2(0, -1)));
        }
        //If the next east coordinate is within the max.
        if(origin.x + 1 < length)
        {
            blocks.Add(RetrieveBlock(origin + new Vector2(1, 0)));
        }
        //If the next west coordiante is within the min.
        if(origin.x - 1 >= 0)
        {
            blocks.Add(RetrieveBlock(origin + new Vector2(-1, 0)));
        }

        if (diagonals)
        {
            //If the next north coordinate is within the max.
            if (origin.y + 1 < length && origin.x + 1 < length)
            {
                blocks.Add(RetrieveBlock(origin + new Vector2(1, 1)));
            }
            //If the next south coordinate is within the min.
            if (origin.y - 1 >= 0 && origin.x + 1 < length)
            {
                blocks.Add(RetrieveBlock(origin + new Vector2(1, -1)));
            }
            //If the next north coordinate is within the max.
            if (origin.y + 1 < length && origin.x - 1 >= 0)
            {
                blocks.Add(RetrieveBlock(origin + new Vector2(-1, 1)));
            }
            //If the next south coordinate is within the min.
            if (origin.y - 1 >= 0 && origin.x - 1 >= 0)
            {
                blocks.Add(RetrieveBlock(origin + new Vector2(-1, -1)));
            }
        }

        GameObject[] returnArray = blocks.ToArray();

        return returnArray;
    }


    //Call day-end function if valid from all farm blocks
    public void DayEnd()
    {
        //Go through all plotBlocks and call the dayend function if it exists
        for(int i = 0; i < plotBlocks.Length; i++)
        {
            if (plotBlocks[i].transform.GetComponent<FarmBlockInfo>())
            {
                plotBlocks[i].transform.GetComponent<FarmBlockInfo>().DayEnd();
            }
        }
    }
}

