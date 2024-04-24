using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Camera))]
public partial class ReBirth : MonoBehaviour
{
    public virtual void Start()
    {
        AudioListener al = null;
        al = Camera.main.gameObject.GetComponent<AudioListener>();
        if (al)
        {
            AudioListener.volume = 1f;
        }
        ShaderDatabase sm = this.GetComponent<ShaderDatabase>();
        this.StartCoroutine(sm.WhiteIn());
        this.GetComponent<Camera>().backgroundColor = Color.white;
    }

}