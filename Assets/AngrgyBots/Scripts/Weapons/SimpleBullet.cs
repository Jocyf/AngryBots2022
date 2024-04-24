using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SimpleBullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float dist;
    private float spawnTime;
    private Transform tr;
    public virtual void OnEnable()
    {
        this.tr = this.transform;
        this.spawnTime = Time.time;
    }

    public virtual void Update()
    {
        this.tr.position = this.tr.position + ((this.tr.forward * this.speed) * Time.deltaTime);
        this.dist = this.dist - (this.speed * Time.deltaTime);
        if ((Time.time > (this.spawnTime + this.lifeTime)) || (this.dist < 0))
        {
            Spawner.Destroy(this.gameObject);
        }
    }

    public SimpleBullet()
    {
        this.speed = 10;
        this.lifeTime = 0.5f;
        this.dist = 10000;
    }

}