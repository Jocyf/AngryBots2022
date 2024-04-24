using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SpiderAnimationTest : MonoBehaviour
{
    public Rigidbody rigid;
    public AnimationClip forwardAnim;
    public AnimationClip backAnim;
    public AnimationClip leftAnim;
    public AnimationClip rightAnim;
    public float walking;
    public float angle;
    private Transform tr;
    public virtual void OnEnable()
    {
        this.tr = this.rigid.transform;
        this.GetComponent<Animation>()[this.forwardAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.forwardAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.backAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.backAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.leftAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.leftAnim.name].enabled = true;
        this.GetComponent<Animation>()[this.rightAnim.name].layer = 1;
        this.GetComponent<Animation>()[this.rightAnim.name].enabled = true;
        this.GetComponent<Animation>().SyncLayer(1);
    }

    public virtual void Update()
    {
        float w = 0.0f;
        this.rigid.velocity = ((Quaternion.Euler(0, this.angle, 0) * this.rigid.transform.forward) * 2.4f) * this.walking;
        Vector3 velocity = this.rigid.velocity;
        velocity.y = 0;
        float walkWeight = velocity.magnitude / 2.4f;
        this.GetComponent<Animation>()[this.forwardAnim.name].speed = walkWeight;
        this.GetComponent<Animation>()[this.rightAnim.name].speed = walkWeight;
        this.GetComponent<Animation>()[this.backAnim.name].speed = walkWeight;
        this.GetComponent<Animation>()[this.leftAnim.name].speed = walkWeight;
        if (velocity == Vector3.zero)
        {
            return;
        }
        float angle = Mathf.DeltaAngle(SpiderAnimationTest.HorizontalAngle(this.tr.forward), SpiderAnimationTest.HorizontalAngle(this.rigid.velocity));
        if (angle < -90)
        {
            w = Mathf.InverseLerp(-180, -90, angle);
            this.GetComponent<Animation>()[this.forwardAnim.name].weight = 0;
            this.GetComponent<Animation>()[this.rightAnim.name].weight = 0;
            this.GetComponent<Animation>()[this.backAnim.name].weight = 1 - w;
            this.GetComponent<Animation>()[this.leftAnim.name].weight = 1;
        }
        else
        {
            if (angle < 0)
            {
                w = Mathf.InverseLerp(-90, 0, angle);
                this.GetComponent<Animation>()[this.forwardAnim.name].weight = w;
                this.GetComponent<Animation>()[this.rightAnim.name].weight = 0;
                this.GetComponent<Animation>()[this.backAnim.name].weight = 0;
                this.GetComponent<Animation>()[this.leftAnim.name].weight = 1 - w;
            }
            else
            {
                if (angle < 90)
                {
                    w = Mathf.InverseLerp(0, 90, angle);
                    this.GetComponent<Animation>()[this.forwardAnim.name].weight = 1 - w;
                    this.GetComponent<Animation>()[this.rightAnim.name].weight = w;
                    this.GetComponent<Animation>()[this.backAnim.name].weight = 0;
                    this.GetComponent<Animation>()[this.leftAnim.name].weight = 0;
                }
                else
                {
                    w = Mathf.InverseLerp(90, 180, angle);
                    this.GetComponent<Animation>()[this.forwardAnim.name].weight = 0;
                    this.GetComponent<Animation>()[this.rightAnim.name].weight = 1 - w;
                    this.GetComponent<Animation>()[this.backAnim.name].weight = w;
                    this.GetComponent<Animation>()[this.leftAnim.name].weight = 0;
                }
            }
        }
    }

    public static float HorizontalAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    public virtual void OnGUI()
    {
        GUILayout.Label("Angle (0 to 360): " + this.angle.ToString("0.00"), new GUILayoutOption[] {});
        this.angle = GUILayout.HorizontalSlider(this.angle, 0, 360, new GUILayoutOption[] {GUILayout.Width(200)});
        int i = 0;
        while (i <= 360)
        {
            if (Mathf.Abs(this.angle - i) < 10)
            {
                this.angle = i;
            }
            i = i + 45;
        }
        GUILayout.Label("Walking (0 to 1): " + this.walking.ToString("0.00"), new GUILayoutOption[] {});
        this.walking = GUILayout.HorizontalSlider(this.walking, 0, 1, new GUILayoutOption[] {GUILayout.Width(100)});
    }

}