using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DestroyObject : MonoBehaviour
{
    public GameObject objectToDestroy;
    public virtual void OnSignal()
    {
        Spawner.Destroy(this.objectToDestroy);
    }

}