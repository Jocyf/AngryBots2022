using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ShiftUV : MonoBehaviour
{
    public Vector2 offsetVector;
    public virtual void Start()
    {
    }

    public virtual void OnSignal()
    {
        this.GetComponent<Renderer>().material.mainTextureOffset = this.GetComponent<Renderer>().material.mainTextureOffset + this.offsetVector;
    }

}