using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpiderAttackMoveController : MonoBehaviour
{
    // Public member data
    public MovementMotor motor;
    public float targetDistanceMin = 2f;
    public float targetDistanceMax = 3f;
    public float proximityDistance = 4f;
    public float damageRadius = 5f;
    public float proximityBuildupTime = 2f;
    public float proximityOfNoReturn = 0.6f;
    public float damageAmount = 30f;
    
    [Space(10)]
    public Renderer proximityRenderer;
    public AudioSource audioSource;
    public SelfIlluminationBlink[] blinkComponents;
    public GlowPlane blinkPlane;
    public GameObject intentionalExplosion;
    public MonoBehaviour animationBehaviour;

    // Private memeber data
    private AI ai;
    private Transform character;
    private Transform player;
    private bool inRange;
    private float nextRaycastTime;
    private float lastRaycastSuccessfulTime;
    private float proximityLevel;
    private float lastBlinkTime;
    private float noticeTime;

    public virtual void Awake()
    {
        this.character = this.motor.transform;
        this.player = GameObject.FindWithTag("Player").transform;
        this.ai = this.transform.parent.GetComponentInChildren<AI>();
        if (this.blinkComponents.Length == 0)
        {
            this.blinkComponents = this.transform.parent.GetComponentsInChildren<SelfIlluminationBlink>();
        }
    }

    public virtual void OnEnable()
    {
        this.inRange = false;
        this.nextRaycastTime = Time.time;
        this.lastRaycastSuccessfulTime = Time.time;
        this.noticeTime = Time.time;
        this.animationBehaviour.enabled = true;
        if (this.blinkPlane)
        {
            this.blinkPlane.GetComponent<Renderer>().enabled = false;
        }
    }

    public virtual void OnDisable()
    {
        if (this.proximityRenderer == null)
        {
            Debug.LogError("proximityRenderer is null", this);
        }
        else
        {
            if (this.proximityRenderer.material == null)
            {
                Debug.LogError("proximityRenderer.material is null", this);
            }
            else
            {
                this.proximityRenderer.material.color = Color.white;
            }
        }
        if (this.blinkPlane)
        {
            this.blinkPlane.GetComponent<Renderer>().enabled = false;
        }
    }

    public virtual void Update()
    {
        if (Time.time < (this.noticeTime + 0.7f))
        {
            this.motor.movementDirection = Vector3.zero;
            return;
        }
        // Calculate the direction from the player to this character
        Vector3 playerDirection = this.player.position - this.character.position;
        playerDirection.y = 0;
        float playerDist = playerDirection.magnitude;
        playerDirection = playerDirection / playerDist;
        // Set this character to face the player,
        // that is, to face the direction from this character to the player
        //motor.facingDirection = playerDirection;
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
        if (((playerDist < this.proximityDistance) && (Time.time < (this.lastRaycastSuccessfulTime + 1))) || (this.proximityLevel > this.proximityOfNoReturn))
        {
            this.proximityLevel = this.proximityLevel + (Time.deltaTime / this.proximityBuildupTime);
        }
        else
        {
            this.proximityLevel = this.proximityLevel - (Time.deltaTime / this.proximityBuildupTime);
        }
        this.proximityLevel = Mathf.Clamp01(this.proximityLevel);
        //proximityRenderer.material.color = Color.Lerp (Color.blue, Color.red, proximityLevel);
        if (this.proximityLevel == 1)
        {
            this.Explode();
        }
        if (Time.time > this.nextRaycastTime)
        {
            this.nextRaycastTime = Time.time + 1;
            if (this.ai.CanSeePlayer())
            {
                this.lastRaycastSuccessfulTime = Time.time;
            }
            else
            {
                if (Time.time > (this.lastRaycastSuccessfulTime + 2))
                {
                    this.ai.OnLostTrack();
                }
            }
        }
        float deltaBlink = 1 / Mathf.Lerp(2, 15, this.proximityLevel);
        if (Time.time > (this.lastBlinkTime + deltaBlink))
        {
            this.lastBlinkTime = Time.time;
            this.proximityRenderer.material.color = Color.red;
            if (this.audioSource.enabled)
            {
                this.audioSource.Play();
            }
            foreach (SelfIlluminationBlink comp in this.blinkComponents)
            {
                comp.Blink();
            }
            if (this.blinkPlane)
            {
                this.blinkPlane.GetComponent<Renderer>().enabled = !this.blinkPlane.GetComponent<Renderer>().enabled;
            }
        }
        if (Time.time > (this.lastBlinkTime + 0.04f))
        {
            this.proximityRenderer.material.color = Color.white;
        }
    }

    public virtual void Explode()
    {
        float damageFraction = 1 - (Vector3.Distance(this.player.position, this.character.position) / this.damageRadius);
        Health targetHealth = this.player.GetComponent<Health>();
        if (targetHealth)
        {
            // Apply damage
            targetHealth.OnDamage(this.damageAmount * damageFraction, this.character.position - this.player.position);
        }
        this.player.GetComponent<Rigidbody>().AddExplosionForce(10, this.character.position, this.damageRadius, 0f, ForceMode.Impulse);
        Spawner.Spawn(this.intentionalExplosion, this.transform.position, Quaternion.identity);
        Spawner.Destroy(this.character.gameObject);
    }

    
}