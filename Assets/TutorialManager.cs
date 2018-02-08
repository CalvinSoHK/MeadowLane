using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Stores all the bools for all tutorial phases
public class TutorialManager : MonoBehaviour {

    //Array that stores whether or not a tutorial has been done
    public TutorialInfo[] TUTORIAL_ARRAY;

    //Helper function to check if a given name is true
    public bool IsComplete(string TUT_NAME)
    {
        for(int i = 0; i < TUTORIAL_ARRAY.Length; i++)
        {
            if (TUTORIAL_ARRAY[i].NAME.Trim().Equals(TUT_NAME.Trim()))
            {
                return TUTORIAL_ARRAY[i].COMPLETE;
            }
        }
        return false;
    }

    //Helper function that sets a given tutorial value to true
    public void SetComplete(string TUT_NAME)
    {
        for (int i = 0; i < TUTORIAL_ARRAY.Length; i++)
        {
            if (TUTORIAL_ARRAY[i].NAME.Trim().Equals(TUT_NAME.Trim()))
            {
                TUTORIAL_ARRAY[i].COMPLETE = true;
            }
        }
    }

    //Function that saves this to a text file
    public void SaveTutValues()
    {

    }

}

[System.Serializable]
public class TutorialInfo
{
    public bool COMPLETE = false;
    public string NAME = "";
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TutorialInfo))]
public class TutorialInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //Indent 
        var indent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel++;

        //Calculate rects
        var NAME_RECT = new Rect(position.x, position.y, 200, position.height);
        var COMPLETE_RECT = new Rect(position.x + 200, position.y, 30, position.height);

        //Make labels
        GUIContent NAME_LABEL = new GUIContent("Name", "Name of the tutorial.");
        //GUIContent COMPLETE_LABEL = new GUIContent("Complete", "Whether or not this tutorial has been done.");

        //Draw fields
        EditorGUI.PropertyField(NAME_RECT, property.FindPropertyRelative("NAME"), NAME_LABEL);
        EditorGUI.PropertyField(COMPLETE_RECT, property.FindPropertyRelative("COMPLETE"), GUIContent.none);

        //Go back to previous indent
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif