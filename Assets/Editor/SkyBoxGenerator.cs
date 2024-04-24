using UnityEngine;
using UnityEditor;
using System.Collections;

//When selected a dialog will pop up with a selection for the Transform you want to use as the origin point for the Skybox. 
//Once the Transform is set hit "Render" and wait a few seconds, then Refresh the Project Pane and the 6 generated images will import into Unity in a folder named "Skyboxes". 
//Be warned that each time you run this script it will overwrite previously generated images.
[System.Serializable]
public class SkyBoxGenerator : ScriptableWizard
{
    public Transform renderFromPosition;
    public object[] skyBoxImage;
    public object[] skyDirection;
    public virtual void OnWizardUpdate()
    {
        this.helpString = "Select transform to render from";
        this.isValid = this.renderFromPosition != null;
    }

    public virtual void OnWizardCreate()
    {
        GameObject go = new GameObject("SkyboxCamera", new System.Type[] {typeof(Camera)});
        go.GetComponent<Camera>().backgroundColor = Camera.main.backgroundColor;
        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        go.GetComponent<Camera>().fieldOfView = 90;
        go.GetComponent<Camera>().aspect = 1f;
        go.transform.position = this.renderFromPosition.position;
        if (this.renderFromPosition.GetComponent<Renderer>())
        {
            go.transform.position = this.renderFromPosition.GetComponent<Renderer>().bounds.center;
        }
        go.transform.rotation = Quaternion.identity;
        int orientation = 0;
        while (orientation < this.skyDirection.Length)
        {
            this.renderSkyImage(orientation, go);
            orientation++;
        }
        UnityEngine.Object.DestroyImmediate(go);
    }

    [UnityEditor.MenuItem("Tools/Standard Editor Tools/Render/Render Into Skybox (Unity Pro Only)", false, 4)]
    public static void RenderSkyBox()
    {
        ScriptableWizard.DisplayWizard("Render SkyBox", typeof(SkyBoxGenerator), "Render!");
    }

    public virtual void renderSkyImage(int orientation, GameObject go)
    {
        go.transform.eulerAngles = (Vector3) this.skyDirection[orientation];
        int screenSize = 512;
        RenderTexture rt = new RenderTexture(screenSize, screenSize, 24);
        go.GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(screenSize, screenSize, TextureFormat.RGB24, false);
        go.GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, screenSize, screenSize), 0, 0);
        RenderTexture.active = null;
        UnityEngine.Object.DestroyImmediate(rt);
        System.Byte[] bytes = ImageConversion.EncodeToPNG(screenShot);
        string directory = "Assets/Skyboxes";
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(directory, this.skyBoxImage[orientation] + ".png"), bytes);
    }

    public SkyBoxGenerator()
    {
        this.skyBoxImage = new object[] {"frontImage", "rightImage", "backImage", "leftImage", "upImage", "downImage"};
        this.skyDirection = new object[] {new Vector3(0, 0, 0), new Vector3(0, -90, 0), new Vector3(0, 180, 0), new Vector3(0, 90, 0), new Vector3(-90, 0, 0), new Vector3(90, 0, 0)};
    }

}