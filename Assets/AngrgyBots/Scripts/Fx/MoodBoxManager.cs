using UnityEngine;
using System.Collections;

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
    public ColoredNoise noise;
    public RenderFogPlane fog;
    public MoodBox startMoodBox;
    public Cubemap defaultPlayerReflection;
    public Material[] playerReflectionMaterials;
    public bool applyNearestMoodBox;
    public MoodBox currentMoodBox;
    [UnityEngine.HideInInspector]
    public GameObject[] splashManagers;
    [UnityEngine.HideInInspector]
    public GameObject[] rainManagers;
    public virtual void Awake()
    {
        // mood boxes have the power to disable expensive effects when it makes sense
        this.splashManagers = GameObject.FindGameObjectsWithTag("RainSplashManager");
        this.rainManagers = GameObject.FindGameObjectsWithTag("RainBoxManager");
    }

    public virtual void Start()
    {
        if (!this.bloom)
        {
            this.bloom = Camera.main.gameObject.GetComponent<MobileBloom>();
        }
        if (!this.noise)
        {
            this.noise = Camera.main.gameObject.GetComponent<ColoredNoise>();
        }
        if (!this.fog)
        {
            this.fog = Camera.main.gameObject.GetComponentInChildren<RenderFogPlane>();
        }
        MoodBoxManager.current = this.startMoodBox;
        this.UpdateFromMoodBox();
    }

    public virtual void Update()
    {
        this.UpdateFromMoodBox();
    }

    public virtual MoodBoxData GetData()
    {
        return this.currentData;
    }

    public virtual void UpdateFromMoodBox()
    {
        this.ApplyNearestMoodBoxIfDesired();
        // we want to see what the current mood box is in the editor
        this.currentMoodBox = MoodBoxManager.current;
        if (MoodBoxManager.current)
        {
            if (!Application.isPlaying)
            {
                this.currentData.noiseAmount = MoodBoxManager.current.data.noiseAmount;
                this.currentData.colorMixBlend = MoodBoxManager.current.data.colorMixBlend;
                this.currentData.colorMix = MoodBoxManager.current.data.colorMix;
                this.currentData.fogY = MoodBoxManager.current.data.fogY;
                this.currentData.fogColor = MoodBoxManager.current.data.fogColor;
                this.currentData.outside = MoodBoxManager.current.data.outside;
            }
            else
            {
                // play mode, interpolate nicely
                this.currentData.noiseAmount = Mathf.Lerp(this.currentData.noiseAmount, MoodBoxManager.current.data.noiseAmount, Time.deltaTime);
                this.currentData.colorMixBlend = Mathf.Lerp(this.currentData.colorMixBlend, MoodBoxManager.current.data.colorMixBlend, Time.deltaTime);
                this.currentData.colorMix = Color.Lerp(this.currentData.colorMix, MoodBoxManager.current.data.colorMix, Time.deltaTime);
                this.currentData.fogY = Mathf.Lerp(this.currentData.fogY, MoodBoxManager.current.data.fogY, Time.deltaTime * 1.5f);
                this.currentData.fogColor = Color.Lerp(this.currentData.fogColor, MoodBoxManager.current.data.fogColor, Time.deltaTime * 0.25f);
                this.currentData.outside = MoodBoxManager.current.data.outside;
            }
        }
        // apply new mood and effect values to actual effects (if in use)
        if (this.bloom && this.bloom.enabled)
        {
            this.bloom.colorMix = this.currentData.colorMix;
            this.bloom.colorMixBlend = this.currentData.colorMixBlend;
        }
        if (this.noise && this.noise.enabled)
        {
            this.noise.localNoiseAmount = this.currentData.noiseAmount;
        }
        if (this.fog && this.fog.enabled)
        {
            this.fog.GetComponent<Renderer>().sharedMaterial.SetFloat("_Y", this.currentData.fogY);
            this.fog.GetComponent<Renderer>().sharedMaterial.SetColor("_FogColor", this.currentData.fogColor);
        }
    }

    public virtual void ApplyNearestMoodBoxIfDesired()
    {
        if (this.applyNearestMoodBox)
        {
            Component[] boxes = null;
            boxes = this.GetComponentsInChildren(typeof(MoodBox)); // as MoodBox[];
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
            this.applyNearestMoodBox = false;
        }
    }

}