using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SetCheckpoint : MonoBehaviour
{
    public Transform spawnTransform;
    public virtual void OnTriggerEnter(Collider other)
    {
        SpawnAtCheckpoint checkpointKeeper = other.GetComponent<SpawnAtCheckpoint>() as SpawnAtCheckpoint;
        checkpointKeeper.checkpoint = this.spawnTransform;
    }

}