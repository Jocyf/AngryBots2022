using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ExplosionTestCage : MonoBehaviour
{
    public GameObject explPrefab;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Player")
        {
            GameObject go = UnityEngine.Object.Instantiate(this.explPrefab, this.transform.position, this.transform.rotation);
        }
    }

}