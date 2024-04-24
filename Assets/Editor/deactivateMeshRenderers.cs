using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public partial class deactivateMeshRenderers : MonoBehaviour
{
    // MenuItem adds a menu item in the GameObject menu
    // and executes the following function when clicked
    [UnityEditor.MenuItem("GameObject/Deactivate Renderers")]
    public static void deactivateRenderers()
    {
         // Get all selected game objects that we are allowed to modify!
        GameObject[] gos = Selection.gameObjects;
        // Change the values of all
        foreach (GameObject go in gos)
        {
            go.GetComponent<Renderer>().enabled = false;
        }
    }

    [UnityEditor.MenuItem("GameObject/Activate Renderers")]
    public static void activateRenderers()
    {
         // Get all selected game objects that we are allowed to modify!
        GameObject[] gos = Selection.gameObjects;
        // Change the values of all
        foreach (GameObject go in gos)
        {
            go.GetComponent<Renderer>().enabled = true;
        }
    }

}