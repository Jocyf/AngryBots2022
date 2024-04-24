using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ScaleAndDestroyRefractSphere : MonoBehaviour
{
    public float maxScale;
    public float scaleSpeed;
    public float lifetime;
    public virtual void Start()
    {
        UnityEngine.Object.Destroy(this.gameObject, this.lifetime);
    }

    public virtual void Update()
    {
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.one * this.maxScale, Time.deltaTime * this.scaleSpeed);
    }

    public ScaleAndDestroyRefractSphere()
    {
        this.maxScale = 5f;
        this.scaleSpeed = 2f;
        this.lifetime = 2f;
    }

}