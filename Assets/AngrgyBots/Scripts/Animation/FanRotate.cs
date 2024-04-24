using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class FanRotate : MonoBehaviour
{
    public Mesh thisMesh;
    public Vector2[] uvs;
    public virtual void Start()
    {
        this.thisMesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh;
        this.uvs = this.thisMesh.uv;
    }

    public virtual void Update()
    {
        int i = 0;
        while (i < this.uvs.Length)
        {
            this.uvs[i].y = this.uvs[i].y + 0.25f;
            i++;
        }
        this.thisMesh.uv = this.uvs;
    }

}