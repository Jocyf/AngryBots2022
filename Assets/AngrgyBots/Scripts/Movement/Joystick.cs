using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary : object
{
    public Vector2 min;
    public Vector2 max;
    public Boundary()
    {
        this.min = Vector2.zero;
        this.max = Vector2.zero;
    }

}
[System.Serializable]
//[UnityEngine.RequireComponent(typeof(GUITexture))]    /**/
public partial class Joystick : MonoBehaviour
{
    private static Joystick[] joysticks; // A static collection of all joysticks
    private static bool enumeratedJoysticks;
    private static float tapTimeDelta; // Time allowed between taps
    public bool touchPad; // Is this a TouchPad?
    public Rect touchZone;
    public float deadZone; // Control when position is output
    public bool normalize; // Normalize output after the dead-zone?
    public Vector2 position; // [-1, 1] in x,y
    public int tapCount; // Current tap count

    private int lastFingerId; // Finger last used for this joystick
    private float tapTimeWindow; // How much time there is left for a tap to occur
    private Vector2 fingerDownPos;
    private float fingerDownTime;
    private float firstDeltaTime;
    //private GUITexture gui; // Joystick graphic   /**/
    private Rect defaultRect; // Default position / extents of the joystick graphic
    private Boundary guiBoundary; // Boundary for joystick graphic
    private Vector2 guiTouchOffset; // Offset to apply to touch input
    private Vector2 guiCenter; // Center of joystick

    public virtual void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public Joystick()
    {
        this.lastFingerId = -1;
        this.firstDeltaTime = 0.5f;
        this.guiBoundary = new Boundary();
    }

    static Joystick()
    {
        Joystick.tapTimeDelta = 0.3f;
    }

}
