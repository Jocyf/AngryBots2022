using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Noise")]
public partial class ColoredNoise : MonoBehaviour
{
    public float globalNoiseAmount;
    public float globalNoiseAmountOnDamage;
    public float localNoiseAmount;
    private Shader noiseShader;
    public Texture2D noiseTexture;
    private Material noise;
    public virtual void Start()
    {
        this.FindShaders();
        this.CheckSupport();
        this.CreateMaterials();
    }

    public virtual void FindShaders()
    {
        if (!this.noiseShader)
        {
            this.noiseShader = Shader.Find("Hidden/ColoredNoise");
        }
    }

    public virtual void CreateMaterials()
    {
        if (!this.noise)
        {
            this.noise = new Material(this.noiseShader);
            this.noise.hideFlags = HideFlags.DontSave;
        }
    }

    public virtual bool Supported()
    {
        return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures) && this.noiseShader.isSupported;
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
        if (this.noise)
        {
            UnityEngine.Object.DestroyImmediate(this.noise);
            this.noise = null;
        }
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this.FindShaders();
        this.CheckSupport();
        this.CreateMaterials();
        this.noise.SetFloat("_NoiseAmount", this.globalNoiseAmount + (this.localNoiseAmount * Mathf.Sign(this.globalNoiseAmount)));
        this.noise.SetTexture("_NoiseTex", this.noiseTexture);
        ColoredNoise.DrawNoiseQuadGrid(source, destination, this.noise, this.noiseTexture, 0);
    }

    // Helper to draw a screenspace grid of quads with random texture coordinates
    public static void DrawNoiseQuadGrid(RenderTexture source, RenderTexture dest, Material fxMaterial, Texture2D noise, int passNr)
    {
        RenderTexture.active = dest;
        float tileSize = 64f;
        float subDs = (1f * source.width) / tileSize;
        fxMaterial.SetTexture("_MainTex", source);
        GL.PushMatrix();
        GL.LoadOrtho();
        float aspectCorrection = (1f * source.width) / (1f * source.height);
        float stepSizeX = 1f / subDs;
        float stepSizeY = stepSizeX * aspectCorrection;
        float texTile = tileSize / (noise.width * 1f);
        fxMaterial.SetPass(passNr);
        GL.Begin(GL.QUADS);
        float x1 = 0f;
        while (x1 < 1f)
        {
            float y1 = 0f;
            while (y1 < 1f)
            {
                float tcXStart = Random.Range(-1f, 1f);
                float tcYStart = Random.Range(-1f, 1f);
                float texTileMod = Mathf.Sign(Random.Range(-1f, 1f));
                GL.MultiTexCoord2(0, tcXStart, tcYStart);
                GL.Vertex3(x1, y1, 0.1f);
                GL.MultiTexCoord2(0, tcXStart + (texTile * texTileMod), tcYStart);
                GL.Vertex3(x1 + stepSizeX, y1, 0.1f);
                GL.MultiTexCoord2(0, tcXStart + (texTile * texTileMod), tcYStart + (texTile * texTileMod));
                GL.Vertex3(x1 + stepSizeX, y1 + stepSizeY, 0.1f);
                GL.MultiTexCoord2(0, tcXStart, tcYStart + (texTile * texTileMod));
                GL.Vertex3(x1, y1 + stepSizeY, 0.1f);
                y1 = y1 + stepSizeY;
            }
            x1 = x1 + stepSizeX;
        }
        GL.End();
        GL.PopMatrix();
    }

    public ColoredNoise()
    {
        this.globalNoiseAmount = 0.075f;
        this.globalNoiseAmountOnDamage = -6f;
    }

}