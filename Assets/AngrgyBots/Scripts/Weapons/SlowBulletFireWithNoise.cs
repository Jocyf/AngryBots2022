using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SlowBulletFireWithNoise : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float frequency;
    public float coneAngle;
    public AudioClip fireSound;
    public bool firing;
    public float noisiness;
    private float nextFireNoise;
    private float lastFireTime;
    public virtual void Update()
    {
        if (this.firing)
        {
            if (Time.time > ((this.nextFireNoise + this.lastFireTime) + (1 / this.frequency)))
            {
                this.Fire();
            }
        }
    }

    public virtual void Fire()
    {
        // Spawn visual bullet
        Quaternion coneRandomRotation = Quaternion.Euler(Random.Range(-this.coneAngle, this.coneAngle), Random.Range(-this.coneAngle, this.coneAngle), 0);
        Spawner.Spawn(this.bulletPrefab, this.transform.position, this.transform.rotation * coneRandomRotation);
        if (this.GetComponent<AudioSource>() && this.fireSound)
        {
            this.GetComponent<AudioSource>().clip = this.fireSound;
            this.GetComponent<AudioSource>().Play();
        }
        this.lastFireTime = Time.time;
        this.nextFireNoise = Random.value * this.noisiness;
    }

    public virtual void OnStartFire()
    {
        this.firing = true;
    }

    public virtual void OnStopFire()
    {
        this.firing = false;
    }

    public SlowBulletFireWithNoise()
    {
        this.frequency = 2;
        this.coneAngle = 1.5f;
        this.noisiness = 2f;
        this.nextFireNoise = 1f;
        this.lastFireTime = -1;
    }

}