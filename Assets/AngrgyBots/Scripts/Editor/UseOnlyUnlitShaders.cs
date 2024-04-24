using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public partial class UseOnlyUnlitShaders : MonoBehaviour
{
    //import UnityEditor;
    // MenuItem adds a menu item in the GameObject menu
    // and executes the following function when clicked
    [UnityEditor.MenuItem("Tools/Use Only Unlit Shaders")]
    public static void SampleAnimation()
    {
        Renderer[] renderers = (Renderer[]) UnityEngine.Object.FindObjectsOfType(typeof(Renderer));
        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial.shader = Shader.Find("Unlit/Texture");
        }
    }

}