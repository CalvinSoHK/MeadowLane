using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// On trigger enter fire the given event
/// </summary>
public class OnTriggerEnterFire : MonoBehaviour {

    public UnityEvent EVENT;

    private void OnTriggerEnter(Collider other)
    {
        EVENT.Invoke();
    }
}
