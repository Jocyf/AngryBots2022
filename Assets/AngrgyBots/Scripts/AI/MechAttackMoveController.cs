using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class MechAttackMoveController : MonoBehaviour
{
    // Public member data
    public MovementMotor motor;
    public Transform head;
    public float targetDistanceMin;
    public float targetDistanceMax;
    public MonoBehaviour[] weaponBehaviours;
    public float fireFrequency;
    // Private memeber data
    private AI ai;
    private Transform character;
    private Transform player;
    private bool inRange;
    private float nextRaycastTime;
    private float lastRaycastSuccessfulTime;
    private float noticeTime;
    private bool firing;
    private float lastFireTime;
    private int nextWeaponToFire;
    public virtual void Awake()
    {
        this.character = this.motor.transform;
        this.player = GameObject.FindWithTag("Player").transform;
        this.ai = this.transform.parent.GetComponentInChildren<AI>();
    }

    public virtual void OnEnable()
    {
        this.inRange = false;
        this.nextRaycastTime = Time.time + 1;
        this.lastRaycastSuccessfulTime = Time.time;
        this.noticeTime = Time.time;
    }

    public virtual void OnDisable()
    {
        this.Shoot(false);
    }

    public virtual void Shoot(bool state)
    {
        this.firing = state;
    }

    public virtual void Fire()
    {
        if (this.weaponBehaviours[this.nextWeaponToFire])
        {
            this.weaponBehaviours[this.nextWeaponToFire].SendMessage("Fire");
            this.nextWeaponToFire = (this.nextWeaponToFire + 1) % this.weaponBehaviours.Length;
            this.lastFireTime = Time.time;
        }
    }

    public virtual void Update()
    {
        // Calculate the direction from the player to this character
        Vector3 playerDirection = this.player.position - this.character.position;
        playerDirection.y = 0;
        float playerDist = playerDirection.magnitude;
        playerDirection = playerDirection / playerDist;
        // Set this character to face the player,
        // that is, to face the direction from this character to the player
        this.motor.facingDirection = playerDirection;
        // For a short moment after noticing player,
        // only look at him but don't walk towards or attack yet.
        if (Time.time < (this.noticeTime + 1.5f))
        {
            this.motor.movementDirection = Vector3.zero;
            return;
        }
        if (this.inRange && (playerDist > this.targetDistanceMax))
        {
            this.inRange = false;
        }
        if (!this.inRange && (playerDist < this.targetDistanceMin))
        {
            this.inRange = true;
        }
        if (this.inRange)
        {
            this.motor.movementDirection = Vector3.zero;
        }
        else
        {
            this.motor.movementDirection = playerDirection;
        }
        if (Time.time > this.nextRaycastTime)
        {
            this.nextRaycastTime = Time.time + 1;
            if (this.ai.CanSeePlayer())
            {
                this.lastRaycastSuccessfulTime = Time.time;
                if (this.IsAimingAtPlayer())
                {
                    this.Shoot(true);
                }
                else
                {
                    this.Shoot(false);
                }
            }
            else
            {
                this.Shoot(false);
                if (Time.time > (this.lastRaycastSuccessfulTime + 5))
                {
                    this.ai.OnLostTrack();
                }
            }
        }
        if (this.firing)
        {
            if (Time.time > (this.lastFireTime + (1 / this.fireFrequency)))
            {
                this.Fire();
            }
        }
    }

    public virtual bool IsAimingAtPlayer()
    {
        Vector3 playerDirection = this.player.position - this.head.position;
        playerDirection.y = 0;
        return Vector3.Angle(this.head.forward, playerDirection) < 15;
    }

    public MechAttackMoveController()
    {
        this.targetDistanceMin = 3f;
        this.targetDistanceMax = 4f;
        this.fireFrequency = 2;
        this.lastFireTime = -1;
    }

}