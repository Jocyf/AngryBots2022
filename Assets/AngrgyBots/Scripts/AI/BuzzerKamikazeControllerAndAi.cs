using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class BuzzerKamikazeControllerAndAi : MonoBehaviour
{
    // Public member data
    public MovementMotor motor;
    public LineRenderer electricArc;
    public AudioClip zapSound;
    public float damageAmount;
    private Transform player;
    private Transform character;
    private Vector3 spawnPos;
    private float startTime;
    private bool threatRange;
    private Vector3 direction;
    private float rechargeTimer;
    private AudioSource audioSource;
    private Vector3 zapNoise;
    public virtual void Awake()
    {
        this.character = this.motor.transform;
        this.player = GameObject.FindWithTag("Player").transform;
        this.spawnPos = this.character.position;
        this.audioSource = this.GetComponent<AudioSource>();
    }

    public virtual void Start()
    {
        this.startTime = Time.time;
        this.motor.movementTarget = this.spawnPos;
        this.threatRange = false;
    }

    public virtual void Update()
    {
        this.motor.movementTarget = this.player.position;
        this.direction = this.player.position - this.character.position;
        this.threatRange = false;
        if (this.direction.magnitude < 2f)
        {
            this.threatRange = true;
            this.motor.movementTarget = Vector3.zero;
        }
        this.rechargeTimer = this.rechargeTimer - Time.deltaTime;
        if (((this.rechargeTimer < 0f) && this.threatRange) && (Vector3.Dot(this.character.forward, this.direction) > 0.8f))
        {
            this.zapNoise = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * 0.5f;
            Health targetHealth = this.player.GetComponent<Health>();
            if (targetHealth)
            {
                Vector3 playerDir = this.player.position - this.character.position;
                float playerDist = playerDir.magnitude;
                playerDir = playerDir / playerDist;
                targetHealth.OnDamage(this.damageAmount / (1f + this.zapNoise.magnitude), -playerDir);
            }
            this.StartCoroutine(this.DoElectricArc());
            this.rechargeTimer = Random.Range(1f, 2f);
        }
    }

    public virtual IEnumerator DoElectricArc()
    {
        if (this.electricArc.enabled)
        {
            yield break;
        }
        // Play attack sound
        this.audioSource.clip = this.zapSound;
        this.audioSource.Play();
        //buzz.didChargeEffect = false;
        // Show electric arc
        this.electricArc.enabled = true;
        this.zapNoise = this.transform.rotation * this.zapNoise;
        // Offset  electric arc texture while it's visible
        float stopTime = Time.time + 0.2f;
        while (Time.time < stopTime)
        {
            this.electricArc.SetPosition(0, this.electricArc.transform.position);
            this.electricArc.SetPosition(1, this.player.position + this.zapNoise);

            {
                float _29 = Random.value;
                Vector2 _30 = this.electricArc.sharedMaterial.mainTextureOffset;
                _30.x = _29;
                this.electricArc.sharedMaterial.mainTextureOffset = _30;
            }
            yield return null;
        }
        // Hide electric arc
        this.electricArc.enabled = false;
    }

    public BuzzerKamikazeControllerAndAi()
    {
        this.damageAmount = 5f;
        this.rechargeTimer = 1f;
        this.zapNoise = Vector3.zero;
    }

}