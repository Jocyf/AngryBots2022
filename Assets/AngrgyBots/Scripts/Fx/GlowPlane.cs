using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GlowPlane : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 pos;
    private Vector3 scale;
    public float minGlow = 0.2f;
    public float maxGlow = 0.5f;
    public Color glowColor = Color.white;
    private Material mat;


    public virtual void Start()
    {
        if (!playerTransform) { playerTransform = GameObject.FindWithTag("Player").transform; }
        pos = transform.position;
        scale = transform.localScale;
        mat = GetComponent<Renderer>().material;
        enabled = false;
    }

    public virtual void OnDrawGizmos()
    {
        Color _finalColor = glowColor;
        _finalColor.a = maxGlow * 0.25f;
        Gizmos.color = _finalColor;

        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 scale = 5f * Vector3.Scale(Vector3.one, new Vector3(1f, 0f, 1f));
        Gizmos.DrawCube(Vector3.zero, scale);
        Gizmos.matrix = Matrix4x4.identity;
    }

    public virtual void OnDrawGizmosSelected()
    {

        Color _finalColor = glowColor;
        _finalColor.a = maxGlow;
        Gizmos.color = _finalColor;

        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 scale = 5f * Vector3.Scale(Vector3.one, new Vector3(1f, 0f, 1f));
        Gizmos.DrawCube(Vector3.zero, scale);
        Gizmos.matrix = Matrix4x4.identity;
    }

    public virtual void OnBecameVisible()
    {
        enabled = true;
    }

    public virtual void OnBecameInvisible()
    {
        enabled = false;
    }

    public virtual void Update()
    {
        Vector3 vec = pos - playerTransform.position;
        vec.y = 0f;
        float distance = vec.magnitude;
        transform.localScale = Vector3.Lerp(Vector3.one * minGlow, scale, Mathf.Clamp01(distance * 0.35f));
        //mat.SetColor("_TintColor", glowColor * Mathf.Clamp(distance * 0.1f, minGlow, maxGlow)); // Legacy Material - Shader
        mat.SetColor("_EmissionColor", glowColor * Mathf.Clamp(distance * 0.1f, minGlow, maxGlow));
    }

}