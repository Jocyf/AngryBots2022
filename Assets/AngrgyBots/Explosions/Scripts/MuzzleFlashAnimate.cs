using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class MuzzleFlashAnimate : MonoBehaviour
{
    public virtual void Update()
    {
        this.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);

        {
            float _27 = Random.Range(0, 90f);
            Vector3 _28 = this.transform.localEulerAngles;
            _28.z = _27;
            this.transform.localEulerAngles = _28;
        }
    }

}