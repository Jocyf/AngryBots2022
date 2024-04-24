using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TriggerOnTag : MonoBehaviour
{
    public string triggerTag;
    public SignalSender enterSignals;
    public SignalSender exitSignals;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if ((other.gameObject.tag == this.triggerTag) || (this.triggerTag == ""))
        {
            this.enterSignals.SendSignals(this);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if ((other.gameObject.tag == this.triggerTag) || (this.triggerTag == ""))
        {
            this.exitSignals.SendSignals(this);
        }
    }

    public TriggerOnTag()
    {
        this.triggerTag = "Player";
    }

}