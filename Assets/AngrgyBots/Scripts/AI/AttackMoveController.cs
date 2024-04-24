using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class AttackMoveController : MonoBehaviour
{
    // Public member data
    public MovementMotor motor;
    public float targetDistanceFront;
    public float targetDistanceBack;
    public MonoBehaviour weaponBehaviour;
    // Private memeber data
    private Transform character;
    private int nextPatrolPoint;
    private Transform player;
    private float goingRight;
    private float directionChangeTime;
    public virtual void Awake()
    {
        this.character = this.motor.transform;
        this.player = GameObject.FindWithTag("Player").transform;
    }

    public virtual void OnEnable()
    {
        if (this.weaponBehaviour)
        {
            this.weaponBehaviour.enabled = true;
        }
    }

    public virtual void OnDisable()
    {
        if (this.weaponBehaviour)
        {
            this.weaponBehaviour.enabled = false;
        }
    }

    public virtual void Update()
    {
        // Calculate the direction from the player to this character
        Vector3 playerToCharacterDirection = this.character.position - this.player.position;
        //playerToCharacterDirection.y = 0;
        playerToCharacterDirection.Normalize();
        // Set this character to face the player,
        // that is, to face the direction from this character to the player
        this.motor.facingDirection = -playerToCharacterDirection;
        // Calculate the angle in degrees this character is away from the front of the character.
        // If this character is in front of the player, the angle is 0,
        // to the side is 90 and behind is 180.
        float degreesFromPlayerForward = Vector3.Angle(playerToCharacterDirection, this.player.forward);
        // If not almost directly in front of the player, base the direction on what
        // is the shortest route to get behind the player: Left or right way around.
        if (degreesFromPlayerForward > 30)
        {
            this.goingRight = Mathf.Sign(Vector3.Dot(-playerToCharacterDirection, this.player.right));
            this.directionChangeTime = Time.time;
        }
        else
        {
            // If this character has stayed in front of the player for more than a little while,
            // change direction to the other way around. This avoids some cases of the character
            // getting stuck while the player is aiming at him.
            if (Time.time > (this.directionChangeTime + 0.8f))
            {
                this.goingRight = -this.goingRight;
                this.directionChangeTime = Time.time;
            }
        }
        // behindFactor is how much this character is behind the player.
        // 0 degrees is a value of 0 and 90 degrees or more is a value of 1.
        float behindFactor = Mathf.InverseLerp(0, 90, degreesFromPlayerForward);
        // Lerp the target distance between the front and back target distances based on the behindFactor
        float targetDistance = Mathf.Lerp(this.targetDistanceFront, this.targetDistanceBack, behindFactor);
        // Calculate the targetPosition
        Vector3 targetPosition = this.player.position + (playerToCharacterDirection * targetDistance);
        // If this character is not already behind the player, strafe sideways to get behind
        if (degreesFromPlayerForward < 120)
        {
            targetPosition = targetPosition + (this.character.right * this.goingRight);
        }
        // Calculate the target vector that this character should move along
        // to reach the target position
        Vector3 targetVector = targetPosition - this.character.position;
        targetVector.y = 0;
        // Make sure the target vector doesn't exceed a length if one
        if (targetVector.sqrMagnitude > 1)
        {
            targetVector.Normalize();
        }
        // Smooth the movement direction a bit so this character doesn't change direction abruptly
        this.motor.movementDirection = targetVector;
    }

    public AttackMoveController()
    {
        this.targetDistanceFront = 2.5f;
        this.targetDistanceBack = 1f;
        this.goingRight = 1;
    }

}