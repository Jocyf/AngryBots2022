using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class EmitParticles : MonoBehaviour
{
    public virtual void OnSignal()
    {
        //this.GetComponent<ParticleEmitter>().emit = true;
        GetComponent<ParticleSystem>().Play();
    }

}