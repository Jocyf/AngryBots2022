using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;

    public virtual void Awake()
    {
        if (!audioSource && GetComponent<AudioSource>())
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public virtual void PlaySoundFX()
    {
        OnSignal();
    }

    public virtual void OnSignal()
    {
        if (sound)
        {
            audioSource.clip = sound;
        }
        audioSource.Play();
    }

}