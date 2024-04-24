using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CopyRotation : MonoBehaviour
{
    public Transform sourceRotation;
    public Vector3 addLocalRotation;
    public virtual void LateUpdate()
    {
        this.transform.rotation = this.sourceRotation.rotation;
        this.transform.localRotation = this.transform.localRotation * Quaternion.Euler(this.addLocalRotation);
    }

}