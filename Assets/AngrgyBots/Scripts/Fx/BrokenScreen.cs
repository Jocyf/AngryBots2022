using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class BrokenScreen : MonoBehaviour
{
    public static Material brokenMaterial;
    public virtual void Start()
    {
        if (BrokenScreen.brokenMaterial == null)
        {
            BrokenScreen.brokenMaterial = new Material(this.GetComponent<Renderer>().sharedMaterial);
        }
        this.GetComponent<Renderer>().material = BrokenScreen.brokenMaterial;
    }

    public virtual void OnWillRenderObject()
    {

        {
            float _33 = BrokenScreen.brokenMaterial.mainTextureOffset.x + Random.Range(1f, 2f);
            Vector2 _34 = BrokenScreen.brokenMaterial.mainTextureOffset;
            _34.x = _33;
            BrokenScreen.brokenMaterial.mainTextureOffset = _34;
        }
    }

}