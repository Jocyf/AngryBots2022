using UnityEngine;
using System.Collections;

[System.Serializable]
public class MoveAnimation : object
{
    // The animation clip
    public AnimationClip clip;
    // The velocity of the walk or run cycle in this clip
    public Vector3 velocity;
    // Store the current weight of this animation
    [UnityEngine.HideInInspector]
    public float weight;
    // Keep track of whether this animation is currently the best match
    [UnityEngine.HideInInspector]
    public bool currentBest;
    // The speed and angle is directly derived from the velocity,
    // but since it's slightly expensive to calculate them
    // we do it once in the beginning instead of in every frame.
    [UnityEngine.HideInInspector]
    public float speed;
    [UnityEngine.HideInInspector]
    public float angle;
    public virtual void Init()
    {
        this.velocity.y = 0;
        this.speed = this.velocity.magnitude;
        this.angle = PlayerAnimation.HorizontalAngle(this.velocity);
    }

}
[System.Serializable]
public partial class PlayerAnimation : MonoBehaviour
{
    public Rigidbody rigid;
    public Transform rootBone;
    public Transform upperBodyBone;
    public float maxIdleSpeed;
    public float minWalkSpeed;
    public AnimationClip idle;
    public AnimationClip turn;
    public AnimationClip shootAdditive;
    public MoveAnimation[] moveAnimations;
    public SignalSender footstepSignals;
    private Transform tr;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private Vector3 localVelocity;
    private float speed;
    private float angle;
    private float lowerBodyDeltaAngle;
    private float idleWeight;
    private Vector3 lowerBodyForwardTarget;
    private Vector3 lowerBodyForward;
    private MoveAnimation bestAnimation;
    private float lastFootstepTime;
    private float lastAnimTime;
    public Animation animationComponent;
    public virtual void Awake()//animation[turn.name].enabled = true;
    {
        this.tr = this.rigid.transform;
        this.lastPosition = this.tr.position;
        foreach (MoveAnimation moveAnimation in this.moveAnimations)
        {
            moveAnimation.Init();
            this.animationComponent[moveAnimation.clip.name].layer = 1;
            this.animationComponent[moveAnimation.clip.name].enabled = true;
        }
        this.animationComponent.SyncLayer(1);
        this.animationComponent[this.idle.name].layer = 2;
        this.animationComponent[this.turn.name].layer = 3;
        this.animationComponent[this.idle.name].enabled = true;
        this.animationComponent[this.shootAdditive.name].layer = 4;
        this.animationComponent[this.shootAdditive.name].weight = 1;
        this.animationComponent[this.shootAdditive.name].speed = 0.6f;
        this.animationComponent[this.shootAdditive.name].blendMode = AnimationBlendMode.Additive;
    }

    public virtual void OnStartFire()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        this.animationComponent[this.shootAdditive.name].enabled = true;
    }

    public virtual void OnStopFire()
    {
        this.animationComponent[this.shootAdditive.name].enabled = false;
    }

    public virtual void FixedUpdate()
    {
        this.velocity = (this.tr.position - this.lastPosition) / Time.deltaTime;
        this.localVelocity = this.tr.InverseTransformDirection(this.velocity);
        this.localVelocity.y = 0;
        this.speed = this.localVelocity.magnitude;
        this.angle = PlayerAnimation.HorizontalAngle(this.localVelocity);
        this.lastPosition = this.tr.position;
    }

    public virtual void Update()
    {
        this.idleWeight = Mathf.Lerp(this.idleWeight, Mathf.InverseLerp(this.minWalkSpeed, this.maxIdleSpeed, this.speed), Time.deltaTime * 10);
        this.animationComponent[this.idle.name].weight = this.idleWeight;
        if (this.speed > 0)
        {
            float smallestDiff = Mathf.Infinity;
            foreach (MoveAnimation moveAnimation in this.moveAnimations)
            {
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(this.angle, moveAnimation.angle));
                float speedDiff = Mathf.Abs(this.speed - moveAnimation.speed);
                float diff = angleDiff + speedDiff;
                if (moveAnimation == this.bestAnimation)
                {
                    diff = diff * 0.9f;
                }
                if (diff < smallestDiff)
                {
                    this.bestAnimation = moveAnimation;
                    smallestDiff = diff;
                }
            }
            this.animationComponent.CrossFade(this.bestAnimation.clip.name);
        }
        else
        {
            this.bestAnimation = null;
        }
        if ((this.lowerBodyForward != this.lowerBodyForwardTarget) && (this.idleWeight >= 0.9f))
        {
            this.animationComponent.CrossFade(this.turn.name, 0.05f);
        }
        if ((this.bestAnimation != null) && (this.idleWeight < 0.9f))
        {
            float newAnimTime = Mathf.Repeat((this.animationComponent[this.bestAnimation.clip.name].normalizedTime * 2) + 0.1f, 1);
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

    public virtual void LateUpdate()
    {
        float idle = Mathf.InverseLerp(this.minWalkSpeed, this.maxIdleSpeed, this.speed);
        if (idle < 1)
        {
            // Calculate a weighted average of the animation velocities that are currently used
            Vector3 animatedLocalVelocity = Vector3.zero;
            foreach (MoveAnimation moveAnimation in this.moveAnimations)
            {
                // Ignore this animation if its weight is 0
                if (this.animationComponent[moveAnimation.clip.name].weight == 0)
                {
                    continue;
                }
                // Ignore this animation if its velocity is more than 90 degrees away from current velocity
                if (Vector3.Dot(moveAnimation.velocity, this.localVelocity) <= 0)
                {
                    continue;
                }
                // Add velocity of this animation to the weighted average
                animatedLocalVelocity = animatedLocalVelocity + (moveAnimation.velocity * this.animationComponent[moveAnimation.clip.name].weight);
            }
            // Calculate target angle to rotate lower body by in order
            // to make feet run in the direction of the velocity
            float lowerBodyDeltaAngleTarget = Mathf.DeltaAngle(PlayerAnimation.HorizontalAngle(this.tr.rotation * animatedLocalVelocity), PlayerAnimation.HorizontalAngle(this.velocity));
            // Lerp the angle to smooth it a bit
            this.lowerBodyDeltaAngle = Mathf.LerpAngle(this.lowerBodyDeltaAngle, lowerBodyDeltaAngleTarget, Time.deltaTime * 10);
            // Update these so they're ready for when we go into idle
            this.lowerBodyForwardTarget = this.tr.forward;
            this.lowerBodyForward = Quaternion.Euler(0, this.lowerBodyDeltaAngle, 0) * this.lowerBodyForwardTarget;
        }
        else
        {
            // Turn the lower body towards it's target direction
            this.lowerBodyForward = Vector3.RotateTowards(this.lowerBodyForward, this.lowerBodyForwardTarget, (Time.deltaTime * 520) * Mathf.Deg2Rad, 1);
            // Calculate delta angle to make the lower body stay in place
            this.lowerBodyDeltaAngle = Mathf.DeltaAngle(PlayerAnimation.HorizontalAngle(this.tr.forward), PlayerAnimation.HorizontalAngle(this.lowerBodyForward));
            // If the body is twisted more than 80 degrees,
            // set a new target direction for the lower body, so it begins turning
            if (Mathf.Abs(this.lowerBodyDeltaAngle) > 80)
            {
                this.lowerBodyForwardTarget = this.tr.forward;
            }
        }
        // Create a Quaternion rotation from the rotation angle
        Quaternion lowerBodyDeltaRotation = Quaternion.Euler(0, this.lowerBodyDeltaAngle, 0);
        // Rotate the whole body by the angle
        this.rootBone.rotation = lowerBodyDeltaRotation * this.rootBone.rotation;
        // Counter-rotate the upper body so it won't be affected
        this.upperBodyBone.rotation = Quaternion.Inverse(lowerBodyDeltaRotation) * this.upperBodyBone.rotation;
    }

    public static float HorizontalAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    public PlayerAnimation()
    {
        this.maxIdleSpeed = 0.5f;
        this.minWalkSpeed = 2f;
        this.lastPosition = Vector3.zero;
        this.velocity = Vector3.zero;
        this.localVelocity = Vector3.zero;
        this.lowerBodyForwardTarget = Vector3.forward;
        this.lowerBodyForward = Vector3.forward;
    }

}