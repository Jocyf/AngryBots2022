using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SeekerBullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damageAmount;
    public float forceAmount;
    public float radius;
    public float seekPrecision;
    public LayerMask ignoreLayers;
    public float noise;
    public GameObject explosionPrefab;
    private Vector3 dir;
    private float spawnTime;
    private GameObject targetObject;
    private Transform tr;
    private float sideBias;
    public virtual void OnEnable()
    {
        this.tr = this.transform;
        this.dir = this.transform.forward;
        this.targetObject = GameObject.FindWithTag("Player");
        this.spawnTime = Time.time;
        this.sideBias = Mathf.Sin(Time.time * 5);
    }

    public virtual void Update()
    {
        if (Time.time > (this.spawnTime + this.lifeTime))
        {
            Spawner.Destroy(this.gameObject);
        }
        if (this.targetObject)
        {
            Vector3 targetPos = this.targetObject.transform.position;
            targetPos = targetPos + ((this.transform.right * (Mathf.PingPong(Time.time, 1f) - 0.5f)) * this.noise);
            Vector3 targetDir = targetPos - this.tr.position;
            float targetDist = targetDir.magnitude;
            targetDir = targetDir / targetDist;
            if (((Time.time - this.spawnTime) < (this.lifeTime * 0.2f)) && (targetDist > 3))
            {
                targetDir = targetDir + ((this.transform.right * 0.5f) * this.sideBias);
            }
            this.dir = Vector3.Slerp(this.dir, targetDir, Time.deltaTime * this.seekPrecision);
            this.tr.rotation = Quaternion.LookRotation(this.dir);
            this.tr.position = this.tr.position + ((this.dir * this.speed) * Time.deltaTime);
        }
        // Check if this one hits something
        Collider[] hits = Physics.OverlapSphere(this.tr.position, this.radius, ~this.ignoreLayers.value);
        bool collided = false;
        foreach (Collider c in hits)
        {
            // Don't collide with triggers
            if (c.isTrigger)
            {
                continue;
            }
            Health targetHealth = c.GetComponent<Health>();
            if (targetHealth)
            {
                // Apply damage
                targetHealth.OnDamage(this.damageAmount, -this.tr.forward);
            }
            // Get the rigidbody if any
            if (c.GetComponent<Rigidbody>())
            {
                // Apply force to the target object
                Vector3 force = this.tr.forward * this.forceAmount;
                force.y = 0;
                c.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }
            collided = true;
        }
        if (collided)
        {
            Spawner.Destroy(this.gameObject);
            Spawner.Spawn(this.explosionPrefab, this.transform.position, this.transform.rotation);
        }
    }

    public SeekerBullet()
    {
        this.speed = 15f;
        this.lifeTime = 1.5f;
        this.damageAmount = 5;
        this.forceAmount = 5;
        this.radius = 1f;
        this.seekPrecision = 1.3f;
    }

}