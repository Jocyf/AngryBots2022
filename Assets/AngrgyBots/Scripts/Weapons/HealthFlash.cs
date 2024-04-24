using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class HealthFlash : MonoBehaviour
{
    public Health playerHealth;
    public Material healthMaterial;
    private float healthBlink;
    private float oneOverMaxHealth;
    public virtual void Start()
    {
        this.oneOverMaxHealth = 1f / this.playerHealth.maxHealth;
    }

    public virtual void Update()
    {
        float relativeHealth = this.playerHealth.health * this.oneOverMaxHealth;
        this.healthMaterial.SetFloat("_SelfIllumination", (relativeHealth * 2f) * this.healthBlink);
        if (relativeHealth < 0.45f)
        {
            this.healthBlink = Mathf.PingPong(Time.time * 6f, 2f);
        }
        else
        {
            this.healthBlink = 1f;
        }
    }

    public HealthFlash()
    {
        this.healthBlink = 1f;
        this.oneOverMaxHealth = 0.5f;
    }

}