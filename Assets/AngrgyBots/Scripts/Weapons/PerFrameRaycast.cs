using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PerFrameRaycast : MonoBehaviour
{
    private RaycastHit hitInfo;
    private Transform tr;
    public virtual void Awake()
    {
        this.tr = this.transform;
    }

    public virtual void Update()
    {
        // Cast a ray to find out the end point of the laser
        this.hitInfo = default(RaycastHit);
        Physics.Raycast(this.tr.position, this.tr.forward, out this.hitInfo);
    }

    public virtual RaycastHit GetHitInfo()
    {
        return this.hitInfo;
    }

}