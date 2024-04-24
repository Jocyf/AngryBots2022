using UnityEngine;
using System.Collections;

public enum DofQualitySetting
{
    OnlyBackground = 1,
    BackgroundAndForeground = 2
}

public enum DofResolution
{
    High = 2,
    Medium = 3,
    Low = 4
}

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Height Depth of Field")]
public partial class HeightDepthOfField : MonoBehaviour
{
    public DofResolution resolution;
    public Transform objectFocus;
    public float maxBlurSpread;
    public float foregroundBlurExtrude;
    public float smoothness;
    private Shader dofBlurShader;
    private Material dofBlurMaterial;
    private Shader dofShader;
    private Material dofMaterial;
    public bool visualize;
    private float widthOverHeight;
    private float oneOverBaseSize;
    private float cameraNear;
    private float cameraFar;
    private float cameraFov;
    private float cameraAspect;
    public virtual void Start()
    {
        this.FindShaders();
        this.CheckSupport();
        this.CreateMaterials();
    }

    public virtual void FindShaders()
    {
        if (!this.dofBlurShader)
        {
            this.dofBlurShader = Shader.Find("Hidden/BlurPassesForDOF");
        }
        if (!this.dofShader)
        {
            this.dofShader = Shader.Find("Hidden/HeightDepthOfField");
        }
    }

    public virtual void CreateMaterials()
    {
        if (!this.dofBlurMaterial)
        {
            this.dofBlurMaterial = PostEffects.CheckShaderAndCreateMaterial(this.dofBlurShader, this.dofBlurMaterial);
        }
        if (!this.dofMaterial)
        {
            this.dofMaterial = PostEffects.CheckShaderAndCreateMaterial(this.dofShader, this.dofMaterial);
        }
    }

    public virtual bool Supported()
    {
        return (PostEffects.CheckSupport(true) && this.dofBlurShader.isSupported) && this.dofShader.isSupported;
    }

    public virtual bool CheckSupport()
    {
        if (!this.Supported())
        {
            this.enabled = false;
            return false;
        }
        return true;
    }

    public virtual void OnDisable()
    {
        if (this.dofBlurMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this.dofBlurMaterial);
            this.dofBlurMaterial = null;
        }
        if (this.dofMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this.dofMaterial);
            this.dofMaterial = null;
        }
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Vector4 vec = default(Vector4);
        Vector3 corner = default(Vector3);
        this.FindShaders();
        this.CheckSupport();
        this.CreateMaterials();
        this.widthOverHeight = (1f * source.width) / (1f * source.height);
        this.oneOverBaseSize = 1f / 512f;
        this.cameraNear = this.GetComponent<Camera>().nearClipPlane;
        this.cameraFar = this.GetComponent<Camera>().farClipPlane;
        this.cameraFov = this.GetComponent<Camera>().fieldOfView;
        this.cameraAspect = this.GetComponent<Camera>().aspect;
        Matrix4x4 frustumCorners = Matrix4x4.identity;
        float fovWHalf = this.cameraFov * 0.5f;
        Vector3 toRight = ((this.GetComponent<Camera>().transform.right * this.cameraNear) * Mathf.Tan(fovWHalf * Mathf.Deg2Rad)) * this.cameraAspect;
        Vector3 toTop = (this.GetComponent<Camera>().transform.up * this.cameraNear) * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);
        Vector3 topLeft = ((this.GetComponent<Camera>().transform.forward * this.cameraNear) - toRight) + toTop;
        float cameraScaleFactor = (topLeft.magnitude * this.cameraFar) / this.cameraNear;
        topLeft.Normalize();
        topLeft = topLeft * cameraScaleFactor;
        Vector3 topRight = ((this.GetComponent<Camera>().transform.forward * this.cameraNear) + toRight) + toTop;
        topRight.Normalize();
        topRight = topRight * cameraScaleFactor;
        Vector3 bottomRight = ((this.GetComponent<Camera>().transform.forward * this.cameraNear) + toRight) - toTop;
        bottomRight.Normalize();
        bottomRight = bottomRight * cameraScaleFactor;
        Vector3 bottomLeft = ((this.GetComponent<Camera>().transform.forward * this.cameraNear) - toRight) - toTop;
        bottomLeft.Normalize();
        bottomLeft = bottomLeft * cameraScaleFactor;
        frustumCorners.SetRow(0, topLeft);
        frustumCorners.SetRow(1, topRight);
        frustumCorners.SetRow(2, bottomRight);
        frustumCorners.SetRow(3, bottomLeft);
        this.dofMaterial.SetMatrix("_FrustumCornersWS", frustumCorners);
        this.dofMaterial.SetVector("_CameraWS", this.GetComponent<Camera>().transform.position);
        Transform t = null;
        if (!this.objectFocus)
        {
            t = this.GetComponent<Camera>().transform;
        }
        else
        {
            t = this.objectFocus.transform;
        }
        this.dofMaterial.SetVector("_ObjectFocusParameter", new Vector4(t.position.y - 0.25f, (t.localScale.y * 1f) / this.smoothness, 1f, this.objectFocus ? this.objectFocus.GetComponent<Collider>().bounds.extents.y * 0.75f : 0.55f));
        this.dofMaterial.SetFloat("_ForegroundBlurExtrude", this.foregroundBlurExtrude);
        this.dofMaterial.SetVector("_InvRenderTargetSize", new Vector4(1f / (1f * source.width), 1f / (1f * source.height), 0f, 0f));
        int divider = 1;
        if (this.resolution == DofResolution.Medium)
        {
            divider = 2;
        }
        else
        {
            if (this.resolution >= DofResolution.Medium)
            {
                divider = 3;
            }
        }
        RenderTexture hrTex = RenderTexture.GetTemporary(source.width, source.height, 0);
        RenderTexture mediumTexture = RenderTexture.GetTemporary(source.width / divider, source.height / divider, 0);
        RenderTexture mediumTexture2 = RenderTexture.GetTemporary(source.width / divider, source.height / divider, 0);
        RenderTexture lowTexture = RenderTexture.GetTemporary(source.width / (divider * 2), source.height / (divider * 2), 0);
        source.filterMode = FilterMode.Bilinear;
        hrTex.filterMode = FilterMode.Bilinear;
        lowTexture.filterMode = FilterMode.Bilinear;
        mediumTexture.filterMode = FilterMode.Bilinear;
        mediumTexture2.filterMode = FilterMode.Bilinear;
        // background (coc -> alpha channel)
        this.CustomGraphicsBlit(null, source, this.dofMaterial, 3);
        // better downsample (should actually be weighted for higher quality)
        mediumTexture2.DiscardContents();
        Graphics.Blit(source, mediumTexture2, this.dofMaterial, 6);
        this.Blur(mediumTexture2, mediumTexture, 1, 0, this.maxBlurSpread * 0.75f);
        this.Blur(mediumTexture, lowTexture, 2, 0, this.maxBlurSpread);
        // some final calculations can be performed in low resolution 		
        this.dofBlurMaterial.SetTexture("_TapLow", lowTexture);
        this.dofBlurMaterial.SetTexture("_TapMedium", mediumTexture);
        Graphics.Blit(null, mediumTexture2, this.dofBlurMaterial, 2);
        this.dofMaterial.SetTexture("_TapLowBackground", mediumTexture2);
        this.dofMaterial.SetTexture("_TapMedium", mediumTexture); // only needed for debugging		
        // apply background defocus
        hrTex.DiscardContents();
        Graphics.Blit(source, hrTex, this.dofMaterial, this.visualize ? 2 : 0);
        // foreground handling
        this.CustomGraphicsBlit(hrTex, source, this.dofMaterial, 5);
        // better downsample and blur (shouldn't be weighted)
        Graphics.Blit(source, mediumTexture2, this.dofMaterial, 6);
        this.Blur(mediumTexture2, mediumTexture, 1, 1, this.maxBlurSpread * 0.75f);
        this.Blur(mediumTexture, lowTexture, 2, 1, this.maxBlurSpread);
        // some final calculations can be performed in low resolution		
        this.dofBlurMaterial.SetTexture("_TapLow", lowTexture);
        this.dofBlurMaterial.SetTexture("_TapMedium", mediumTexture);
        Graphics.Blit(null, mediumTexture2, this.dofBlurMaterial, 2);
        if (destination != null)
        {
            destination.DiscardContents();
        }
        this.dofMaterial.SetTexture("_TapLowForeground", mediumTexture2);
        this.dofMaterial.SetTexture("_TapMedium", mediumTexture); // only needed for debugging	   
        Graphics.Blit(source, destination, this.dofMaterial, this.visualize ? 1 : 4);
        RenderTexture.ReleaseTemporary(hrTex);
        RenderTexture.ReleaseTemporary(mediumTexture);
        RenderTexture.ReleaseTemporary(mediumTexture2);
        RenderTexture.ReleaseTemporary(lowTexture);
    }

    // flat blur
    public virtual void Blur(RenderTexture from, RenderTexture to, int iterations, int blurPass, float spread)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(to.width, to.height, 0);
        if (iterations < 2)
        {
            this.dofBlurMaterial.SetVector("offsets", new Vector4(0f, spread * this.oneOverBaseSize, 0f, 0f));
            tmp.DiscardContents();
            Graphics.Blit(from, tmp, this.dofBlurMaterial, blurPass);
            this.dofBlurMaterial.SetVector("offsets", new Vector4((spread / this.widthOverHeight) * this.oneOverBaseSize, 0f, 0f, 0f));
            to.DiscardContents();
            Graphics.Blit(tmp, to, this.dofBlurMaterial, blurPass);
        }
        else
        {
            this.dofBlurMaterial.SetVector("offsets", new Vector4(0f, spread * this.oneOverBaseSize, 0f, 0f));
            tmp.DiscardContents();
            Graphics.Blit(from, tmp, this.dofBlurMaterial, blurPass);
            this.dofBlurMaterial.SetVector("offsets", new Vector4((spread / this.widthOverHeight) * this.oneOverBaseSize, 0f, 0f, 0f));
            to.DiscardContents();
            Graphics.Blit(tmp, to, this.dofBlurMaterial, blurPass);
            this.dofBlurMaterial.SetVector("offsets", new Vector4((spread / this.widthOverHeight) * this.oneOverBaseSize, spread * this.oneOverBaseSize, 0f, 0f));
            tmp.DiscardContents();
            Graphics.Blit(to, tmp, this.dofBlurMaterial, blurPass);
            this.dofBlurMaterial.SetVector("offsets", new Vector4((spread / this.widthOverHeight) * this.oneOverBaseSize, -spread * this.oneOverBaseSize, 0f, 0f));
            to.DiscardContents();
            Graphics.Blit(tmp, to, this.dofBlurMaterial, blurPass);
        }
        RenderTexture.ReleaseTemporary(tmp);
    }

    // used for noise
    public virtual void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
    {
        RenderTexture.active = dest;
        fxMaterial.SetTexture("_MainTex", source);
        GL.PushMatrix();
        GL.LoadOrtho();
        fxMaterial.SetPass(passNr);
        GL.Begin(GL.QUADS);
        GL.MultiTexCoord2(0, 0f, 0f);
        GL.Vertex3(0f, 0f, 3f); // BL
        GL.MultiTexCoord2(0, 1f, 0f);
        GL.Vertex3(1f, 0f, 2f); // BR
        GL.MultiTexCoord2(0, 1f, 1f);
        GL.Vertex3(1f, 1f, 1f); // TR
        GL.MultiTexCoord2(0, 0f, 1f);
        GL.Vertex3(0f, 1f, 0f); // TL
        GL.End();
        GL.PopMatrix();
    }

    public HeightDepthOfField()
    {
        this.resolution = DofResolution.High;
        this.maxBlurSpread = 1.55f;
        this.foregroundBlurExtrude = 1.055f;
        this.smoothness = 1f;
        this.widthOverHeight = 1.25f;
        this.oneOverBaseSize = 1f / 512f;
        this.cameraNear = 0.5f;
        this.cameraFar = 50f;
        this.cameraFov = 60f;
        this.cameraAspect = 1.333333f;
    }

}