using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Passes the string its given to the right text UI object.
public class TextPasser : MonoBehaviour {

    public Text TEXT;

    public void SetText(string MESSAGE)
    {
        TEXT.text = MESSAGE;
    }
}
