using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class LockedThing : MonoBehaviour
{
    // This component will forward a signal only if all the locks are unlocked
    public Lock[] locks;
    public SignalSender conditionalSignal;
    public virtual void OnSignal()
    {
        bool locked = false;
        foreach (Lock lockObj in this.locks)
        {
            if (lockObj.locked)
            {
                locked = true;
            }
        }
        if (locked == false)
        {
            this.conditionalSignal.SendSignals(this);
        }
    }

}