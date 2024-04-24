using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class conveyorBelt : MonoBehaviour
{
    public float scrollSpeed;
    public Material mat;
    public virtual void Start()
    {
        this.enabled = false;
    }

    public virtual void OnBecameVisible()
    {
        this.enabled = true;
    }

    public virtual void OnBecameInvisible()
    {
        this.enabled = false;
    }

    public virtual void Update()
    {
        float offset = (Time.time * this.scrollSpeed) % 1f;
        this.mat.SetTextureOffset("_MainTex", new Vector2(0, -offset));
        this.mat.SetTextureOffset("_BumpMap", new Vector2(0, -offset));
    }

    public conveyorBelt()
    {
        this.scrollSpeed = 0.1f;
    }

}