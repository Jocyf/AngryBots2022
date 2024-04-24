using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Health))]
public partial class TerminalHack : MonoBehaviour
{
    private Health health;
    private Animation animationComp;
    public virtual void Start()
    {
        this.health = this.GetComponent<Health>();
        this.animationComp = this.GetComponentInChildren<Animation>();
        this.UpdateHackingProgress();
        this.enabled = false;
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.health.OnDamage(Time.deltaTime, Vector3.zero);
        }
    }

    public virtual void OnHacking()
    {
        this.enabled = true;
        this.UpdateHackingProgress();
    }

    public virtual void OnHackingCompleted()
    {
        this.GetComponent<AudioSource>().Play();
        this.animationComp.Stop();
        this.enabled = false;
    }

    public virtual void UpdateHackingProgress()
    {
        this.animationComp.clip.SampleAnimation(this.animationComp.gameObject, (1 - (this.health.health / this.health.maxHealth)) * this.animationComp.clip.length);
    }

    public virtual void Update()
    {
        this.UpdateHackingProgress();
        if ((this.health.health == 0) || (this.health.health == this.health.maxHealth))
        {
            this.UpdateHackingProgress();
            this.enabled = false;
        }
    }

}