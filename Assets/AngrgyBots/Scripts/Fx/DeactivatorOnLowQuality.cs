using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DeactivatorOnLowQuality : MonoBehaviour
{
    public Quality qualityThreshhold;
    public virtual void Start()
    {
        if (QualityManager.quality < this.qualityThreshhold)
        {
            this.gameObject.SetActive(false);
        }
        this.enabled = false;
    }

    public DeactivatorOnLowQuality()
    {
        this.qualityThreshhold = Quality.High;
    }

}