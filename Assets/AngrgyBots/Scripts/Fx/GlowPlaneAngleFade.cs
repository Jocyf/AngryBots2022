using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GlowPlaneAngleFade : MonoBehaviour
{
    public Transform cameraTransform;
    public Color glowColor;
    private float dot;
    public virtual void Start()
    {
        if (!this.cameraTransform)
        {
            this.cameraTransform = Camera.main.transform;
        }
    }

    public virtual void Update()
    {
        this.dot = 1.5f * Mathf.Clamp01(Vector3.Dot(this.cameraTransform.forward, -this.transform.up) - 0.25f);
    }

    public virtual void OnWillRenderObject()
    {
        this.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", this.glowColor * this.dot);
    }

    public GlowPlaneAngleFade()
    {
        this.glowColor = Color.grey;
        this.dot = 0.5f;
    }

}