using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpawnMultipleObjects : MonoBehaviour
{
    // (#pragma downcast is required to iterate through children of a transform)
    // The object prefab will be spawned on the locations of each of the child transforms of this object
    public GameObject objectToSpawn;
    public float delayInBetween;
    public SignalSender onDestroyedSignals;
    private System.Collections.Generic.List<GameObject> spawned;
    // Keep disabled from the beginning
    // When we get a signal, spawn the objectToSpawn objects and store them.
    // Also enable this behaviour so the Update function will be run.
    public virtual IEnumerator OnSignal()
    {
        foreach (Transform child in this.transform)
        {
            // Spawn with the position and rotation of the child transform
            this.spawned.Add(Spawner.Spawn(this.objectToSpawn, child.position, child.rotation));
            // Delay
            yield return new WaitForSeconds(this.delayInBetween);
        }
        this.enabled = true;
    }

    // After the objects are spawned, check each frame if they're still there.
    // Once they're not,
    public virtual void Update()
    {
        // Once the list is empty, activate the onDestroyedSignals and disable again.
        if (this.spawned.Count == 0)
        {
            this.onDestroyedSignals.SendSignals(this);
            this.enabled = false;
        }
        else
        {
            // As long as the list is not empty, check if the first object in the list
            // has been destroyed, and remove it from the list if it has.
            // We don't need to check the rest of the list. All of the entries will
            // end up being the first one eventually.
            // Note that only one object can be removed per frame, so if there's
            // a really high amount, there may be a slight delay before the list is empty.
            if ((this.spawned[0] == null) || (this.spawned[0].activeInHierarchy == false))
            {
                this.spawned.RemoveAt(0);
            }
        }
    }

    public virtual void Start()
    {
        this.enabled = false;
    }

    public SpawnMultipleObjects()
    {
        this.spawned = new List<GameObject>();
    }

}