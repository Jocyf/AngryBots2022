// Effect Sequence of any enemy destruction, missile impact or regular hit FX
// There could be several partcile system involved (explosion, showwave, explosion sound, etc)
// This scripot will instantiate all the effects

using UnityEngine;
using System.Collections;

[System.Serializable]
public class ExplosionPart : object
{
    public GameObject gameObject;
    public float delay;
    public bool hqOnly;
    public float yOffset;
}

[System.Serializable]
public partial class EffectSequencer : MonoBehaviour
{
    public ExplosionPart[] ambientEmitters;
    public ExplosionPart[] explosionEmitters;
    public ExplosionPart[] smokeEmitters;
    public ExplosionPart[] miscSpecialEffects;

    private GameObject instantiatedObject = null;

    public virtual IEnumerator Start()
    {
        //ExplosionPart go = null;
        float maxTime = 0;
        foreach (ExplosionPart go in ambientEmitters)
        {
            yield return StartCoroutine(InstantiateDelayed(go));
            if (instantiatedObject.GetComponent<ParticleSystem>())
            {
                ParticleSystem particleSystem = instantiatedObject.GetComponent<ParticleSystem>();
                ParticleSystem.MinMaxCurve lifeCurve = particleSystem.main.startLifetime;

                maxTime = Mathf.Max(maxTime, go.delay + lifeCurve.constantMax);
            }
        }

        foreach (ExplosionPart go in this.explosionEmitters)
        {
            yield return StartCoroutine(InstantiateDelayed(go));
            if (instantiatedObject.GetComponent<ParticleSystem>())
            {
                ParticleSystem particleSystem = instantiatedObject.GetComponent<ParticleSystem>();
                ParticleSystem.MinMaxCurve lifeCurve = particleSystem.main.startLifetime;

                maxTime = Mathf.Max(maxTime, go.delay + lifeCurve.constantMax);
            }
        }

        foreach (ExplosionPart go in smokeEmitters)
        {
            yield return StartCoroutine(InstantiateDelayed(go));
            if (instantiatedObject.GetComponent<ParticleSystem>())
            {
                ParticleSystem particleSystem = instantiatedObject.GetComponent<ParticleSystem>();
                ParticleSystem.MinMaxCurve lifeCurve = particleSystem.main.startLifetime;

                maxTime = Mathf.Max(maxTime, go.delay + lifeCurve.constantMax);
            }
        }

        //Get the explosion audioClip if exist
        AudioSource _audio = GetComponent<AudioSource>();
        if (_audio != null && _audio.clip != null)
        {
            maxTime = Mathf.Max(maxTime, _audio.clip.length);
        }

        yield return null;

        foreach (ExplosionPart go in miscSpecialEffects)
        {
            yield return StartCoroutine(InstantiateDelayed(go));
            if (instantiatedObject.GetComponent<ParticleSystem>())
            {
                ParticleSystem particleSystem = instantiatedObject.GetComponent<ParticleSystem>();
                ParticleSystem.MinMaxCurve lifeCurve = particleSystem.main.startLifetime;

                maxTime = Mathf.Max(maxTime, go.delay + lifeCurve.constantMax);
            }
        }
        
        Destroy(gameObject, maxTime + 0.5f);
    }

    public virtual IEnumerator InstantiateDelayed (ExplosionPart go)
    {
        if (go.hqOnly && QualityManager.quality < Quality.High) { yield break; }
        yield return new WaitForSeconds(go.delay);
        instantiatedObject = Instantiate(go.gameObject, transform.position + (Vector3.up * go.yOffset), transform.rotation);
        //Debug.Log("EffectSequencer:: Created FX: " + instantiatedObject.name);
    }

}