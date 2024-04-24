using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TriggerOnMouseOrJoystick : MonoBehaviour
{
    public SignalSender mouseDownSignals;
    public SignalSender mouseUpSignals;
    private bool state;
    public virtual void Update()
    {
        if ((this.state == false) && Input.GetMouseButtonDown(0))
        {
            this.mouseDownSignals.SendSignals(this);
            this.state = true;
        }
        else
        {
            if ((this.state == true) && Input.GetMouseButtonUp(0))
            {
                this.mouseUpSignals.SendSignals(this);
                this.state = false;
            }
        }
    }

}