using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PatrolPoint : MonoBehaviour
{
    public Vector3 position;
    public virtual void Awake()
    {
        this.position = this.transform.position;
    }

}