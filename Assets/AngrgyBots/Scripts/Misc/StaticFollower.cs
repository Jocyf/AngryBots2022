using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class StaticFollower : MonoBehaviour
{
    public Transform target;
    private Vector3 relativePos;
    public virtual void Awake()
    {
        this.relativePos = this.transform.position - this.target.position;
    }

    public virtual void LateUpdate()
    {
        this.transform.position = this.target.position + this.relativePos;
    }

}