using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class RenderFogPlane : MonoBehaviour
{
    public Camera cameraForRay;

    private Matrix4x4 frustumCorners;
    private float CAMERA_ASPECT_RATIO = 1.333333f;
    private float CAMERA_NEAR;
    private float CAMERA_FAR;
    private float CAMERA_FOV;
    private Mesh mesh;
    private Vector2[] uv = new Vector2[4];
    private Renderer _renderer;


    public virtual void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = true;   /**/
        if (!mesh)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }
        // write indices into uv's for fast world space reconstruction
        if (mesh)
        {
            uv[0] = new Vector2(1f, 1f); // TR
            uv[1] = new Vector2(0f, 0f); // TL
            uv[2] = new Vector2(2f, 2f); // BR
            uv[3] = new Vector2(3f, 3f); // BL
            mesh.uv = uv;
        }
        if (!cameraForRay)
        {
            cameraForRay = Camera.main;
        }
    }

    private bool EarlyOutIfNotSupported()
    {
        if (!Supported())
        {
            enabled = false;
            return true;
        }
        return false;
    }

    public virtual void OnDisable()
    {
        _renderer.enabled = false;
    }

    public virtual bool Supported()
    {
        return ((_renderer.sharedMaterial.shader.isSupported && SystemInfo.supportsImageEffects) && SystemInfo.supportsRenderTextures) && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
    }

    public virtual void Update()
    {
        Ray ray = new Ray();
        Vector4 vec = new Vector4();
        Vector3 corner = new Vector3();

        if (EarlyOutIfNotSupported()) { enabled = false; return; }

        if (!_renderer.enabled) { return; }

        frustumCorners = Matrix4x4.identity;
        CAMERA_NEAR = cameraForRay.nearClipPlane;
        CAMERA_FAR = cameraForRay.farClipPlane;
        CAMERA_FOV = cameraForRay.fieldOfView;
        CAMERA_ASPECT_RATIO = cameraForRay.aspect;
        float fovWHalf = CAMERA_FOV * 0.5f;
        Vector3 toRight = ((cameraForRay.transform.right * CAMERA_NEAR) * Mathf.Tan(fovWHalf * Mathf.Deg2Rad)) * CAMERA_ASPECT_RATIO;
        Vector3 toTop = (cameraForRay.transform.up * CAMERA_NEAR) * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);
        Vector3 topLeft = ((cameraForRay.transform.forward * CAMERA_NEAR) - toRight) + toTop;
        float CAMERA_SCALE = (topLeft.magnitude * CAMERA_FAR) / CAMERA_NEAR;

        // correctly place transform first
        Vector3 _positionAux = transform.localPosition;
        _positionAux.z = CAMERA_NEAR + 0.0001f;
        transform.localPosition = _positionAux;

        Vector3 _ScaleAux = transform.localScale;
        _ScaleAux.x = (toRight * 0.5f).magnitude;
        _ScaleAux.y = 1f;
        _ScaleAux.z = (toTop * 0.5f).magnitude;
        transform.localScale = _ScaleAux;

        Quaternion _rotationAux = transform.localRotation;
        _rotationAux.eulerAngles = new Vector3(270f, 0f, 0f);
        transform.localRotation = _rotationAux;

        // write view frustum corner "rays"
        topLeft.Normalize();
        topLeft = topLeft * CAMERA_SCALE;
        Vector3 topRight = ((cameraForRay.transform.forward * CAMERA_NEAR) + toRight) + toTop;
        topRight.Normalize();
        topRight = topRight * CAMERA_SCALE;
        Vector3 bottomRight = ((cameraForRay.transform.forward * CAMERA_NEAR) + toRight) - toTop;
        bottomRight.Normalize();
        bottomRight = bottomRight * CAMERA_SCALE;
        Vector3 bottomLeft = ((cameraForRay.transform.forward * CAMERA_NEAR) - toRight) - toTop;
        bottomLeft.Normalize();
        bottomLeft = bottomLeft * CAMERA_SCALE;
        frustumCorners.SetRow(0, topLeft);
        frustumCorners.SetRow(1, topRight);
        frustumCorners.SetRow(2, bottomRight);
        frustumCorners.SetRow(3, bottomLeft);
        _renderer.sharedMaterial.SetMatrix("_FrustumCornersWS", frustumCorners);
        _renderer.sharedMaterial.SetVector("_CameraWS", cameraForRay.transform.position);
    }

}