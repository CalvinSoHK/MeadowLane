using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageNotification : MonoBehaviour {

    public Image notificationImage;
    Color tmp;
    float speed = 3, startTime, journeyLength, alpha;
    // Use this for initialization
    void Start () {
        startTime = Time.time;
        journeyLength = 1;
	}
	
	// Update is called once per frame
	void Update () {
        tmp = notificationImage.color;
        float distanceCovered = (Time.time - startTime) * speed; //checks the distance covered based on time (and speed)
        float fracJourney = distanceCovered / journeyLength; //remainder of the journey
        fracJourney = 1f - Mathf.Cos(fracJourney * Mathf.PI * 0.5f); //set it to use the cos function
        alpha = Mathf.Lerp(0.0f, 1.0f, fracJourney); //lerp the movement of the enemy using frac journey
        tmp.a = alpha;
        //Debug.Log(alpha);
        notificationImage.color = tmp;
    }
}
