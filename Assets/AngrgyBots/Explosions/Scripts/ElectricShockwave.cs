using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ElectricShockwave : MonoBehaviour
{
    public float autoDisableAfter;
    public virtual void OnEnable()
    {
        this.StartCoroutine(this.DeactivateCoroutine(this.autoDisableAfter));
    }

    public virtual IEnumerator DeactivateCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        this.gameObject.SetActive(false);
    }

    public ElectricShockwave()
    {
        this.autoDisableAfter = 2f;
    }

}