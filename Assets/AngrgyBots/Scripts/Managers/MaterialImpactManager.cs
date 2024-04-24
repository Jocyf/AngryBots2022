using UnityEngine;
using System.Collections;

[System.Serializable]
public class MaterialImpact : object
{
    public PhysicMaterial physicMaterial;
    public AudioClip[] playerFootstepSounds;
    public AudioClip[] mechFootstepSounds;
    public AudioClip[] spiderFootstepSounds;
    public AudioClip[] bulletHitSounds;
}

[System.Serializable]
public class MaterialImpactManager : MonoBehaviour
{
    public MaterialImpact[] materials;
    private static System.Collections.Generic.Dictionary<PhysicMaterial, MaterialImpact> dict;
    private static MaterialImpact defaultMat;

    public virtual void Awake()
    {
        MaterialImpactManager.defaultMat = materials[0];
        MaterialImpactManager.dict = new System.Collections.Generic.Dictionary<PhysicMaterial, MaterialImpact>();
        int i = 0;
        while (i < materials.Length)
        {
            MaterialImpactManager.dict.Add(materials[i].physicMaterial, materials[i]);
            i++;
        }
    }

    public static AudioClip GetPlayerFootstepSound(PhysicMaterial mat)
    {
        MaterialImpact imp = MaterialImpactManager.GetMaterialImpact(mat);
        return MaterialImpactManager.GetRandomSoundFromArray(imp.playerFootstepSounds);
    }

    public static AudioClip GetMechFootstepSound(PhysicMaterial mat)
    {
        MaterialImpact imp = MaterialImpactManager.GetMaterialImpact(mat);
        return MaterialImpactManager.GetRandomSoundFromArray(imp.mechFootstepSounds);
    }

    public static AudioClip GetSpiderFootstepSound(PhysicMaterial mat)
    {
        MaterialImpact imp = MaterialImpactManager.GetMaterialImpact(mat);
        return MaterialImpactManager.GetRandomSoundFromArray(imp.spiderFootstepSounds);
    }

    public static AudioClip GetBulletHitSound(PhysicMaterial mat)
    {
        MaterialImpact imp = MaterialImpactManager.GetMaterialImpact(mat);
        return MaterialImpactManager.GetRandomSoundFromArray(imp.bulletHitSounds);
    }

    public static MaterialImpact GetMaterialImpact(PhysicMaterial mat)
    {
        if (mat && MaterialImpactManager.dict.ContainsKey(mat))
        {
            return MaterialImpactManager.dict[mat];
        }
        return MaterialImpactManager.defaultMat;
    }

    public static AudioClip GetRandomSoundFromArray(AudioClip[] audioClipArray)
    {
        if (audioClipArray.Length > 0)
        {
            return audioClipArray[Random.Range(0, audioClipArray.Length)];
        }
        return null;
    }

}