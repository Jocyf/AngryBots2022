using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(BoxCollider))]
public partial class MoodBox : MonoBehaviour
{
    public MoodBoxData data;
    public Cubemap playerReflection;
    private MoodBoxManager manager;
    private Collider myCollider;

    public virtual void Start()
    {
        manager = transform.parent.GetComponent<MoodBoxManager>();
        if (!manager)
        {
            Debug.Log(("Disabled moodbox " + gameObject.name) + " as a MoodBoxManager was not found.", transform);
            enabled = false;
        }

        myCollider = GetComponent<Collider>();
    }

    public virtual void OnDrawGizmos()
    {
        if (transform.parent)
        {
            if(myCollider == null) { myCollider = GetComponent<Collider>();  }
            Gizmos.color = new Color(0.5f, 0.9f, 1f, 0.15f);
            Gizmos.DrawCube(myCollider.bounds.center, myCollider.bounds.size);
        }
    }

    public virtual void OnDrawGizmosSelected()
    {
        if (transform.parent)
        {
            if (myCollider == null) { myCollider = GetComponent<Collider>(); }
            Gizmos.color = new Color(0.5f, 0.9f, 1f, 0.75f);
            Gizmos.DrawCube(myCollider.bounds.center, myCollider.bounds.size);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            ApplyMoodBox();
        }
    }

    public virtual void ApplyMoodBox()
    {
        // optimization: deactivate rain stuff a little earlier
        if (manager.GetData().outside != data.outside)
        {
            foreach (GameObject m in manager.rainManagers)
            {
                m.SetActive(data.outside);
            }
            foreach (GameObject m in manager.splashManagers)
            {
                m.SetActive(data.outside);
            }
        }
        MoodBoxManager.current = this;

        if (manager.playerReflectionMaterials.Length != 0)
        {
            foreach (Material m in manager.playerReflectionMaterials)
            {
                m.SetTexture("_Cube", playerReflection ? playerReflection : manager.defaultPlayerReflection);
            }
        }
    }

}