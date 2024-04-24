using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class ReplacePrefabInstances : ScriptableWizard
{
    public GameObject originalPrefab;
    public GameObject replacementPrefab;
    [UnityEditor.MenuItem("Tools/Replace Prefab Instances")]
    public static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ReplacePrefabInstances>("Replace Prefab Instances", "Replace");
    }

    public virtual void OnWizardCreate()
    {
        if (!this.originalPrefab || !this.replacementPrefab)
        {
            return;
        }
        UnityEngine.Object[] gos = (GameObject[]) UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
        int i = 0;
        while (i < gos.Length)
        {
            if (PrefabUtility.GetPrefabParent(gos[i]) == this.originalPrefab)
            {
                GameObject oldGo = gos[i] as GameObject;
                GameObject newGo = PrefabUtility.InstantiatePrefab(this.replacementPrefab) as GameObject;
                newGo.transform.parent = oldGo.transform.parent;
                newGo.transform.localPosition = oldGo.transform.localPosition;
                newGo.transform.localRotation = oldGo.transform.localRotation;
                newGo.transform.localScale = oldGo.transform.localScale;
                newGo.isStatic = oldGo.isStatic;
                newGo.layer = oldGo.layer;
                newGo.tag = oldGo.tag;
                newGo.name = oldGo.name.Replace(this.originalPrefab.name, this.replacementPrefab.name);
                UnityEngine.Object.DestroyImmediate(oldGo);
            }
            i++;
        }
    }

}