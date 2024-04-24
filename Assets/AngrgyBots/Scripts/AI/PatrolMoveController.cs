using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PatrolMoveController : MonoBehaviour
{
    // Public member data
    public MovementMotor motor;
    public PatrolRoute patrolRoute;
    public float patrolPointRadius;
    // Private memeber data
    private Transform character;
    private int nextPatrolPoint;
    private int patrolDirection;
    public virtual void Start()
    {
        this.character = this.motor.transform;
        this.patrolRoute.Register(this.transform.parent.gameObject);
    }

    public virtual void OnEnable()
    {
        this.nextPatrolPoint = this.patrolRoute.GetClosestPatrolPoint(this.transform.position);
    }

    public virtual void OnDestroy()
    {
        this.patrolRoute.UnRegister(this.transform.parent.gameObject);
    }

    public virtual void Update()
    {
        // Early out if there are no patrol points
        if ((this.patrolRoute == null) || (this.patrolRoute.patrolPoints.Count == 0))
        {
            return;
        }
        // Find the vector towards the next patrol point.
        Vector3 targetVector = this.patrolRoute.patrolPoints[this.nextPatrolPoint].position - this.character.position;
        targetVector.y = 0;
        // If the patrol point has been reached, select the next one.
        if (targetVector.sqrMagnitude < (this.patrolPointRadius * this.patrolPointRadius))
        {
            this.nextPatrolPoint = this.nextPatrolPoint + this.patrolDirection;
            if (this.nextPatrolPoint < 0)
            {
                this.nextPatrolPoint = 1;
                this.patrolDirection = 1;
            }
            if (this.nextPatrolPoint >= this.patrolRoute.patrolPoints.Count)
            {
                if (this.patrolRoute.pingPong)
                {
                    this.patrolDirection = -1;
                    this.nextPatrolPoint = this.patrolRoute.patrolPoints.Count - 2;
                }
                else
                {
                    this.nextPatrolPoint = 0;
                }
            }
        }
        // Make sure the target vector doesn't exceed a length if one
        if (targetVector.sqrMagnitude > 1)
        {
            targetVector.Normalize();
        }
        // Set the movement direction.
        this.motor.movementDirection = targetVector;
        // Set the facing direction.
        this.motor.facingDirection = targetVector;
    }

    public PatrolMoveController()
    {
        this.patrolPointRadius = 0.5f;
        this.patrolDirection = 1;
    }

}