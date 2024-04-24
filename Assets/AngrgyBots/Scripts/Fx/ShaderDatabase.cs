using UnityEngine;
using System.Collections;

[System.Serializable]
// ShaderDatabase
// knows and eventually "cooks" shaders in the beginning of the game (see CookShaders),
// also knows some tricks to hide the frame buffer with white and/or black planes
// to hide loading artefacts or shader cooking process
[UnityEngine.RequireComponent(typeof(Camera))]
public partial class ShaderDatabase : MonoBehaviour
{
    public Shader[] shaders;
    public bool cookShadersOnMobiles;
    public Material cookShadersCover;
    private GameObject cookShadersObject;
    public virtual void Awake()
    {
    }

    public virtual GameObject CreateCameraCoverPlane()
    {
        this.cookShadersObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.cookShadersObject.GetComponent<Renderer>().material = this.cookShadersCover;
        this.cookShadersObject.transform.parent = this.transform;
        this.cookShadersObject.transform.localPosition = Vector3.zero;

        {
            float _57 = this.cookShadersObject.transform.localPosition.z + 1.55f;
            Vector3 _58 = this.cookShadersObject.transform.localPosition;
            _58.z = _57;
            this.cookShadersObject.transform.localPosition = _58;
        }
        this.cookShadersObject.transform.localRotation = Quaternion.identity;

        {
            float _59 = this.cookShadersObject.transform.localEulerAngles.z + 180;
            Vector3 _60 = this.cookShadersObject.transform.localEulerAngles;
            _60.z = _59;
            this.cookShadersObject.transform.localEulerAngles = _60;
        }
        this.cookShadersObject.transform.localScale = Vector3.one * 1.5f;

        {
            float _61 = this.cookShadersObject.transform.localScale.x * 1.6f;
            Vector3 _62 = this.cookShadersObject.transform.localScale;
            _62.x = _61;
            this.cookShadersObject.transform.localScale = _62;
        }
        return this.cookShadersObject;
    }

    public virtual IEnumerator WhiteOut()
    {
        this.CreateCameraCoverPlane();
        Material mat = this.cookShadersObject.GetComponent<Renderer>().sharedMaterial;
        mat.SetColor("_TintColor", new Color(1f, 1f, 1f, 0f));
        yield return null;
        Color c = new Color(1f, 1f, 1f, 0f);
        while (c.a < 1f)
        {
            c.a = c.a + (Time.deltaTime * 0.25f);
            mat.SetColor("_TintColor", c);
            yield return null;
        }
        this.DestroyCameraCoverPlane();
    }

    public virtual IEnumerator WhiteIn()
    {
        this.CreateCameraCoverPlane();
        Material mat = this.cookShadersObject.GetComponent<Renderer>().sharedMaterial;
        mat.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));
        yield return null;
        Color c = new Color(1f, 1f, 1f, 1f);
        while (c.a > 0f)
        {
            c.a = c.a - (Time.deltaTime * 0.25f);
            mat.SetColor("_TintColor", c);
            yield return null;
        }
        this.DestroyCameraCoverPlane();
    }

    public virtual void DestroyCameraCoverPlane()
    {
        if (this.cookShadersObject)
        {
            UnityEngine.Object.DestroyImmediate(this.cookShadersObject);
        }
        this.cookShadersObject = null;
    }

    public virtual void Start()
    {
    }

    // this function is cooking all shaders to be used in the game. 
    // it's good practice to draw all of them in order to avoid
    // triggering in game shader compilations which might cause evil
    // frame time spikes
    // currently only enabled for mobile (iOS, Android, Windows Phone and BlackBerry) platforms
    public virtual IEnumerator CookShaders()
    {
        if (this.shaders.Length != 0)
        {
            Material m = new Material(this.shaders[0]);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = this.transform;
            cube.transform.localPosition = Vector3.zero;

            {
                float _63 = cube.transform.localPosition.z + 4f;
                Vector3 _64 = cube.transform.localPosition;
                _64.z = _63;
                cube.transform.localPosition = _64;
            }
            yield return null;
            foreach (Shader s in this.shaders)
            {
                if (s)
                {
                    m.shader = s;
                    cube.GetComponent<Renderer>().material = m;
                }
                yield return null;
            }
            UnityEngine.Object.Destroy(m);
            UnityEngine.Object.Destroy(cube);
            yield return null;
            Color c = Color.black;
            c.a = 1f;
            while (c.a > 0f)
            {
                c.a = c.a - (Time.deltaTime * 0.5f);
                this.cookShadersCover.SetColor("_TintColor", c);
                yield return null;
            }
        }
        this.DestroyCameraCoverPlane();
    }

    public ShaderDatabase()
    {
        this.cookShadersOnMobiles = true;
    }

}