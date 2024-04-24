using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PlayAnimationAndBack : MonoBehaviour
{
    public string clip;
    public float speed;
    public virtual void OnSignal()
    {
        this.FixTime();
        this.PlayWithSpeed();
    }

    public virtual void OnPlay()
    {
        this.FixTime();
        // Set the speed to be positive
        this.speed = Mathf.Abs(this.speed);
        this.PlayWithSpeed();
    }

    public virtual void OnPlayReverse()
    {
        this.FixTime();
        // Set the speed to be negative
        this.speed = Mathf.Abs(this.speed) * -1;
        this.PlayWithSpeed();
    }

    private void PlayWithSpeed()
    {
        // Play the animation with the desired speed.
        this.GetComponent<Animation>()[this.clip].speed = this.speed;
        this.GetComponent<Animation>()[this.clip].weight = 1;
        this.GetComponent<Animation>()[this.clip].enabled = true;
        // Reverse the speed so it's ready for playing the other way next time.
        this.speed = -this.speed;
    }

    private void FixTime()
    {
        // If the animation played to the end last time, it got automatically rewinded.
        // We don't want that here, so set the time back to 1.
        if ((this.speed < 0) && (this.GetComponent<Animation>()[this.clip].time == 0))
        {
            this.GetComponent<Animation>()[this.clip].normalizedTime = 1;
        }
        else
        {
            // In other cases, just clamp the time so it doesn't exceed the bounds of the animation.
            this.GetComponent<Animation>()[this.clip].normalizedTime = Mathf.Clamp01(this.GetComponent<Animation>()[this.clip].normalizedTime);
        }
    }

    public PlayAnimationAndBack()
    {
        this.clip = "MyAnimation";
        this.speed = 1f;
    }

}