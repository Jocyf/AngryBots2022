using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableLight : MonoBehaviour
{
    private Light _light;
    public float disableTime = 1f;


    public void OnDisable()
    {
        if (_light.enabled) { StartCoroutine("_DisableParticleTimed"); }
    }

    private void Start()
    {
        _light = GetComponent<Light>();
        _light.enabled = false;
    }

    private IEnumerator _DisableParticleTimed()
    {
        yield return new WaitForSeconds(disableTime);
        _light.enabled = false;
    }
}
