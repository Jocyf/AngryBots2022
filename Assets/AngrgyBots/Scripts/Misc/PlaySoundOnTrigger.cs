using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(AudioSource))]
public partial class PlaySoundOnTrigger : MonoBehaviour
{
    public bool onlyPlayOnce;
    private bool playedOnce;
    public virtual void OnTriggerEnter(object unusedArg)
    {
        if (this.playedOnce && this.onlyPlayOnce)
        {
            return;
        }
        this.GetComponent<AudioSource>().Play();
        this.playedOnce = true;
    }

    public PlaySoundOnTrigger()
    {
        this.onlyPlayOnce = true;
    }

}