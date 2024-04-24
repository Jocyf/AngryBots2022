using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SlowBulletFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float frequency;
    public float coneAngle;
    public AudioClip fireSound;
    public bool firing;
    private float lastFireTime;
    public virtual void Update()
    {
        if (this.firing)
        {
            if (Time.time > (this.lastFireTime + (1 / this.frequency)))
            {
                this.Fire();
            }
        }
    }

    public virtual void Fire()
    {
        // Spawn bullet
        Quaternion coneRandomRotation = Quaternion.Euler(Random.Range(-this.coneAngle, this.coneAngle), Random.Range(-this.coneAngle, this.coneAngle), 0);
        Spawner.Spawn(this.bulletPrefab, this.transform.position, this.transform.rotation * coneRandomRotation);
        if (this.GetComponent<AudioSource>() && this.fireSound)
        {
            this.GetComponent<AudioSource>().clip = this.fireSound;
            this.GetComponent<AudioSource>().Play();
        }
        this.lastFireTime = Time.time;
    }

    public virtual void OnStartFire()
    {
        this.firing = true;
    }

    public virtual void OnStopFire()
    {
        this.firing = false;
    }

    public SlowBulletFire()
    {
        this.frequency = 2;
        this.coneAngle = 1.5f;
        this.lastFireTime = -1;
    }

}