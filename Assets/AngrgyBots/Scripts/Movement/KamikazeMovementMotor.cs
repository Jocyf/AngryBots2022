using UnityEngine;
using System.Collections;

[System.Serializable]
public class KamikazeMovementMotor : MovementMotor
{
    public float flyingSpeed;
    public float zigZagness;
    public float zigZagSpeed;
    public float oriantationMultiplier;
    public float backtrackIntensity;
    private Vector3 smoothedDirection;
    public virtual void FixedUpdate()
    {
        Vector3 dir = this.movementTarget - this.transform.position;
        Vector3 zigzag = (this.transform.right * (Mathf.PingPong(Time.time * this.zigZagSpeed, 2f) - 1f)) * this.zigZagness;
        dir.Normalize();
        this.smoothedDirection = Vector3.Slerp(this.smoothedDirection, dir, Time.deltaTime * 3f);
        float orientationSpeed = 1f;
        Vector3 deltaVelocity = ((this.smoothedDirection * this.flyingSpeed) + zigzag) - this.GetComponent<Rigidbody>().velocity;
        if (Vector3.Dot(dir, this.transform.forward) > 0.8f)
        {
            this.GetComponent<Rigidbody>().AddForce(deltaVelocity, ForceMode.Force);
        }
        else
        {
            this.GetComponent<Rigidbody>().AddForce(-deltaVelocity * this.backtrackIntensity, ForceMode.Force);
            orientationSpeed = this.oriantationMultiplier;
        }
        // Make the character rotate towards the target rotation
        Vector3 faceDir = this.smoothedDirection;
        if (faceDir == Vector3.zero)
        {
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        else
        {
            float rotationAngle = KamikazeMovementMotor.AngleAroundAxis(this.transform.forward, faceDir, Vector3.up);
            this.GetComponent<Rigidbody>().angularVelocity = ((Vector3.up * rotationAngle) * 0.2f) * orientationSpeed;
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

    public virtual void OnCollisionEnter(Collision collisionInfo)
    {
    }

    public KamikazeMovementMotor()
    {
        this.flyingSpeed = 5f;
        this.zigZagness = 3f;
        this.zigZagSpeed = 2.5f;
        this.oriantationMultiplier = 2.5f;
        this.backtrackIntensity = 0.5f;
        this.smoothedDirection = Vector3.zero;
    }

}