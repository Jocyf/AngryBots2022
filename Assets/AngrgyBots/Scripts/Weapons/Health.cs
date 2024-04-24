using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public partial class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;
    public float regenerateSpeed;
    public bool invincible;
    public bool dead;
    public GameObject damagePrefab;
    public Transform damageEffectTransform;
    public float damageEffectMultiplier = 1f;
    public bool damageEffectCentered = true;
    public GameObject scorchMarkPrefab;
    private GameObject scorchMark;
    public SignalSender damageSignals;
    public SignalSender dieSignals;
    public UnityEvent damageEvents; /**/
    public UnityEvent dieEvents;
    private float lastDamageTime;
    private ParticleSystem damageEffect;
    private float damageEffectCenterYOffset;
    private float colliderRadiusHeuristic = 1f;

    public virtual void Awake()
    {
        this.enabled = false;
        if (this.damagePrefab)
        {
            if (this.damageEffectTransform == null)
            {
                this.damageEffectTransform = this.transform;
            }
            GameObject effect = Spawner.Spawn(this.damagePrefab, Vector3.zero, Quaternion.identity);
            effect.transform.parent = this.damageEffectTransform;
            effect.transform.localPosition = Vector3.zero;
            this.damageEffect = effect.GetComponent<ParticleSystem>();
            Vector2 tempSize = new Vector2(this.GetComponent<Collider>().bounds.extents.x, this.GetComponent<Collider>().bounds.extents.z);
            this.colliderRadiusHeuristic = tempSize.magnitude * 0.5f;
            this.damageEffectCenterYOffset = this.GetComponent<Collider>().bounds.extents.y;
        }
        if (this.scorchMarkPrefab)
        {
            this.scorchMark = GameObject.Instantiate(this.scorchMarkPrefab, Vector3.zero, Quaternion.identity);
            this.scorchMark.SetActive(false);
        }
    }

    public virtual void OnDamage(float amount, Vector3 fromDirection)
    {
        // Take no damage if invincible, dead, or if the damage is zero
        if (this.invincible)
        {
            return;
        }
        if (this.dead)
        {
            return;
        }
        if (amount <= 0)
        {
            return;
        }
        // Decrease health by damage and send damage signals
        // @HACK: this hack will be removed for the final game
        //  but makes playing and showing certain areas in the
        //  game a lot easier
        /*

	if(gameObject.tag != "Player")
		amount *= 10.0;

	*/
        this.health = this.health - amount;

        damageEvents.Invoke();
        //this.damageSignals.SendSignals(this); /**/

        this.lastDamageTime = Time.time;
        // Enable so the Update function will be called
        // if regeneration is enabled
        if (this.regenerateSpeed > 0)
        {
            this.enabled = true;
        }
        // Show damage effect if there is one
        if (this.damageEffect)
        {
            this.damageEffect.transform.rotation = Quaternion.LookRotation(fromDirection, Vector3.up);
            if (!this.damageEffectCentered)
            {
                Vector3 dir = fromDirection;
                dir.y = 0f;
                this.damageEffect.transform.position = (this.transform.position + (Vector3.up * this.damageEffectCenterYOffset)) + (this.colliderRadiusHeuristic * dir);
            }
            // @NOTE: due to popular demand (ethan, storm) we decided
            // to make the amount damage independent ...
            //var particleAmount = Random.Range (damageEffect.minEmission, damageEffect.maxEmission + 1);
            //particleAmount = particleAmount * amount * damageEffectMultiplier;

            //ParticleSystem.EmissionModule module = damageEffect.GetComponent<ParticleSystem.EmissionModule>();
            this.damageEffect.Play();// (particleAmount);
        }
        // Die if no health left
        if (this.health <= 0)
        {
            GameScore.RegisterDeath(this.gameObject);
            this.health = 0;
            this.dead = true;
            this.dieSignals.SendSignals(this);
            this.enabled = false;
            // scorch marks
            if (this.scorchMark)
            {
                this.scorchMark.SetActive(true);
                // @NOTE: maybe we can justify a raycast here so we can place the mark
                // on slopes with proper normal alignments
                // @TODO: spawn a yield Sub() to handle placement, as we can
                // spread calculations over several frames => cheap in total
                Vector3 scorchPosition = this.GetComponent<Collider>().ClosestPointOnBounds(this.transform.position - (Vector3.up * 100));
                this.scorchMark.transform.position = scorchPosition + (Vector3.up * 0.1f);

                {
                    float _73 = Random.Range(0f, 90f);
                    Vector3 _74 = this.scorchMark.transform.eulerAngles;
                    _74.y = _73;
                    this.scorchMark.transform.eulerAngles = _74;
                }
            }
        }
    }

    public virtual void OnEnable()
    {
        this.StartCoroutine(this.Regenerate());
    }

    // Regenerate health
    public virtual IEnumerator Regenerate()
    {
        if (this.regenerateSpeed > 0f)
        {
            while (this.enabled)
            {
                if (Time.time > (this.lastDamageTime + 3))
                {
                    this.health = this.health + this.regenerateSpeed;
                    yield return null;
                    if (this.health >= this.maxHealth)
                    {
                        this.health = this.maxHealth;
                        this.enabled = false;
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }



}