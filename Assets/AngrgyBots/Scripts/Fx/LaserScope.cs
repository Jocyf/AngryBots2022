using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(PerFrameRaycast))]
public partial class LaserScope : MonoBehaviour
{
    public float scrollSpeed;
    public float pulseSpeed;
    public float noiseSize;
    public float maxWidth;
    public float minWidth;
    public GameObject pointer;
    private LineRenderer lRenderer;
    private float aniTime;
    private float aniDir;
    private PerFrameRaycast raycast;
    public virtual void Start()
    {
        this.lRenderer = ((LineRenderer) this.gameObject.GetComponent(typeof(LineRenderer))) as LineRenderer;
        this.aniTime = 0f;
        // Change some animation values here and there
        this.StartCoroutine(this.ChoseNewAnimationTargetCoroutine());
        this.raycast = this.GetComponent<PerFrameRaycast>();
    }

    public virtual IEnumerator ChoseNewAnimationTargetCoroutine()
    {
        while (true)
        {
            this.aniDir = (this.aniDir * 0.9f) + (Random.Range(0.5f, 1.5f) * 0.1f);
            yield return null;
            this.minWidth = (this.minWidth * 0.8f) + (Random.Range(0.1f, 1f) * 0.2f);
            yield return new WaitForSeconds((1f + (Random.value * 2f)) - 1f);
        }
    }

    public virtual void Update()
    {

        {
            float _39 = this.GetComponent<Renderer>().material.mainTextureOffset.x + ((Time.deltaTime * this.aniDir) * this.scrollSpeed);
            Vector2 _40 = this.GetComponent<Renderer>().material.mainTextureOffset;
            _40.x = _39;
            this.GetComponent<Renderer>().material.mainTextureOffset = _40;
        }
        this.GetComponent<Renderer>().material.SetTextureOffset("_NoiseTex", new Vector2((-Time.time * this.aniDir) * this.scrollSpeed, 0f));
        float aniFactor = Mathf.PingPong(Time.time * this.pulseSpeed, 1f);
        aniFactor = Mathf.Max(this.minWidth, aniFactor) * this.maxWidth;
        //this.lRenderer.SetWidth(aniFactor, aniFactor);
        this.lRenderer.startWidth = aniFactor;
        this.lRenderer.endWidth = aniFactor;

        // Cast a ray to find out the end point of the laser
        RaycastHit hitInfo = this.raycast.GetHitInfo();
        if (hitInfo.transform)
        {
            this.lRenderer.SetPosition(1, hitInfo.distance * Vector3.forward);

            {
                float _41 = 0.1f * hitInfo.distance;
                Vector2 _42 = this.GetComponent<Renderer>().material.mainTextureScale;
                _42.x = _41;
                this.GetComponent<Renderer>().material.mainTextureScale = _42;
            }
            this.GetComponent<Renderer>().material.SetTextureScale("_NoiseTex", new Vector2((0.1f * hitInfo.distance) * this.noiseSize, this.noiseSize));
            // Use point and normal to align a nice & rough hit plane
            if (this.pointer)
            {
                this.pointer.GetComponent<Renderer>().enabled = true;
                this.pointer.transform.position = hitInfo.point + ((this.transform.position - hitInfo.point) * 0.01f);
                this.pointer.transform.rotation = Quaternion.LookRotation(hitInfo.normal, this.transform.up);

                {
                    float _43 = 90f;
                    Vector3 _44 = this.pointer.transform.eulerAngles;
                    _44.x = _43;
                    this.pointer.transform.eulerAngles = _44;
                }
            }
        }
        else
        {
            if (this.pointer)
            {
                this.pointer.GetComponent<Renderer>().enabled = false;
            }
            float maxDist = 200f;
            this.lRenderer.SetPosition(1, maxDist * Vector3.forward);

            {
                float _45 = 0.1f * maxDist;
                Vector2 _46 = this.GetComponent<Renderer>().material.mainTextureScale;
                _46.x = _45;
                this.GetComponent<Renderer>().material.mainTextureScale = _46;
            }
            this.GetComponent<Renderer>().material.SetTextureScale("_NoiseTex", new Vector2((0.1f * maxDist) * this.noiseSize, this.noiseSize));
        }
    }

    public LaserScope()
    {
        this.scrollSpeed = 0.5f;
        this.pulseSpeed = 1.5f;
        this.noiseSize = 1f;
        this.maxWidth = 0.5f;
        this.minWidth = 0.2f;
        this.aniDir = 1f;
    }

}