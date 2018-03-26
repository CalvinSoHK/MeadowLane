using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node:IHeapItem<Node> {
    public bool walkable; //could an NPC walk on this Node
    public Vector3 worldPosition; //reference to the node position

    public int gCost; //how far from starting Node
    public int hCost; // how far from target Node

    public int gridX; //X position of the Node in the grid
    public int gridY; //Y position of the Node in the grid
    public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;

        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
