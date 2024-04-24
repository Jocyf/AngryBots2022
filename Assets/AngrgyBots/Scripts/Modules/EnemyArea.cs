using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class EnemyArea : MonoBehaviour
{
    public System.Collections.Generic.List<GameObject> affected;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.StartCoroutine(this.ActivateAffected(true));
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            this.StartCoroutine(this.ActivateAffected(false));
        }
    }

    public virtual IEnumerator ActivateAffected(bool state)
    {
        foreach (GameObject go in this.affected)
        {
            if (go == null)
            {
                continue;
            }
            go.SetActive(state);
            yield return null;
        }
        foreach (Transform tr in this.transform)
        {
            tr.gameObject.SetActive(state);
            yield return null;
        }
    }

    public virtual void Start()
    {
        this.StartCoroutine(this.ActivateAffected(false));
    }

    public EnemyArea()
    {
        this.affected = new List<GameObject>();
    }

}