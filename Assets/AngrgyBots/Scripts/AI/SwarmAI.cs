using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuzzerData : object
{
    public Transform transform;
    public MovementMotor motor;
    public AudioSource audio;
    public float charged;
    public bool didChargeEffect;
    public int sign;
    public Material chargeMaterial;
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;
    public ParticleSystem electricBall;
    public LineRenderer electricArc;
}

[System.Serializable]
public class SwarmAI : MonoBehaviour
{
    public MovementMotor[] buzzers;
    public float zapDist;
    public float slowDownDist;
    public float rechargeDist;
    public float chargeTime;
    public float visibleChargeFraction;
    public float nonAttackSpeedFactor;
    public float damageAmount;
    public float minTimeBetweenAttacks;
    public AudioClip zapSound;
    public AudioClip rechargeSound;
    // Private memeber data
    private System.Collections.Generic.List<BuzzerData> buzz;
    private Transform player;
    private bool attacking;
    private int attackerIndex;
    private float nextAttackTime;
    public virtual void Awake()
    {
        this.player = GameObject.FindWithTag("Player").transform;
        this.buzz = new System.Collections.Generic.List<BuzzerData>();
        int i = 0;
        while (i < this.buzzers.Length)
        {
            BuzzerData buzzer = new BuzzerData();
            buzzer.motor = this.buzzers[i];
            if (!this.buzzers[i])
            {
                Debug.Log("buzzer not found at " + i, this.transform);
            }
            buzzer.transform = this.buzzers[i].transform;
            buzzer.audio = this.buzzers[i].GetComponent<AudioSource>();
            buzzer.sign = (i % 2) == 0 ? 1 : -1;
            buzzer.chargeMaterial = buzzer.transform.Find("buzzer_bot/electric_buzzer_plasma").GetComponent<Renderer>().material;
            buzzer.spawnPosition = buzzer.transform.position;
            buzzer.spawnRotation = buzzer.transform.rotation;
            buzzer.electricBall = buzzer.transform.GetComponentInChildren<ParticleSystem>();
            buzzer.electricArc = buzzer.electricBall.GetComponent<LineRenderer>();
            this.buzz.Add(buzzer);
            i++;
        }
        this.buzz[this.attackerIndex].charged = 0.5f;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.transform == this.player)
        {
            this.attacking = true;
            int i = 0;
            while (i < this.buzz.Count)
            {
                this.buzz[i].motor.enabled = true;
                i++;
            }
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.transform == this.player)
        {
            this.attacking = false;
        }
    }

    public virtual void Update()
    {
        int c = this.buzz.Count - 1;
        while (c >= 0)
        {
            if (this.buzz[c].transform == null)
            {
                this.buzz.RemoveAt(c);
                if (this.buzz.Count > 0)
                {
                    this.attackerIndex = this.attackerIndex % this.buzz.Count;
                }
            }
            c--;
        }
        if (this.buzz.Count == 0)
        {
            return;
        }
        if (this.attacking)
        {
            this.UpdateAttack();
        }
        else
        {
            this.UpdateRetreat();
        }
    }

    public virtual void UpdateRetreat()
    {
        int i = 0;
        while (i < this.buzz.Count)
        {
            if (this.buzz[i].motor.enabled)
            {
                Vector3 spawnDir = this.buzz[i].spawnPosition - this.buzz[i].transform.position;
                if (spawnDir.sqrMagnitude > 1)
                {
                    spawnDir.Normalize();
                }
                this.buzz[i].motor.movementDirection = spawnDir * this.nonAttackSpeedFactor;
                this.buzz[i].motor.facingDirection = this.buzz[i].spawnRotation * Vector3.forward;
                if (spawnDir.sqrMagnitude < 0.01f)
                {
                    this.buzz[i].transform.position = this.buzz[i].spawnPosition;
                    this.buzz[i].transform.rotation = this.buzz[i].spawnRotation;
                    this.buzz[i].motor.enabled = false;
                    this.buzz[i].transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    this.buzz[i].transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
            i++;
        }
    }

    public virtual void UpdateAttack()
    {
        Vector3 pos = default(Vector3);
        int count = this.buzz.Count;
        Vector3 attackerDir = this.player.position - this.buzz[this.attackerIndex].transform.position;
        attackerDir.y = 0;
        attackerDir.Normalize();
        // Rotate by 90 degrees the fast way
        Vector3 fleeDir = new Vector3(attackerDir.z, 0, -attackerDir.x);
        int i = 0;
        while (i < count)
        {
            Vector3 playerDir = this.player.position - this.buzz[i].transform.position;
            float playerDist = playerDir.magnitude;
            playerDir = playerDir / playerDist;
            if (i == this.attackerIndex)
            {
                this.buzz[i].motor.facingDirection = playerDir;
                bool aimingCorrect = Vector3.Dot(this.buzz[i].transform.forward, playerDir) > 0.8f;
                if ((!aimingCorrect || (this.buzz[i].charged < 1)) || (Time.time < this.nextAttackTime))
                {
                    if (playerDist < this.rechargeDist)
                    {
                        this.buzz[i].motor.movementDirection = playerDir * -this.nonAttackSpeedFactor;
                    }
                    else
                    {
                        this.buzz[i].motor.movementDirection = Vector3.zero;
                    }
                }
                else
                {
                    this.buzz[i].motor.movementDirection = playerDir;
                    if (playerDist < (this.zapDist + this.slowDownDist))
                    {
                        // Slow down when close;
                        this.buzz[i].motor.movementDirection = this.buzz[i].motor.movementDirection * 0.01f;
                        // Zap when within range
                        if ((playerDist < this.zapDist) && aimingCorrect)
                        {
                            // Zap player here
                            this.StartCoroutine(this.DoElectricArc(this.buzz[i]));
                            // Apply damage
                            Health targetHealth = this.player.GetComponent<Health>();
                            if (targetHealth)
                            {
                                targetHealth.OnDamage(this.damageAmount, -playerDir);
                            }
                            // Change active attacker
                            this.buzz[i].charged = 0;
                            this.attackerIndex = (this.attackerIndex + 1) % count;
                            this.nextAttackTime = Time.time + (this.minTimeBetweenAttacks * Random.Range(1f, 1.2f));
                        }
                    }
                }
            }
            else
            {
                float s = -Mathf.Sign(Vector3.Dot(fleeDir, playerDir));
                Vector3 posSide = (this.player.position + Vector3.Project(-playerDir * playerDist, attackerDir)) + ((fleeDir * s) * this.rechargeDist);
                Vector3 posBehind = this.player.position + (attackerDir * this.rechargeDist);
                float lerp = playerDist / this.rechargeDist;
                lerp = lerp * lerp;
                pos = Vector3.Lerp(posSide, posBehind, lerp * 0.6f);
                if (this.buzz[i].charged == 1)
                {
                    pos = pos + (Vector3.up * 2);
                }
                this.buzz[i].motor.movementDirection = (pos - this.buzz[i].transform.position).normalized * this.nonAttackSpeedFactor;
                if (((i + 1) % count) == this.attackerIndex)
                {
                    this.buzz[i].motor.facingDirection = playerDir;
                }
                else
                {
                    this.buzz[i].motor.facingDirection = this.buzz[i].motor.movementDirection;
                }
            }
            // Recharge
            this.buzz[i].charged = this.buzz[i].charged + (Time.deltaTime / this.chargeTime);
            if (this.buzz[i].charged > 1)
            {
                this.buzz[i].charged = 1;
            }
            float visibleCharged = Mathf.InverseLerp(this.visibleChargeFraction, 1f, this.buzz[i].charged);

            ParticleSystem.MainModule module = buzz[i].electricBall.GetComponent<ParticleSystem.MainModule>();
            ParticleSystem.MinMaxCurve lifeCurve = module.startLifetime;
            lifeCurve.constantMin = 0.3f * visibleCharged;
            lifeCurve.constantMax = 0.45f * visibleCharged;
            //this.buzz[i].electricBall.minSize = 0.3f * visibleCharged;
            //this.buzz[i].electricBall.maxSize = 0.45f * visibleCharged;

            // Play rechage sound
            if (!this.buzz[i].didChargeEffect && (visibleCharged > 0.5f))
            {
                this.buzz[i].audio.clip = this.rechargeSound;
                this.buzz[i].audio.Play();
                this.buzz[i].didChargeEffect = true;
            }
            // Make charged buzzer glow
            this.buzz[i].chargeMaterial.mainTextureOffset = new Vector2(0, (1 - visibleCharged) * -1.9f);
            // Make charged buzzer vibrate
            this.buzz[i].motor.GetComponent<Rigidbody>().angularVelocity = this.buzz[i].motor.GetComponent<Rigidbody>().angularVelocity + ((Random.onUnitSphere * 4) * visibleCharged);
            i++;
        }
    }

    public virtual IEnumerator DoElectricArc(BuzzerData buzz)
    {
        // Play attack sound
        buzz.audio.clip = this.zapSound;
        buzz.audio.Play();
        buzz.didChargeEffect = false;
        // Show electric arc
        buzz.electricArc.enabled = true;
        // Offset  electric arc texture while it's visible
        float stopTime = Time.time + 0.2f;
        while (Time.time < stopTime)
        {
            buzz.electricArc.SetPosition(0, buzz.electricArc.transform.position);
            buzz.electricArc.SetPosition(1, this.player.position);

            {
                float _31 = Random.value;
                Vector2 _32 = buzz.electricArc.sharedMaterial.mainTextureOffset;
                _32.x = _31;
                buzz.electricArc.sharedMaterial.mainTextureOffset = _32;
            }
            yield return null;
        }
        // Hide electric arc
        buzz.electricArc.enabled = false;
    }

    public SwarmAI()
    {
        this.zapDist = 2.3f;
        this.slowDownDist = 1f;
        this.rechargeDist = 5.5f;
        this.chargeTime = 6f;
        this.visibleChargeFraction = 0.8f;
        this.nonAttackSpeedFactor = 0.2f;
        this.damageAmount = 5f;
        this.minTimeBetweenAttacks = 1f;
    }

}