using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Mobile Bloom")]
public partial class MobileBloom : MonoBehaviour
{
    public float intensity;
    public Color colorMix;
    public float colorMixBlend;
    public float agonyTint;
    private Shader bloomShader;
    private Material apply;
    private RenderTextureFormat rtFormat;
    public virtual void Start()
    {
        this.FindShaders();
        this.CheckSupport();
        this.CreateMaterials();
    }

    public virtual void FindShaders()
    {
        if (!this.bloomShader)
        {
            this.bloomShader = Shader.Find("Hidden/MobileBloom");
        }
    }

    public virtual void CreateMaterials()
    {
        if (!this.apply)
        {
            this.apply = new Material(this.bloomShader);
            this.apply.hideFlags = HideFlags.DontSave;
        }
    }

    public virtual void OnDamage()
    {
        this.agonyTint = 1f;
    }

    public virtual bool Supported()
    {
        return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures) && this.bloomShader.isSupported;
    }

    public virtual bool CheckSupport()
    {
        if (!this.Supported())
        {
            this.enabled = false;
            return false;
        }
        this.rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
        return true;
    }

    public virtual void OnDisable()
    {
        if (this.apply)
        {
            UnityEngine.Object.DestroyImmediate(this.apply);
            this.apply = null;
        }
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this.FindShaders();
        this.CheckSupport();
        this.CreateMaterials();
        this.agonyTint = Mathf.Clamp01(this.agonyTint - (Time.deltaTime * 2.75f));
        RenderTexture tempRtLowA = RenderTexture.GetTemporary(source.width / 4, source.height / 4, (int) this.rtFormat);
        RenderTexture tempRtLowB = RenderTexture.GetTemporary(source.width / 4, source.height / 4, (int) this.rtFormat);
        // prepare data
        this.apply.SetColor("_ColorMix", this.colorMix);
        this.apply.SetVector("_Parameter", new Vector4(this.colorMixBlend * 0.25f, 0f, 0f, (1f - this.intensity) - this.agonyTint));
        // downsample & blur
        Graphics.Blit(source, tempRtLowA, this.apply, this.agonyTint < 0.5f ? 1 : 5);
        Graphics.Blit(tempRtLowA, tempRtLowB, this.apply, 2);
        Graphics.Blit(tempRtLowB, tempRtLowA, this.apply, 3);
        // apply
        this.apply.SetTexture("_Bloom", tempRtLowA);
        Graphics.Blit(source, destination, this.apply, QualityManager.quality > Quality.Medium ? 4 : 0);
        RenderTexture.ReleaseTemporary(tempRtLowA);
        RenderTexture.ReleaseTemporary(tempRtLowB);
    }

    public MobileBloom()
    {
        this.intensity = 0.5f;
        this.colorMix = Color.white;
        this.colorMixBlend = 0.25f;
        this.rtFormat = RenderTextureFormat.Default;
    }

}