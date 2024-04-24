using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ActivatePanelByClickSimple : MonoBehaviour
{
    public GameObject panelToActivate;

    public void SetPanelActive(bool _active)
    {
        if (panelToActivate != null) { panelToActivate.SetActive(_active); }
    }

}