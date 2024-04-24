using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class RenderCubemapWizard : ScriptableWizard
{
    public Transform renderFromPosition;
    public Cubemap cubemap;
    public Material mySkyBoxMat;
    public virtual void OnWizardUpdate()
    {
        this.helpString = "Select transform to render from and cubemap to render into";
        this.isValid = (this.renderFromPosition != null) && (this.cubemap != null);
    }

    public virtual void OnWizardCreate()
    {
         // create temporary camera for rendering
        GameObject go = new GameObject("CubemapCamera", new System.Type[] {typeof(Camera)});
        //        go.camera.backgroundColor = Color (0.1, 0.1, 0.1, 1.0);
        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        if (!((Skybox) go.GetComponent(typeof(Skybox))))
        {
            go.AddComponent(typeof(Skybox));
        }
        (((Skybox) go.GetComponent(typeof(Skybox))) as Skybox).material = this.mySkyBoxMat;
        // place it on the object
        go.transform.position = this.renderFromPosition.position;
        if (this.renderFromPosition.GetComponent<Renderer>())
        {
            go.transform.position = this.renderFromPosition.GetComponent<Renderer>().bounds.center;
        }
        go.transform.rotation = Quaternion.identity;
        go.GetComponent<Camera>().fieldOfView = 90f;
        go.GetComponent<Camera>().aspect = 1f;
        // render into cubemap        
        go.GetComponent<Camera>().RenderToCubemap(this.cubemap);
        // destroy temporary camera
        UnityEngine.Object.DestroyImmediate(go);
    }

    [UnityEditor.MenuItem("Tools/Standard Editor Tools/Render/Render Into Cubemap", false, 4)]
    public static void RenderCubemap()
    {
        ScriptableWizard.DisplayWizard("Render cubemap", typeof(RenderCubemapWizard), "Render!");
    }

}