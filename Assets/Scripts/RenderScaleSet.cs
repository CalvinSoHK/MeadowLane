using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class RenderScaleSet : MonoBehaviour
{
    [SerializeField] private float m_RenderScale = 1.5f;              
    //The render scale. Higher numbers = better quality, but trades performance

    void Start()
    {
        VRSettings.renderScale = m_RenderScale;
    }
}
