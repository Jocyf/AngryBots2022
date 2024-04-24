using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TriggerOnMouse : MonoBehaviour
{
    public SignalSender mouseDownSignals;
    public SignalSender mouseUpSignals;
    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.mouseDownSignals.SendSignals(this);
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.mouseUpSignals.SendSignals(this);
        }
    }

}