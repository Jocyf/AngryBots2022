using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Rigidbody))]
public partial class HoverMovementMotor : MovementMotor
{
    //public var movement : MoveController;
    public float flyingSpeed;
    public float flyingSnappyness;
    public float turningSpeed;
    public float turningSnappyness;
    public float bankingAmount;
    public virtual void FixedUpdate()
    {
        Vector3 axis = default(Vector3);
        float angle = 0.0f;
        // Handle the movement of the character
        Vector3 targetVelocity = this.movementDirection * this.flyingSpeed;
        Vector3 deltaVelocity = targetVelocity - this.GetComponent<Rigidbody>().velocity;
        this.GetComponent<Rigidbody>().AddForce(deltaVelocity * this.flyingSnappyness, ForceMode.Acceleration);
        // Make the character rotate towards the target rotation
        Vector3 facingDir = this.facingDirection != Vector3.zero ? this.facingDirection : this.movementDirection;
        if (facingDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(facingDir, Vector3.up);
            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(this.transform.rotation);
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 deltaAngularVelocity = (axis * Mathf.Clamp(angle, -this.turningSpeed, this.turningSpeed)) - this.GetComponent<Rigidbody>().angularVelocity;
            float banking = Vector3.Dot(this.movementDirection, -this.transform.right);
            this.GetComponent<Rigidbody>().AddTorque((deltaAngularVelocity * this.turningSnappyness) + ((this.transform.forward * banking) * this.bankingAmount));
        }
    }

    public virtual void OnCollisionStay(Collision collisionInfo)
    {
        // Move up if colliding with static geometry
        if (collisionInfo.rigidbody == null)
        {
            this.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + ((Vector3.up * Time.deltaTime) * 50);
        }
    }

    public HoverMovementMotor()
    {
        this.flyingSpeed = 5f;
        this.flyingSnappyness = 2f;
        this.turningSpeed = 3f;
        this.turningSnappyness = 3f;
        this.bankingAmount = 1f;
    }

}