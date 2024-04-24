using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Rigidbody))]
public partial class FreeMovementMotor : MovementMotor
{
    //public var movement : MoveController;
    public float walkingSpeed;
    public float walkingSnappyness;
    public float turningSmoothing;
    public virtual void FixedUpdate()
    {
        // Handle the movement of the character
        Vector3 targetVelocity = this.movementDirection * this.walkingSpeed;
        Vector3 deltaVelocity = targetVelocity - this.GetComponent<Rigidbody>().velocity;
        if (this.GetComponent<Rigidbody>().useGravity)
        {
            deltaVelocity.y = 0;
        }
        this.GetComponent<Rigidbody>().AddForce(deltaVelocity * this.walkingSnappyness, ForceMode.Acceleration);
        // Setup player to face facingDirection, or if that is zero, then the movementDirection
        Vector3 faceDir = this.facingDirection;
        if (faceDir == Vector3.zero)
        {
            faceDir = this.movementDirection;
        }
        // Make the character rotate towards the target rotation
        if (faceDir == Vector3.zero)
        {
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        else
        {
            float rotationAngle = FreeMovementMotor.AngleAroundAxis(this.transform.forward, faceDir, Vector3.up);
            this.GetComponent<Rigidbody>().angularVelocity = (Vector3.up * rotationAngle) * this.turningSmoothing;
        }
    }

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

    public FreeMovementMotor()
    {
        this.walkingSpeed = 5f;
        this.walkingSnappyness = 50;
        this.turningSmoothing = 0.3f;
    }

}