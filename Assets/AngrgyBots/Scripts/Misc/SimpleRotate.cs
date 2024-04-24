using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SimpleRotate : MonoBehaviour
{
    public float speed;
    public virtual void OnBecameVisible()
    {
        this.enabled = true;
    }

    public virtual void OnBecameInvisible()
    {
        this.enabled = false;
    }

    public virtual void Update()
    {
        this.transform.Rotate(0f, 0f, Time.deltaTime * this.speed);
    }

    public SimpleRotate()
    {
        this.speed = 4f;
    }

}