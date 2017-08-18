using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the plot under this manager.
//Gives utility functions to check adjacencies and other things.
public class PlotManager : MonoBehaviour {

    //Array of all elements in plot
    public GameObject[] plotBlocks;
    public GameObject[] testBlocks;

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
	
	// Update is called once per frame
	void Update () {

        

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

}
