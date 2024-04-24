using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PlayerMoveController : MonoBehaviour
{

    // Objects to drag in
    public MovementMotor motor;
    public Transform character;
    public GameObject cursorPrefab;
    public GameObject joystickPrefab;

    // Settings
    public float cameraSmoothing = 0.01f;
    public float cameraPreview = 2f;

    // Cursor settings
    public float cursorPlaneHeight;
    public float cursorFacingCamera;
    public float cursorSmallerWithDistance;
    public float cursorSmallerWhenClose = 1;

    // Private memeber data
    private Camera mainCamera;
    private Transform cursorObject;
    private Joystick joystickLeft;
    private Joystick joystickRight;
    private Transform mainCameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 cameraOffset = Vector3.zero;
    private Vector3 initOffsetToPlayer;

    // Prepare a cursor point varibale. This is the mouse position on PC and controlled by the thumbstick on mobiles.
    private Vector3 cursorScreenPosition;
    private Plane playerMovementPlane;
    private GameObject joystickRightGO;
    private Quaternion screenMovementSpace;
    private Vector3 screenMovementForward;
    private Vector3 screenMovementRight;

    public virtual void Awake()
    {
        this.motor.movementDirection = Vector2.zero;
        this.motor.facingDirection = Vector2.zero;
        // Set main camera
        this.mainCamera = Camera.main;
        this.mainCameraTransform = this.mainCamera.transform;
        // Ensure we have character set
        // Default to using the transform this component is on
        if (!this.character)
        {
            this.character = this.transform;
        }
        this.initOffsetToPlayer = this.mainCameraTransform.position - this.character.position;
        if (this.cursorPrefab)
        {
            this.cursorObject = (UnityEngine.Object.Instantiate(this.cursorPrefab) as GameObject).transform;
        }
        // Save camera offset so we can use it in the first frame
        this.cameraOffset = this.mainCameraTransform.position - this.character.position;
        // Set the initial cursor position to the center of the screen
        this.cursorScreenPosition = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0);
        // caching movement plane
        this.playerMovementPlane = new Plane(this.character.up, this.character.position + (this.character.up * this.cursorPlaneHeight));
    }

    public virtual void Start()
    {
        // it's fine to calculate this on Start () as the camera is static in rotation
        this.screenMovementSpace = Quaternion.Euler(0, this.mainCameraTransform.eulerAngles.y, 0);
        this.screenMovementForward = this.screenMovementSpace * Vector3.forward;
        this.screenMovementRight = this.screenMovementSpace * Vector3.right;
    }

    public virtual void OnDisable()
    {
        if (this.joystickLeft)
        {
            this.joystickLeft.enabled = false;
        }
        if (this.joystickRight)
        {
            this.joystickRight.enabled = false;
        }
    }

    public virtual void OnEnable()
    {
        if (this.joystickLeft)
        {
            this.joystickLeft.enabled = true;
        }
        if (this.joystickRight)
        {
            this.joystickRight.enabled = true;
        }
    }

    private bool button8Down;
    private bool button9Down;
    private int moveF;
    private int moveB;
    public virtual void OnGUI() //GUI.Label(new Rect(10,70,140,25),"JS Connected: "+GLOBAL.isJSConnected);
    {
    }

    public virtual void Update()
    {
        // HANDLE CHARACTER MOVEMENT DIRECTION
        if (GLOBAL.isJSConnected)
        {
            if (Input.GetButtonDown("Joystick button 8"))
            {
                this.button8Down = true;
            }
            if (Input.GetButtonUp("Joystick button 8"))
            {
                this.button8Down = false;
            }
            if (Input.GetButtonDown("Joystick button 9"))
            {
                this.button9Down = true;
            }
            if (Input.GetButtonUp("Joystick button 9"))
            {
                this.button9Down = false;
            }
            // Inverted logic might be more comfortable
            if (!this.button8Down)
            {
                this.moveF = 1;
            }
            else
            {
                this.moveF = 0;
            }
            if (this.button9Down)
            {
                this.moveB = 1;
            }
            else
            {
                this.moveB = 0;
            }
        }
        this.motor.movementDirection = (Input.GetAxis("Horizontal") * this.screenMovementRight) + (Input.GetAxis("Vertical") * this.screenMovementForward);
        //Debug.Log("H:"+Input.GetAxis ("Horizontal"));
        //Debug.Log("3rd:"+Input.GetAxis ("3rdAxis"));
        //Debug.Log("J3rd:"+Input.GetAxis ("Joystick 3rdAxis"));
        // Make sure the direction vector doesn't exceed a length of 1
        // so the character can't move faster diagonally than horizontally or vertically
        if (this.motor.movementDirection.sqrMagnitude > 1)
        {
            this.motor.movementDirection.Normalize();
        }
        // HANDLE CHARACTER FACING DIRECTION AND SCREEN FOCUS POINT
        // First update the camera position to take into account how much the character moved since last frame
        //mainCameraTransform.position = Vector3.Lerp (mainCameraTransform.position, character.position + cameraOffset, Time.deltaTime * 45.0f * deathSmoothoutMultiplier);
        // Set up the movement plane of the character, so screenpositions
        // can be converted into world positions on this plane
        //playerMovementPlane = new Plane (Vector3.up, character.position + character.up * cursorPlaneHeight);
        // optimization (instead of newing Plane):
        this.playerMovementPlane.normal = this.character.up;
        this.playerMovementPlane.distance = -this.character.position.y + this.cursorPlaneHeight;
        // used to adjust the camera based on cursor or joystick position
        Vector3 cameraAdjustmentVector = Vector3.zero;
        //#if !UNITY_EDITOR && (UNITY_XBOX360 || UNITY_PS3 || UNITY_IPHONE)
        // On PC, the cursor point is the mouse position
        Vector3 cursorScreenPosition = Input.mousePosition;
        // Find out where the mouse ray intersects with the movement plane of the player
        Vector3 cursorWorldPosition = PlayerMoveController.ScreenPointToWorldPointOnPlane(cursorScreenPosition, this.playerMovementPlane, this.mainCamera);
        float halfWidth = Screen.width / 2f;
        float halfHeight = Screen.height / 2f;
        float maxHalf = Mathf.Max(halfWidth, halfHeight);
        // Acquire the relative screen position			
        Vector3 posRel = cursorScreenPosition - new Vector3(halfWidth, halfHeight, cursorScreenPosition.z);
        posRel.x = posRel.x / maxHalf;
        posRel.y = posRel.y / maxHalf;
        cameraAdjustmentVector = (posRel.x * this.screenMovementRight) + (posRel.y * this.screenMovementForward);
        cameraAdjustmentVector.y = 0f;
        // The facing direction is the direction from the character to the cursor world position
        this.motor.facingDirection = cursorWorldPosition - this.character.position;
        this.motor.facingDirection.y = 0;
        // Draw the cursor nicely
        this.HandleCursorAlignment(cursorWorldPosition);
        // HANDLE CAMERA POSITION
        // Set the target position of the camera to point at the focus point
        Vector3 cameraTargetPosition = (this.character.position + this.initOffsetToPlayer) + (cameraAdjustmentVector * this.cameraPreview);
        // Apply some smoothing to the camera movement
        this.mainCameraTransform.position = Vector3.SmoothDamp(this.mainCameraTransform.position, cameraTargetPosition, ref this.cameraVelocity, this.cameraSmoothing);
        // Save camera offset so we can use it in the next frame
        this.cameraOffset = this.mainCameraTransform.position - this.character.position;
    }

    public static Vector3 PlaneRayIntersection(Plane plane, Ray ray)
    {
        float dist = 0.0f;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    public static Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
    {
        // Set up a ray corresponding to the screen position
        Ray ray = camera.ScreenPointToRay(screenPoint);
        // Find out where the ray intersects with the plane
        return PlayerMoveController.PlaneRayIntersection(plane, ray);
    }

    public virtual void HandleCursorAlignment(Vector3 cursorWorldPosition)
    {
        if (!this.cursorObject)
        {
            return;
        }
        // HANDLE CURSOR POSITION
        // Set the position of the cursor object
        this.cursorObject.position = cursorWorldPosition;
        // Hide mouse cursor when within screen area, since we're showing game cursor instead
        Cursor.visible = (((Input.mousePosition.x < 0) || (Input.mousePosition.x > Screen.width)) || (Input.mousePosition.y < 0)) || (Input.mousePosition.y > Screen.height);
        // HANDLE CURSOR ROTATION
        Quaternion cursorWorldRotation = this.cursorObject.rotation;
        if (this.motor.facingDirection != Vector3.zero)
        {
            cursorWorldRotation = Quaternion.LookRotation(this.motor.facingDirection);
        }
        // Calculate cursor billboard rotation
        Vector3 cursorScreenspaceDirection = Input.mousePosition - this.mainCamera.WorldToScreenPoint(this.transform.position + (this.character.up * this.cursorPlaneHeight));
        cursorScreenspaceDirection.z = 0;
        Quaternion cursorBillboardRotation = this.mainCameraTransform.rotation * Quaternion.LookRotation(cursorScreenspaceDirection, -Vector3.forward);
        // Set cursor rotation
        this.cursorObject.rotation = Quaternion.Slerp(cursorWorldRotation, cursorBillboardRotation, this.cursorFacingCamera);
        // HANDLE CURSOR SCALING
        // The cursor is placed in the world so it gets smaller with perspective.
        // Scale it by the inverse of the distance to the camera plane to compensate for that.
        float compensatedScale = 0.1f * Vector3.Dot(cursorWorldPosition - this.mainCameraTransform.position, this.mainCameraTransform.forward);
        // Make the cursor smaller when close to character
        float cursorScaleMultiplier = Mathf.Lerp(0.7f, 1f, Mathf.InverseLerp(0.5f, 4f, this.motor.facingDirection.magnitude));
        // Set the scale of the cursor
        this.cursorObject.localScale = (Vector3.one * Mathf.Lerp(compensatedScale, 1, this.cursorSmallerWithDistance)) * cursorScaleMultiplier;
        // DEBUG - REMOVE LATER
        if (Input.GetKey(KeyCode.O))
        {
            this.cursorFacingCamera = this.cursorFacingCamera + (Time.deltaTime * 0.5f);
        }
        if (Input.GetKey(KeyCode.P))
        {
            this.cursorFacingCamera = this.cursorFacingCamera - (Time.deltaTime * 0.5f);
        }
        this.cursorFacingCamera = Mathf.Clamp01(this.cursorFacingCamera);
        if (Input.GetKey(KeyCode.K))
        {
            this.cursorSmallerWithDistance = this.cursorSmallerWithDistance + (Time.deltaTime * 0.5f);
        }
        if (Input.GetKey(KeyCode.L))
        {
            this.cursorSmallerWithDistance = this.cursorSmallerWithDistance - (Time.deltaTime * 0.5f);
        }
        this.cursorSmallerWithDistance = Mathf.Clamp01(this.cursorSmallerWithDistance);
    }



}