using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class AI : MonoBehaviour
{
    // Public member data
    public MonoBehaviour behaviourOnSpotted;
    public AudioClip soundOnSpotted;
    public MonoBehaviour behaviourOnLostTrack;
    [Space(10)]
    public bool showDebug = false;

    [Space(10)]
    // Private memeber data
    [SerializeField]
    private Transform character;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private bool insideInterestArea = true;

    [Space(10)]
    [SerializeField]
    private bool canSeePlayer = false;
    [SerializeField]
    private GameObject objectDetected = null;

    public virtual void Awake()
    {
        character = this.transform;
        player = GameObject.FindWithTag("Player").transform;
    }

    public virtual void OnEnable()
    {
        behaviourOnLostTrack.enabled = true;
        behaviourOnSpotted.enabled = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if ((other.transform == player) && CanSeePlayer())
        {
            OnSpotted();
        }
    }

    public virtual void OnEnterInterestArea()
    {
        insideInterestArea = true;
    }

    public virtual void OnExitInterestArea()
    {
        insideInterestArea = false;
        OnLostTrack();
    }

    public virtual void OnSpotted()
    {
        if (!insideInterestArea)
        {
            return;
        }
        if (!behaviourOnSpotted.enabled)
        {
            behaviourOnSpotted.enabled = true;
            behaviourOnLostTrack.enabled = false;
            if (GetComponent<AudioSource>() && soundOnSpotted)
            {
                GetComponent<AudioSource>().clip = soundOnSpotted;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    public virtual void OnLostTrack()
    {
        if (!behaviourOnLostTrack.enabled)
        {
            behaviourOnLostTrack.enabled = true;
            behaviourOnSpotted.enabled = false;
        }
    }

    public virtual bool CanSeePlayer()
    {
        //Debug.Log("CanSeePlayer - Start");
        RaycastHit hit = default(RaycastHit);
        Vector3 playerDirection = player.position - character.position;
        Physics.Raycast(character.position + new Vector3(0f, 0.5f, 0f), playerDirection, out hit, playerDirection.magnitude);

        // DEBUG
        if (hit.collider)
        {
            if (showDebug) { Debug.Log("CanSeePlayer -> Object Detected"); }
            objectDetected = hit.collider.transform.gameObject;
            canSeePlayer = false;
            if (!hit.collider.tag.Contains("Player")) { return false; }
        }

        if (hit.collider && hit.collider.tag.Contains("Player"))
        {
            if (showDebug) { Debug.Log("CanSeePlayer -> True"); }
            objectDetected = hit.collider.transform.gameObject;
            canSeePlayer = true;
            return true;
        }

        if (showDebug) { Debug.Log("CanSeePlayer -> False"); }
        objectDetected = null;
        canSeePlayer = false;
        return false;
    }

}