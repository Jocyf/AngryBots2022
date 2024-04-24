using UnityEngine;
using System.Collections;

public enum FootType
{
    Player = 0,
    Mech = 1,
    Spider = 2
}

[System.Serializable]
public partial class FootstepHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public FootType footType;

    private PhysicMaterial physicMaterial;
    private AudioClip sound = null;

    public virtual void OnCollisionEnter(Collision collisionInfo)
    {
        physicMaterial = collisionInfo.collider.sharedMaterial;
    }

    
    public virtual void OnFootstep()
    {
        if (!audioSource.enabled) { return; }

        switch (footType)
        {
            case FootType.Player:
                sound = MaterialImpactManager.GetPlayerFootstepSound(physicMaterial);
                break;
            case FootType.Mech:
                sound = MaterialImpactManager.GetMechFootstepSound(physicMaterial);
                break;
            case FootType.Spider:
                sound = MaterialImpactManager.GetSpiderFootstepSound(physicMaterial);
                break;
                default:
                sound = null;
                return;
        }
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.PlayOneShot(sound, Random.Range(0.8f, 1.2f));
    }

}