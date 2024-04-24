using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public partial class SampleAnimationOnSelected : MonoBehaviour
{
    //import UnityEditor;
    // MenuItem adds a menu item in the GameObject menu
    // and executes the following function when clicked
    [UnityEditor.MenuItem("Tools/Sample Animation On Selected")]
    public static void SampleAnimation()
    {
        Animation anim = Selection.activeGameObject.GetComponent<Animation>();
        if (anim != null)
        {
            anim.clip.SampleAnimation(Selection.activeGameObject, 0);
        }
    }

}