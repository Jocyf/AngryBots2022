using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class MechAnimation : MonoBehaviour
{
    public Rigidbody rigid;
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip turnLeft;
    public AnimationClip turnRight;
    public SignalSender footstepSignals;
    private Transform tr;
    private float lastFootstepTime;
    private float lastAnimTime;
    public virtual void OnEnable()//animation.SyncLayer (1);
    {
        this.tr = this.rigid.transform;
        this.GetComponent<Animation>()[this.idle.name].layer = 0;
        this.GetComponent<Animation>()[this.idle.name].weight = 1;
        this.GetComponent<Animation>()[this.idle.name].enabled = true;
        this.GetComponent<Animation>()[this.walk.name].layer = 1;
        this.GetComponent<Animation>()[this.turnLeft.name].layer = 1;
        this.GetComponent<Animation>()[this.turnRight.name].layer = 1;
        this.GetComponent<Animation>()[this.walk.name].weight = 1;
        this.GetComponent<Animation>()[this.turnLeft.name].weight = 0;
        this.GetComponent<Animation>()[this.turnRight.name].weight = 0;
        this.GetComponent<Animation>()[this.walk.name].enabled = true;
        this.GetComponent<Animation>()[this.turnLeft.name].enabled = true;
        this.GetComponent<Animation>()[this.turnRight.name].enabled = true;
    }

    public virtual void FixedUpdate()
    {
        float turningWeight = (Mathf.Abs(this.rigid.angularVelocity.y) * Mathf.Rad2Deg) / 100f;
        float forwardWeight = this.rigid.velocity.magnitude / 2.5f;
        float turningDir = Mathf.Sign(this.rigid.angularVelocity.y);
        // Temp, until we get the animations fixed
        this.GetComponent<Animation>()[this.walk.name].speed = Mathf.Lerp(1f, (this.GetComponent<Animation>()[this.walk.name].length / this.GetComponent<Animation>()[this.turnLeft.name].length) * 1.33f, turningWeight);
        this.GetComponent<Animation>()[this.turnLeft.name].time = this.GetComponent<Animation>()[this.walk.name].time;
        this.GetComponent<Animation>()[this.turnRight.name].time = this.GetComponent<Animation>()[this.walk.name].time;
        this.GetComponent<Animation>()[this.turnLeft.name].weight = Mathf.Clamp01(-turningWeight * turningDir);
        this.GetComponent<Animation>()[this.turnRight.name].weight = Mathf.Clamp01(turningWeight * turningDir);
        this.GetComponent<Animation>()[this.walk.name].weight = Mathf.Clamp01(forwardWeight);
        if ((forwardWeight + turningWeight) > 0.1f)
        {
            float newAnimTime = Mathf.Repeat((this.GetComponent<Animation>()[this.walk.name].normalizedTime * 2) + 0.1f, 1);
            if (newAnimTime < this.lastAnimTime)
            {
                if (Time.time > (this.lastFootstepTime + 0.1f))
                {
                    this.footstepSignals.SendSignals(this);
                    this.lastFootstepTime = Time.time;
                }
            }
            this.lastAnimTime = newAnimTime;
        }
    }

}