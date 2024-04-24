using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SelfIlluminationBlink : MonoBehaviour
{
    public float blink;
    public virtual void OnWillRenderObject()
    {
        this.GetComponent<Renderer>().sharedMaterial.SetFloat("_SelfIllumStrength", this.blink);
    }

    public virtual void Blink()
    {
        this.blink = 1f - this.blink;
    }

}