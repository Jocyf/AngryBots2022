using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GlowChange : MonoBehaviour
{
    public int signalsNeeded;
    public virtual void OnSignal()
    {
        this.signalsNeeded--;
        if (this.signalsNeeded == 0)
        {
            this.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.29f, 0.64f, 0.15f, 0.5f));
            this.enabled = false;
        }
    }

    public GlowChange()
    {
        this.signalsNeeded = 1;
    }

}