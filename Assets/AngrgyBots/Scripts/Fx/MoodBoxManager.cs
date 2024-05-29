using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
// about the MoodBox (tm) system
//
// MoodBoxManager, MoodBox and MoodBoxData and friends are being used to enable
// local bloom, noise, color correction, etc.
//
// The player triggers MoodBoxes by walking through them, so they are placed strategically
// at entrances and exits. custom effect values will then be interpolated each frame and 
// given to (image) effects and shaders.
[UnityEngine.ExecuteInEditMode]
public partial class MoodBoxManager : MonoBehaviour
{
    public static MoodBox current;
    public MoodBoxData currentData;
    public MobileBloom bloom;
    public PostProcessVolume postProcessVolume;
    public ColoredNoise noise;
    public RenderFogPlane fog;
    public MoodBox startMoodBox;
    public Cubemap defaultPlayerReflection;
    public Material[] playerReflectionMaterials;
    public bool applyNearestMoodBox;
    public MoodBox currentMoodBox;
    [HideInInspector]
    public GameObject[] splashManagers;
    [HideInInspector]
    public GameObject[] rainManagers;

    private Bloom bloomPostProcess;
    private ColorParameter colorParameter;

    private Renderer fogRenderer;   // Cache del renderer para no llamar al getComponent cada Update()
    private float previousFogY = 0f;    // Valor del fogY para para las asignaciones a los valores del shader.


    public virtual void Awake()
    {
        // mood boxes have the power to disable expensive effects when it makes sense
       splashManagers = GameObject.FindGameObjectsWithTag("RainSplashManager");
       rainManagers = GameObject.FindGameObjectsWithTag("RainBoxManager");
    }

    public virtual void Start()
    {
        if (!bloom)
        {
            bloom = Camera.main.gameObject.GetComponent<MobileBloom>();
        }
        if (!noise)
        {
            noise = Camera.main.gameObject.GetComponent<ColoredNoise>();
        }
        if (!fog)
        {
            fog = Camera.main.gameObject.GetComponentInChildren<RenderFogPlane>();
        }

        if (!fogRenderer)
        {
            fogRenderer = fog.GetComponent<Renderer>();
        }

        bloomPostProcess = postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.Bloom>();   /**/
        colorParameter = new UnityEngine.Rendering.PostProcessing.ColorParameter();

        MoodBoxManager.current = startMoodBox;
        UpdateFromMoodBox();
    }

    public virtual void Update()
    {
        UpdateFromMoodBox();
    }

    public virtual MoodBoxData GetData()
    {
        return currentData;
    }

    public virtual void UpdateFromMoodBox()
    {
        ApplyNearestMoodBoxIfDesired();

        // we want to see what the current mood box is in the editor
        currentMoodBox = MoodBoxManager.current;
        if (MoodBoxManager.current)
        {
            if (!Application.isPlaying)
            {
                currentData.noiseAmount = MoodBoxManager.current.data.noiseAmount;
                currentData.colorMixBlend = MoodBoxManager.current.data.colorMixBlend;
                currentData.colorMix = MoodBoxManager.current.data.colorMix;
                currentData.fogY = MoodBoxManager.current.data.fogY;
                currentData.fogColor = MoodBoxManager.current.data.fogColor;
                currentData.outside = MoodBoxManager.current.data.outside;
            }
            else
            {
                // play mode, interpolate nicely
                currentData.noiseAmount = Mathf.Lerp(currentData.noiseAmount, MoodBoxManager.current.data.noiseAmount, Time.deltaTime);
                currentData.colorMixBlend = Mathf.Lerp(currentData.colorMixBlend, MoodBoxManager.current.data.colorMixBlend, Time.deltaTime);
                currentData.colorMix = Color.Lerp(currentData.colorMix, MoodBoxManager.current.data.colorMix, Time.deltaTime);
                currentData.fogY = Mathf.Lerp(currentData.fogY, MoodBoxManager.current.data.fogY, Time.deltaTime * 1.5f);
                currentData.fogColor = Color.Lerp(currentData.fogColor, MoodBoxManager.current.data.fogColor, Time.deltaTime * 0.25f);
                currentData.outside = MoodBoxManager.current.data.outside;
            }
        }
        // apply new mood and effect values to actual effects (if in use)
        if ((bloom && bloom.enabled && bloom.colorMix != currentData.colorMix) || (bloomPostProcess !=null))
        {
            bloom.colorMix = currentData.colorMix;
            bloom.colorMixBlend = currentData.colorMixBlend;

            colorParameter.value = currentData.colorMix;
            bloomPostProcess.color.Override(colorParameter); /**/
        }
        if (noise && noise.enabled && noise.localNoiseAmount != currentData.noiseAmount)
        {
            noise.localNoiseAmount = currentData.noiseAmount;
        }
        if (fog && fog.enabled && previousFogY != currentData.fogY) /**/
        {
            fogRenderer.sharedMaterial.SetFloat("_Y", currentData.fogY);
            fogRenderer.sharedMaterial.SetColor("_FogColor", currentData.fogColor);
            previousFogY = currentData.fogY;
        }
    }

    public virtual void ApplyNearestMoodBoxIfDesired()
    {
        if (applyNearestMoodBox)
        {
            Component[] boxes = null;
            boxes = GetComponentsInChildren<MoodBox>(); // as MoodBox[];
            if (boxes != null)
            {
                Vector3 cameraPos = Camera.main.transform.position;
                MoodBox minMoodBox = boxes[0] as MoodBox;
                float minDistance = Mathf.Infinity;
                foreach (Component b in boxes)
                {
                    if (((b as MoodBox).transform.position - cameraPos).sqrMagnitude < minDistance)
                    {
                        minDistance = ((b as MoodBox).transform.position - cameraPos).sqrMagnitude;
                        minMoodBox = b as MoodBox;
                    }
                }
                MoodBoxManager.current = minMoodBox;
            }
            else
            {
                Debug.Log("no MoodBox components found ...");
            }
            applyNearestMoodBox = false;
        }
    }

}