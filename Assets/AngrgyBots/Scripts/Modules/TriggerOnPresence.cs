using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TriggerOnPresence : MonoBehaviour
{
    public SignalSender enterSignals;
    public SignalSender exitSignals;
    public System.Collections.Generic.List<GameObject> objects;
    public virtual void Awake()
    {
        this.objects = new System.Collections.Generic.List<GameObject>();
        this.enabled = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        bool wasEmpty = this.objects.Count == 0;
        this.objects.Add(other.gameObject);
        if (wasEmpty)
        {
            this.enterSignals.SendSignals(this);
            this.enabled = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (this.objects.Contains(other.gameObject))
        {
            this.objects.Remove(other.gameObject);
        }
        if (this.objects.Count == 0)
        {
            this.exitSignals.SendSignals(this);
            this.enabled = false;
        }
    }

}