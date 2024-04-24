using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CameraFollowController : MonoBehaviour
{

    // Objects to drag in
    public Transform character;
    public GameObject cursorPrefab;
    //public GameObject joystickPrefab;

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
    //private Joystick joystickLeft;
    //private Joystick joystickRight;
    private Transform mainCameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 cameraOffset = Vector3.zero;
    private Vector3 initOffsetToPlayer;

    // Prepare a cursor point varibale. This is the mouse position on PC and controlled by the thumbstick on mobiles.
    private Vector3 cursorScreenPosition;
    private Plane playerMovementPlane;
    //private GameObject joystickRightGO;
    private Quaternion screenMovementSpace;
    private Vector3 screenMovementForward;
    private Vector3 screenMovementRight;

    public virtual void Awake()
    {
        // Set main camera
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;

        // Ensure we have character set
        // Default to using the transform this component is on
        if (!character) { character = transform; }
        initOffsetToPlayer = mainCameraTransform.position - this.character.position;
        if (cursorPrefab) { cursorObject = Instantiate(cursorPrefab).transform; }

        // Save camera offset so we can use it in the first frame
        cameraOffset = mainCameraTransform.position - character.position;
        // Set the initial cursor position to the center of the screen
        cursorScreenPosition = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0);
        // caching movement plane
        playerMovementPlane = new Plane(character.up, character.position + (character.up * cursorPlaneHeight));
    }

    public virtual void Start()
    {
        // it's fine to calculate this on Start () as the camera is static in rotation
        screenMovementSpace = Quaternion.Euler(0, mainCameraTransform.eulerAngles.y, 0);
        screenMovementForward = screenMovementSpace * Vector3.forward;
        screenMovementRight = screenMovementSpace * Vector3.right;
    }


    public virtual void Update()
    {

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
        // Vector3 cameraAdjustmentVector = Vector3.zero; /**/

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
        //this.motor.facingDirection = cursorWorldPosition - this.character.position;
        //this.motor.facingDirection.y = 0;

        // Draw the cursor nicely
        HandleCursorAlignment(cursorWorldPosition);
        // HANDLE CAMERA POSITION
        // Set the target position of the camera to point at the focus point
        //cameraTargetPosition = (character.position + initOffsetToPlayer) + (cameraAdjustmentVector * cameraPreview); /**/
        // Apply some smoothing to the camera movement
        // mainCameraTransform.position = Vector3.SmoothDamp(mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing); /**/
        // Save camera offset so we can use it in the next frame
        cameraOffset = mainCameraTransform.position - character.position;
    }

    private Vector3 cameraAdjustmentVector;
    private Vector3 cameraTargetPosition;
    public virtual void FixedUpdate()
    {
        cameraTargetPosition = (character.position + initOffsetToPlayer) + (cameraAdjustmentVector * cameraPreview);
        mainCameraTransform.position = Vector3.SmoothDamp(mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing);
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
        if (!cursorObject) { return; }


        // HANDLE CURSOR POSITION
        // Set the position of the cursor object
        cursorObject.position = cursorWorldPosition;
        // Hide mouse cursor when within screen area, since we're showing game cursor instead

        /**/
        #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL)
        if (DemoControlv2.Instance.isPaused)
        {
            if (cursorObject.gameObject.activeSelf) { cursorObject.gameObject.SetActive(false); }
            if (!Cursor.visible) { Cursor.visible = true; }
        }
        else
        {
            if (!cursorObject.gameObject.activeSelf) { cursorObject.gameObject.SetActive(true); }
            if (Cursor.visible) { Cursor.visible = false; }
            //Cursor.visible = (((Input.mousePosition.x < 0) || (Input.mousePosition.x > Screen.width)) || (Input.mousePosition.y < 0)) || (Input.mousePosition.y > Screen.height);
        }
        #elif UNITY_ANDROID || UNITY_IOS
        cursorObject.gameObject.SetActive(true);
        if (Cursor.visible) { Cursor.visible = false; }
        //Cursor.visible = (((Input.mousePosition.x < 0) || (Input.mousePosition.x > Screen.width)) || (Input.mousePosition.y < 0)) || (Input.mousePosition.y > Screen.height);
        #endif




        // HANDLE CURSOR ROTATION
        Quaternion cursorWorldRotation = this.cursorObject.rotation;
        /*if (this.motor.facingDirection != Vector3.zero)
        {
            cursorWorldRotation = Quaternion.LookRotation(this.motor.facingDirection);
        }*/
        cursorWorldRotation = Quaternion.LookRotation(this.character.forward);

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
        float cursorScaleMultiplier = Mathf.Lerp(0.7f, 1f, Mathf.InverseLerp(0.5f, 4f, this.character.forward.magnitude) ); //this.motor.facingDirection.magnitude) );
        // Set the scale of the cursor
        this.cursorObject.localScale = (Vector3.one * Mathf.Lerp(compensatedScale, 1, this.cursorSmallerWithDistance)) * cursorScaleMultiplier;
        
        // DEBUG - REMOVE LATER
        /*if (Input.GetKey(KeyCode.O))
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
        this.cursorSmallerWithDistance = Mathf.Clamp01(this.cursorSmallerWithDistance);*/
    }

}