using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ExplosionTrail : MonoBehaviour
{
    private Vector3 _dir;
    public virtual void OnEnable()
    {
        this._dir = Random.onUnitSphere;
        this._dir.y = 1.25f;
    }

    public virtual void Update()
    {
        this.transform.position = this.transform.position + ((this._dir * Time.deltaTime) * 5.5f);
        this._dir.y = this._dir.y - Time.deltaTime;
        if ((this._dir.y < 0f) && (this.transform.position.y <= -1f))
        {
            this.enabled = false;
        }
    }

}