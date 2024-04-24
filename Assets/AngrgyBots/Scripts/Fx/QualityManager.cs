using UnityEngine;
using System.Collections;

// QualityManager sets shader LOD's and enabled/disables special effects
// based on platform and/or desired quality settings.
// Disable 'autoChoseQualityOnStart' if you want to overwrite the quality
// for a specific platform with the desired level.
// Quality enum values will be used directly for shader LOD settings
public enum Quality
{
    Lowest = 100,
    Poor = 190,
    Low = 200,
    Medium = 210,
    High = 300,
    Highest = 500
}

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.RequireComponent(typeof(ShaderDatabase))]
public partial class QualityManager : MonoBehaviour
{
    public bool autoChoseQualityOnStart;
    public Quality currentQuality;
    public MobileBloom bloom;
    public HeightDepthOfField depthOfField;
    public ColoredNoise noise;
    public RenderFogPlane heightFog;
    public MonoBehaviour reflection;
    public ShaderDatabase shaders;
    public GameObject heightFogBeforeTransparentGO;
    public static Quality quality;
    public virtual void Start()
    {
        if (this.heightFogBeforeTransparentGO != null)
        {
            this.heightFogBeforeTransparentGO.SetActive(true);
        }
        if (!this.bloom)
        {
            this.bloom = this.GetComponent<MobileBloom>();
        }
        if (!this.noise)
        {
            this.noise = this.GetComponent<ColoredNoise>();
        }
        if (!this.depthOfField)
        {
            this.depthOfField = this.GetComponent<HeightDepthOfField>();
        }
        if (!this.heightFog)
        {
            this.heightFog = this.gameObject.GetComponentInChildren<RenderFogPlane>();
        }
        if (!this.shaders)
        {
            this.shaders = this.GetComponent<ShaderDatabase>();
        }
        if (!this.reflection)
        {
            this.reflection = this.GetComponent("ReflectionFx") as MonoBehaviour;
        }
        if (this.autoChoseQualityOnStart)
        {
            this.AutoDetectQuality();
        }
        this.ApplyAndSetQuality(this.currentQuality);
    }

    // we support dynamic quality adjustments if in edit mode
    public virtual void Update()
    {
        Quality newQuality = this.currentQuality;
        if (newQuality != QualityManager.quality)
        {
            this.ApplyAndSetQuality(newQuality);
        }
    }

    private void AutoDetectQuality()
    {
        /*switch (Application.platform)
        {
            case RuntimePlatform.NaCl:
                this.currentQuality = Quality.Highest;
                break;
            case RuntimePlatform.FlashPlayer:
                this.currentQuality = Quality.Low;
                break;
            default:
                this.currentQuality = SystemInfo.graphicsPixelFillrate < 2800 ? Quality.High : Quality.Highest;
                break;
        }*/

        this.currentQuality = Quality.High; /**/
        Debug.Log(string.Format("AngryBots: Quality set to '{0}'{1}", (object) this.currentQuality, (" (" + Application.platform) + ")"));
    }

    private void ApplyAndSetQuality(Quality newQuality)
    {
        QualityManager.quality = newQuality;
        // default states
        this.GetComponent<Camera>().cullingMask = -1 & ~(1 << LayerMask.NameToLayer("Adventure"));
        GameObject textAdventure = GameObject.Find("TextAdventure");
        if (textAdventure)
        {
            textAdventure.GetComponent<TextAdventureManager>().enabled = false;
        }
        // check for quality specific states
        if (QualityManager.quality == Quality.Lowest)
        {
            this.DisableAllFx();
            if (textAdventure)
            {
                textAdventure.GetComponent<TextAdventureManager>().enabled = true;
            }
            this.GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Adventure");
            this.EnableFx(this.depthOfField, false);
            this.EnableFx(this.heightFog, false);
            this.EnableFx(this.bloom, false);
            this.EnableFx(this.noise, false);
            this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
        }
        else
        {
            if (QualityManager.quality == Quality.Poor)
            {
                this.EnableFx(this.depthOfField, false);
                this.EnableFx(this.heightFog, false);
                this.EnableFx(this.bloom, false);
                this.EnableFx(this.noise, false);
                this.EnableFx(this.reflection, false);
                this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
            }
            else
            {
                if (QualityManager.quality == Quality.Low)
                {
                    this.EnableFx(this.depthOfField, false);
                    this.EnableFx(this.heightFog, false);
                    this.EnableFx(this.bloom, false);
                    this.EnableFx(this.noise, false);
                    this.EnableFx(this.reflection, SystemInfo.supportsRenderTextures);
                    this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
                }
                else
                {
                    if (QualityManager.quality == Quality.Medium)
                    {
                        this.EnableFx(this.depthOfField, false);
                        this.EnableFx(this.heightFog, false);
                        this.EnableFx(this.bloom, true);
                        this.EnableFx(this.noise, false);
                        this.EnableFx(this.reflection, SystemInfo.supportsRenderTextures);
                        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
                    }
                    else
                    {
                        if (QualityManager.quality == Quality.High)
                        {
                            this.EnableFx(this.depthOfField, false);
                            this.EnableFx(this.heightFog, false);
                            this.EnableFx(this.bloom, true);
                            this.EnableFx(this.noise, true);
                            this.EnableFx(this.reflection, SystemInfo.supportsRenderTextures);
                            this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
                        }
                        else
                        {
                             // Highest
                            this.EnableFx(this.depthOfField, true);
                            this.EnableFx(this.heightFog, true);
                            this.EnableFx(this.bloom, true);
                            this.EnableFx(this.reflection, SystemInfo.supportsRenderTextures);
                            this.EnableFx(this.noise, true);
                            if ((this.heightFog && this.heightFog.enabled) || (this.depthOfField && this.depthOfField.enabled))
                            {
                                this.GetComponent<Camera>().depthTextureMode = this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("AngryBots: setting shader LOD to " + QualityManager.quality);
        Shader.globalMaximumLOD = (int) QualityManager.quality;
        foreach (Shader s in this.shaders.shaders)
        {
            s.maximumLOD = (int) QualityManager.quality;
        }
    }

    private void DisableAllFx()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
        this.EnableFx(this.reflection, false);
        this.EnableFx(this.depthOfField, false);
        this.EnableFx(this.heightFog, false);
        this.EnableFx(this.bloom, false);
        this.EnableFx(this.noise, false);
    }

    private void EnableFx(MonoBehaviour fx, bool enable)
    {
        if (fx)
        {
            fx.enabled = enable;
        }
    }

    public QualityManager()
    {
        this.autoChoseQualityOnStart = true;
        this.currentQuality = Quality.Highest;
    }

    static QualityManager()
    {
        QualityManager.quality = Quality.Highest;
    }

}