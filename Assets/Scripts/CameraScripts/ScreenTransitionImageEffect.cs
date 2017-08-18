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

    [Range(0,1.5f)]
    public float maskValue;
    public Color maskColor = Color.black;
    public Texture2D maskTexture;
    public bool maskInvert, runEffect = false;
    public float openSpeedModifier = 1,
        closeSpeedModifier = 4,
        openTimeBetween = 2,
        closeTimeBetween = 4;

    public Transform destination = null, player = null;

    private Material m_Material;
    private bool m_maskInvert;

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
        if (runEffect)
        {
            time = 0.0f;
            setCurrentState(Gamestate.close);
        }
        switch (currentState)
        {
            case Gamestate.wait:
                time = 0.0f;
                break;
            case Gamestate.close:
                runEffect = false;
                time += Time.deltaTime / closeSpeedModifier;
                maskValue = Mathf.SmoothStep(0.77f, 1.5f, time);
                if (getStateElapsed() > closeTimeBetween)
                {
                    setCurrentState(Gamestate.open);
                    player.transform.position = destination.position;
                    player.transform.eulerAngles += new Vector3(0, 180, 0);
                    time = 0.0f;
                }
                break;
            case Gamestate.open:
                time += Time.deltaTime / openSpeedModifier;
                maskValue = Mathf.SmoothStep(1.5f, 0.0f, time);
                if (getStateElapsed() > openTimeBetween)
                {
                    setCurrentState(Gamestate.wait);
                    destination = null;
                    player = null;
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


    public void setCurrentState(Gamestate state)
    {
        currentState = state;
        lastStateChange = Time.time;
    }

    float getStateElapsed()
    {
        return Time.time - lastStateChange;
    }

    public void MovePlayer(Transform location, Transform playerT)
    {
        destination = location;
        player = playerT;
        runEffect = true;
    }
}
