using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Manages the locations of the map.
public class MapManager : MonoBehaviour {

    //List of all the location objects
    public List<LocationObj> LocationList = new List<LocationObj>();

    //Current location
    public LocationObj CURRENT = null;

    //Max distance
    public float MAX_DIFFERENCE = 20;

    //Function that sets current location
    public void SetLocation(GameObject location)
    {
        //Find corresponding obj in list
        foreach(LocationObj obj in LocationList)
        {
            if(obj.obj == location)
            {
                CURRENT = obj;
                return;
            }
        }
    }

    //Function that is called to check and remove locations, enables close locations as well.
    public void RemoveFar()
    {
        //Compare distances
        foreach(LocationObj obj in LocationList)
        {
            float comparison = Mathf.Abs(CURRENT.weight - obj.weight);
            if(comparison > MAX_DIFFERENCE)
            {
                obj.obj.SetActive(false);
            }
            else
            {
                obj.obj.SetActive(true);
            }
        }
    }

    //Custom obj that holds a gameobject location and a weight value.
    [System.Serializable]
    public class LocationObj
    {
        //GameObject
        public GameObject obj;

        //Weight value
        public float weight;
        
        //Create custom obj.
        public LocationObj(GameObject inObj, float inWeight)
        {
            obj = inObj;
            weight = inWeight;
        }
    }
}

//Custom editor for map manager
[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    //Target
    MapManager t;

    SerializedObject GetTarget;
    SerializedProperty List;
    int ListSize;

    //On enable, retrieve the list.
    private void OnEnable()
    {
        t = (MapManager)target;
        GetTarget = new SerializedObject(t);
        List = GetTarget.FindProperty("LocationList");
    }


    public override void OnInspectorGUI()
    {
        GetTarget.Update();

        //Display difference property first
        SerializedProperty diffRef = GetTarget.FindProperty("MAX_DIFFERENCE");
        EditorGUILayout.PropertyField(diffRef);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Set and show list size.
        ListSize = List.arraySize;
        ListSize = EditorGUILayout.IntField("List Size", ListSize);

        //If not equal, add or remove elements.
        if(ListSize != List.arraySize)
        {
            while(ListSize > List.arraySize)
            {
                List.InsertArrayElementAtIndex(List.arraySize);
            }
            while(ListSize < List.arraySize)
            {
                List.DeleteArrayElementAtIndex(List.arraySize - 1);
            }
        }

        //Display list to window
        for(int i = 0; i < List.arraySize; i++)
        {
            //Retrieve all properties.
            SerializedProperty ListRef = List.GetArrayElementAtIndex(i);
            SerializedProperty Obj = ListRef.FindPropertyRelative("obj");
            SerializedProperty Weight = ListRef.FindPropertyRelative("weight");

            //Default display type
            //EditorGUILayout.LabelField("Location List");
            EditorGUILayout.PropertyField(Obj);
            EditorGUILayout.PropertyField(Weight);

            EditorGUILayout.Space();
        }
      
        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();    
    }
}
