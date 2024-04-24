using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpawnAtCheckpoint : MonoBehaviour
{
    public Transform checkpoint;


    public virtual void RespawnPlayer(float delayTime)
    {
        StartCoroutine("_OnSignalInternalTimed", delayTime);
    }

    public IEnumerator _OnSignalInternalTimed(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        OnSignal();
    }


    public virtual void OnSignal()
    {
        transform.position = checkpoint.position;
        transform.rotation = checkpoint.rotation;
        SpawnAtCheckpoint.ResetHealthOnAll();
    }

    public static void ResetHealthOnAll()
    {
        Health[] healthObjects = FindObjectsOfType<Health>();
        foreach (Health health in healthObjects)
        {
            health.dead = false;
            health.health = health.maxHealth;
        }
    }

}