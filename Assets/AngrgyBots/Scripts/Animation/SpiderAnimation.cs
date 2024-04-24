using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpiderAnimation : MonoBehaviour
{
    public MovementMotor motor;
    public AnimationClip activateAnim;
    public AnimationClip forwardAnim;
    public AnimationClip backAnim;
    public AnimationClip leftAnim;
    public AnimationClip rightAnim;
    public AudioSource audioSource;
    public SignalSender footstepSignals;
    public bool skiddingSounds;
    public bool footstepSounds;
    private Transform tr;
    private float lastFootstepTime;
    private float lastAnimTime;
    public virtual void OnEnable()
    {
        this.tr = this.motor.transform;
        this.GetComponent<Animation>()[this.activateAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.activateAnim.name].weight = 1;
        this.GetComponent<Animation>()[this.activateAnim.name].time = 0;
        this.GetComponent<Animation>()[this.activateAnim.name].speed = 1;
        this.GetComponent<Animation>()[this.forwardAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.forwardAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.forwardAnim.name].weight = 0;
        this.GetComponent<Animation>()[this.backAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.backAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.backAnim.name].weight = 0;
        this.GetComponent<Animation>()[this.leftAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.leftAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.leftAnim.name].weight = 0;
        this.GetComponent<Animation>()[this.rightAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.rightAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.rightAnim.name].weight = 0;
    }

    public virtual void OnDisable()
    {
        this.GetComponent<Animation>()[this.activateAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.activateAnim.name].weight = 1;
        this.GetComponent<Animation>()[this.activateAnim.name].normalizedTime = 1;
        this.GetComponent<Animation>()[this.activateAnim.name].speed = -1;
        this.GetComponent<Animation>().CrossFade(this.activateAnim.name, 0.3f, PlayMode.StopAll);
    }

    public virtual void Update()
    {
        float w = 0.0f;
        Vector3 direction = this.motor.movementDirection;
        direction.y = 0;
        float walkWeight = direction.magnitude;
        this.GetComponent<Animation>()[this.forwardAnim.name].speed = walkWeight;
        this.GetComponent<Animation>()[this.rightAnim.name].speed = walkWeight;
        this.GetComponent<Animation>()[this.backAnim.name].speed = walkWeight;
        this.GetComponent<Animation>()[this.leftAnim.name].speed = walkWeight;
        float angle = Mathf.DeltaAngle(SpiderAnimation.HorizontalAngle(this.tr.forward), SpiderAnimation.HorizontalAngle(direction));
        if (walkWeight > 0.01f)
        {
            if (angle < -90)
            {
                w = Mathf.InverseLerp(-180, -90, angle);
                this.GetComponent<Animation>()[this.forwardAnim.name].weight = 0;
                this.GetComponent<Animation>()[this.rightAnim.name].weight = 0;
                this.GetComponent<Animation>()[this.backAnim.name].weight = 1 - w;
                this.GetComponent<Animation>()[this.leftAnim.name].weight = 1;
            }
            else
            {
                if (angle < 0)
                {
                    w = Mathf.InverseLerp(-90, 0, angle);
                    this.GetComponent<Animation>()[this.forwardAnim.name].weight = w;
                    this.GetComponent<Animation>()[this.rightAnim.name].weight = 0;
                    this.GetComponent<Animation>()[this.backAnim.name].weight = 0;
                    this.GetComponent<Animation>()[this.leftAnim.name].weight = 1 - w;
                }
                else
                {
                    if (angle < 90)
                    {
                        w = Mathf.InverseLerp(0, 90, angle);
                        this.GetComponent<Animation>()[this.forwardAnim.name].weight = 1 - w;
                        this.GetComponent<Animation>()[this.rightAnim.name].weight = w;
                        this.GetComponent<Animation>()[this.backAnim.name].weight = 0;
                        this.GetComponent<Animation>()[this.leftAnim.name].weight = 0;
                    }
                    else
                    {
                        w = Mathf.InverseLerp(90, 180, angle);
                        this.GetComponent<Animation>()[this.forwardAnim.name].weight = 0;
                        this.GetComponent<Animation>()[this.rightAnim.name].weight = 1 - w;
                        this.GetComponent<Animation>()[this.backAnim.name].weight = w;
                        this.GetComponent<Animation>()[this.leftAnim.name].weight = 0;
                    }
                }
            }
        }
        if (this.skiddingSounds)
        {
            if ((walkWeight > 0.2f) && !this.audioSource.isPlaying)
            {
                this.audioSource.Play();
            }
            else
            {
                if ((walkWeight < 0.2f) && this.audioSource.isPlaying)
                {
                    this.audioSource.Pause();
                }
            }
        }
        if (this.footstepSounds && (walkWeight > 0.2f))
        {
            float newAnimTime = Mathf.Repeat((this.GetComponent<Animation>()[this.forwardAnim.name].normalizedTime * 4) + 0.1f, 1);
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

    public static float HorizontalAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

}