using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PlayAnimation : MonoBehaviour
{
    public string clip;
    public virtual void OnSignal()
    {
        this.GetComponent<Animation>().Play(this.clip);
    }

    public PlayAnimation()
    {
        this.clip = "MyAnimation";
    }

}