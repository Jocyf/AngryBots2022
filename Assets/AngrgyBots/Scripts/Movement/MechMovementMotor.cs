using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Rigidbody))]
public partial class MechMovementMotor : MovementMotor
{
    public float walkingSpeed;
    public float turningSpeed;
    public float aimingSpeed;
    public Transform head;
    //private var wallNormal : Vector3 = Vector3.zero;
    private Vector3 wallHit;
    private bool facingInRightDirection;
    private Quaternion headRotation;
    public virtual void FixedUpdate()//transform.position += targetVelocity * Time.deltaTime * walkingSpeed * 3;
    {
        float rotationAngle = 0.0f;
        Vector3 targetVelocity = default(Vector3);
        Vector3 adjustedMovementDirection = this.movementDirection;
        // If the movement direction points into a wall as defined by the wall normal,
        // then change the movement direction to be perpendicular to the wall,
        // so the character "glides" along the wall.
        /*if (Vector3.Dot (movementDirection, wallNormal) < 0) {
			// Keep the vector length prior to adjustment
			var vectorLength : float = movementDirection.magnitude;
			// Project the movement vector onto the plane defined by the wall normal
			adjustedMovementDirection =
				movementDirection - Vector3.Project (movementDirection, wallNormal) * 0.9;
			// Apply the original length of the vector
			adjustedMovementDirection = adjustedMovementDirection.normalized * vectorLength;
		}*/
        /*Debug.DrawRay(transform.position, adjustedMovementDirection, Color.yellow);
		Debug.DrawRay(transform.position, movementDirection, Color.green);
		Debug.DrawRay(transform.position, wallNormal, Color.red);*/
        // Make the character rotate towards the target rotation
        if (adjustedMovementDirection != Vector3.zero)
        {
            rotationAngle = MechMovementMotor.AngleAroundAxis(this.transform.forward, adjustedMovementDirection, Vector3.up) * 0.3f;
        }
        else
        {
            rotationAngle = 0;
        }
        Vector3 targetAngularVelocity = Vector3.up * Mathf.Clamp(rotationAngle, -this.turningSpeed * Mathf.Deg2Rad, this.turningSpeed * Mathf.Deg2Rad);
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.MoveTowards(this.GetComponent<Rigidbody>().angularVelocity, targetAngularVelocity, ((Time.deltaTime * this.turningSpeed) * Mathf.Deg2Rad) * 3);
        /*
		if ((transform.position - wallHit).magnitude > 2) {
			wallNormal = Vector3.zero;
		}*/
        float angle = Vector3.Angle(this.transform.forward, adjustedMovementDirection);
        if (this.facingInRightDirection && (angle > 25))
        {
            this.facingInRightDirection = false;
        }
        if (!this.facingInRightDirection && (angle < 5))
        {
            this.facingInRightDirection = true;
        }
        // Handle the movement of the character
        if (this.facingInRightDirection)
        {
            targetVelocity = (this.transform.forward * this.walkingSpeed) + (this.GetComponent<Rigidbody>().velocity.y * Vector3.up);
        }
        else
        {
            targetVelocity = this.GetComponent<Rigidbody>().velocity.y * Vector3.up;
        }
        this.GetComponent<Rigidbody>().velocity = Vector3.MoveTowards(this.GetComponent<Rigidbody>().velocity, targetVelocity, (Time.deltaTime * this.walkingSpeed) * 3);
    }

    public virtual void LateUpdate()
    {
        // Target with head
        if (this.facingDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(this.facingDirection);
            this.headRotation = Quaternion.RotateTowards(this.headRotation, targetRotation, this.aimingSpeed * Time.deltaTime);
            this.head.rotation = (this.headRotation * Quaternion.Inverse(this.transform.rotation)) * this.head.rotation;
        }
    }

    /*
	function OnCollisionStay (collisionInfo : Collision) {
		if (collisionInfo.gameObject.tag == "Player")
			return;
		
		// Record the first wall normal
		for (var contact : ContactPoint in collisionInfo.contacts) {
			// Discard normals that are not mostly horizontal
			if (Mathf.Abs(contact.normal.y) < 0.7) {
				wallNormal = contact.normal;
				wallNormal.y = 0;
				wallHit = transform.position;
				break;
			}
		}
		
		// Only keep the horizontal components
		wallNormal.y = 0;
	}
	*/
    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);
        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);
        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }

    public MechMovementMotor()
    {
        this.walkingSpeed = 3f;
        this.turningSpeed = 100f;
        this.aimingSpeed = 150f;
        this.headRotation = Quaternion.identity;
    }

}