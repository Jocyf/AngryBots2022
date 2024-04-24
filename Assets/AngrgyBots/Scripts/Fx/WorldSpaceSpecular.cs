using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
public partial class WorldSpaceSpecular : MonoBehaviour
{
    public Transform casterA;
    public Color colorA;
    public Transform casterB;
    public Color colorB;
    public Transform casterC;
    public Color colorC;
    public virtual void Update()
    {
        if (this.casterA)
        {
            Shader.SetGlobalVector("SPEC_LIGHT_DIR_0", this.casterA.forward);
        }
        if (this.casterB)
        {
            Shader.SetGlobalVector("SPEC_LIGHT_DIR_1", this.casterB.forward);
        }
        if (this.casterC)
        {
            Shader.SetGlobalVector("SPEC_LIGHT_DIR_2", this.casterC.forward);
        }
        Shader.SetGlobalVector("SPEC_LIGHT_COLOR_0", this.colorA);
        Shader.SetGlobalVector("SPEC_LIGHT_COLOR_1", this.colorB);
        Shader.SetGlobalVector("SPEC_LIGHT_COLOR_2", this.colorC);
    }

    public WorldSpaceSpecular()
    {
        this.colorA = Color.white;
        this.colorB = Color.white;
        this.colorC = Color.white;
    }

}