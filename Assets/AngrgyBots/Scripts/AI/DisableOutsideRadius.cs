using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(SphereCollider))]
public partial class DisableOutsideRadius : MonoBehaviour
{
    private GameObject target;
    private SphereCollider sphereCollider;
    private float activeRadius;

    public virtual void Awake()
    {
        this.target = this.transform.parent.gameObject;
        this.sphereCollider = this.GetComponent<SphereCollider>();
        this.activeRadius = this.sphereCollider.radius;
        this.Disable();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player") && (this.target.transform.parent == this.transform))
        {
            this.Enable();
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            this.Disable();
        }
    }

    public virtual void Disable()
    {
        this.transform.parent = this.target.transform.parent;
        this.target.transform.parent = this.transform;
        this.target.SetActive(false);
        this.sphereCollider.radius = this.activeRadius;
    }

    public virtual void Enable()
    {
        this.target.transform.parent = this.transform.parent;
        this.target.SetActive(true);
        this.transform.parent = this.target.transform;
        this.sphereCollider.radius = this.activeRadius * 1.1f;
    }

}