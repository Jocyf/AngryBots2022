using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Lock : MonoBehaviour
{
    public bool locked;
    public SignalSender unlockedSignal;
    public virtual void OnSignal()
    {
        if (this.locked)
        {
            this.locked = false;
            this.unlockedSignal.SendSignals(this);
        }
    }

    public Lock()
    {
        this.locked = true;
    }

}