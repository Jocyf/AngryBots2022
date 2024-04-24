using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ExplosionLine : MonoBehaviour
{
    public int frames;
    private int _frames;
    public virtual void OnEnable()
    {
        this._frames = 0;
    }

    public virtual void Update()
    {
        this._frames++;
        if (this._frames > this.frames)
        {
            this.gameObject.SetActive(false);
        }
    }

    public ExplosionLine()
    {
        this.frames = 2;
    }

}