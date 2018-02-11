using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Screen Transition")]
public class ScreenTransitionImageEffect : MonoBehaviour
{
    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    public Shader shader;
    float time, whenEyesClosed, lastStateChange;
    public enum Gamestate{ wait, open, close };
    public Gamestate currentState;

    //We set the range of the maskValue starting at 0 and going up to 1.5
    [Range(0,1.5f)]
    public float maskValue;
    public Color maskColor = Color.black;
    public Texture2D maskTexture;
    public bool maskInvert, runEffect = false;
    public float openSpeedModifier = 4f,
        closeSpeedModifier = 4f,
        openTimeBetween = 2f,
        closeTimeBetween = 2f;

    public Transform destination = null, player = null;

    private Material m_Material;
    private bool m_maskInvert;

    //Whether or not it is a fixed travel
    private bool isFixed;

    //Whether or not we want to flip the camera on travel. 180 rotation.
    private bool isFlipped;

    //Whether or not we want to anchor
    private bool isAnchored = false;

    //Public bools for other scripts to reference
    public bool isOpen, isClosed;


    Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }

    void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        shader = Shader.Find("Hidden/ScreenTransitionImageEffect");

        // Disable the image effect if the shader can't
        // run on the users graphics card
        if (shader == null || !shader.isSupported)
            enabled = false;

        //StartCoroutine(closeEye());
        
    }

    private void Update()
    {
        //we check if we need to start the event system and fade the screen out to teleport the player
        if (runEffect)
        {
            time = 0.0f;
            setCurrentState(Gamestate.close);
        }

        //We need a state system to know when to fade in and out when player needs to be telported into rooms
        switch (currentState)
        {
            case Gamestate.wait:
                time = 0.0f;
                isOpen = false;
                break;
            case Gamestate.close:
                runEffect = false;
                time += Time.deltaTime / closeSpeedModifier;
                maskValue = Mathf.SmoothStep(0.77f, 1.5f, time);
                if (getStateElapsed() > closeTimeBetween)
                {
                    isClosed = true;
                    setCurrentState(Gamestate.open);
                    System.GC.Collect();
                    player.transform.position = destination.position;
                    if (isFlipped)
                    {
                        player.transform.eulerAngles += new Vector3(0, 180, 0);
                    }           
                    if (isFixed)
                    {
                        //Get the actual player
                        Transform playerObject = player.GetChild(0).Find("VRCamera");

                        //Move the player based on the local position of the playerObject
                        player.transform.position -= new Vector3(
                            playerObject.transform.localPosition.x,
                            0,
                            playerObject.transform.localPosition.z);

                    }
                    if (isAnchored)
                    {
                        player.transform.parent = destination;
                    }
                    else
                    {
                        player.transform.parent = null;
                    }
                    time = 0.0f;
                }
                break;
            case Gamestate.open:
                isClosed = false;
                time += Time.deltaTime / openSpeedModifier;
                maskValue = Mathf.SmoothStep(1.5f, 0.0f, time);
                if (getStateElapsed() > openTimeBetween)
                {
                    isOpen = true;
                    setCurrentState(Gamestate.wait);
                    //destination = null;
                    //player = null;
                }
                break;
        }
    }
    void OnDisable()
    {
        if (m_Material)
        {
            DestroyImmediate(m_Material);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!enabled)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        material.SetColor("_MaskColor", maskColor);
        material.SetFloat("_MaskValue", maskValue);
        material.SetTexture("_MainTex", source);
        material.SetTexture("_MaskTex", maskTexture);

        if (material.IsKeywordEnabled("INVERT_MASK") != maskInvert)
        {
            if (maskInvert)
                material.EnableKeyword("INVERT_MASK");
            else
                material.DisableKeyword("INVERT_MASK");
        }

        Graphics.Blit(source, destination, material);
    }

    //We set what state the camera needs to be in, and update the time since the last update
    public void setCurrentState(Gamestate state)
    {
        currentState = state;
        lastStateChange = Time.time;
    }
    //We check how much time has passed since the last update
    float getStateElapsed()
    {
        return Time.time - lastStateChange;
    }

    //Getter function for state
    public Gamestate GetCurrentState()
    {
        return currentState;
    }

    //We move the player to the new location
    public void MovePlayer(Transform location, Transform playerT, bool isFixedT, bool isAnchoredT)
    {
        destination = location;
        player = playerT;
        isFixed = isFixedT;
        isAnchored = isAnchoredT;
        runEffect = true;
    }

    //Blink eyes
    //This is going to move us to the same location as last time. Should be no problem for most cases.
    public void BlinkEyes()
    {
        runEffect = true;
    }

    //End the day
    public void EndDay(Transform playerT)
    {
        player = playerT;
        destination = player;
        isFixed = false;
        isFlipped = false;
        runEffect = true;

    }
}
