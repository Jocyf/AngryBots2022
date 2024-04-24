using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PlaySoundAndBack : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;
    public AudioClip soundReverse;
    public float lengthWithoutTrailing;
    private bool back;
    private float normalizedTime;
    public virtual void Awake()
    {
        if (!this.audioSource && this.GetComponent<AudioSource>())
        {
            this.audioSource = this.GetComponent<AudioSource>();
        }
        if (this.lengthWithoutTrailing == 0)
        {
            this.lengthWithoutTrailing = Mathf.Min(this.sound.length, this.soundReverse.length);
        }
    }

    public virtual void OnSignal()
    {
        this.FixTime();
        this.PlayWithDirection();
    }

    public virtual void OnPlay()
    {
        this.FixTime();
        // Set the speed to be positive
        this.back = false;
        this.PlayWithDirection();
    }

    public virtual void OnPlayReverse()
    {
        this.FixTime();
        // Set the speed to be negative
        this.back = true;
        this.PlayWithDirection();
    }

    private void PlayWithDirection()
    {
        float playbackTime = 0.0f;
        if (this.back)
        {
            this.audioSource.clip = this.soundReverse;
            playbackTime = (1 - this.normalizedTime) * this.lengthWithoutTrailing;
        }
        else
        {
            this.audioSource.clip = this.sound;
            playbackTime = this.normalizedTime * this.lengthWithoutTrailing;
        }
        this.audioSource.time = playbackTime;
        this.audioSource.Play();
        this.back = !this.back;
    }

    private void FixTime()
    {
        if (this.audioSource.clip)
        {
            this.normalizedTime = 1f;
            if (this.audioSource.isPlaying)
            {
                this.normalizedTime = Mathf.Clamp01(this.audioSource.time / this.lengthWithoutTrailing);
            }
            if (this.audioSource.clip == this.soundReverse)
            {
                this.normalizedTime = 1 - this.normalizedTime;
            }
        }
    }

}