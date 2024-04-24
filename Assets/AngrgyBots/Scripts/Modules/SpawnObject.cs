using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpawnObject : MonoBehaviour
{
    public GameObject objectToSpawn;
    public SignalSender onDestroyedSignals;
    private GameObject spawned;
    // Keep disabled from the beginning
    // When we get a signal, spawn the objectToSpawn and store the spawned object.
    // Also enable this behaviour so the Update function will be run.
    public virtual void OnSignal()
    {
        this.spawned = Spawner.Spawn(this.objectToSpawn, this.transform.position, this.transform.rotation);
        if (this.onDestroyedSignals.receivers.Length > 0)
        {
            this.enabled = true;
        }
    }

    // After the object is spawned, check each frame if it's still there.
    // Once it's not, activate the onDestroyedSignals and disable again.
    public virtual void Update()
    {
        if ((this.spawned == null) || (this.spawned.activeInHierarchy == false))
        {
            this.onDestroyedSignals.SendSignals(this);
            this.enabled = false;
        }
    }

    public virtual void Start()
    {
        this.enabled = false;
    }

}