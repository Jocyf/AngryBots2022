using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpiderReturnMoveController : MonoBehaviour
{
    // Public member data
    public MovementMotor motor;
    // Private memeber data
    private AI ai;
    private Transform character;
    private Vector3 spawnPos;
    public MonoBehaviour animationBehaviour;
    public virtual void Awake()
    {
        this.character = this.motor.transform;
        this.ai = this.transform.parent.GetComponentInChildren<AI>();
        this.spawnPos = this.character.position;
    }

    public virtual void Update()
    {
        this.motor.movementDirection = this.spawnPos - this.character.position;
        this.motor.movementDirection.y = 0;
        if (this.motor.movementDirection.sqrMagnitude > 1)
        {
            this.motor.movementDirection = this.motor.movementDirection.normalized;
        }
        if (this.motor.movementDirection.sqrMagnitude < 0.01f)
        {
            this.character.position = new Vector3(this.spawnPos.x, this.character.position.y, this.spawnPos.z);
            this.motor.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.motor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            this.motor.movementDirection = Vector3.zero;
            this.enabled = false;
            this.animationBehaviour.enabled = false;
        }
    }

}