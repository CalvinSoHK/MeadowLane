using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Draws our custom property enumtag.
[CustomPropertyDrawer(typeof(EnumTag))]
public class EnumTagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position,
            _label, 
            _property.intValue, 
            _property.enumNames);
    }
}