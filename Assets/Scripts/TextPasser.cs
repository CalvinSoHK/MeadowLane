using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Passes the string its given to the right text UI object.
public class TextPasser : MonoBehaviour {

    public Text TEXT;

    public Image PROFILE_PIC;

    public Image NOTIFICATION;

    public List<string> MESSAGES;

    public void SetText(string MESSAGE)
    {
        TEXT.text = MESSAGE;
    }

    public void SetProfile(Sprite SPRITE)
    {
        PROFILE_PIC.sprite = SPRITE;
    }

    public void SetNotification(bool BOOL)
    {
        NOTIFICATION.gameObject.SetActive(BOOL);
    }

    public void CopyMessages(List<string> LIST)
    {
        foreach (string TEMP in LIST)
        {
            MESSAGES.Add(TEMP);
        }
    }
}
