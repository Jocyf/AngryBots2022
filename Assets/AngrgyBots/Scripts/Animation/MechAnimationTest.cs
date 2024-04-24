using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class MechAnimationTest : MonoBehaviour
{
    public float turning;
    public float walking;
    public float turnOffset;
    public Rigidbody rigid;
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip turnLeft;
    public AnimationClip turnRight;
    public SignalSender footstepSignals;
    public virtual void OnEnable()//animation.Play ();
    {
        this.GetComponent<Animation>()[this.idle.name].layer = 0;
        this.GetComponent<Animation>()[this.idle.name].weight = 1;
        this.GetComponent<Animation>()[this.idle.name].enabled = true;
        this.GetComponent<Animation>()[this.walk.name].layer = 1;
        this.GetComponent<Animation>()[this.turnLeft.name].layer = 1;
        this.GetComponent<Animation>()[this.turnRight.name].layer = 1;
        this.GetComponent<Animation>()[this.walk.name].weight = 1;
        this.GetComponent<Animation>()[this.turnLeft.name].weight = 0;
        this.GetComponent<Animation>()[this.turnRight.name].weight = 0;
        this.GetComponent<Animation>()[this.walk.name].enabled = true;
        this.GetComponent<Animation>()[this.turnLeft.name].enabled = true;
        this.GetComponent<Animation>()[this.turnRight.name].enabled = true;
    }

    public virtual void FixedUpdate()
    {
        this.GetComponent<Animation>()[this.walk.name].speed = Mathf.Lerp(1, this.GetComponent<Animation>()[this.walk.name].length / this.GetComponent<Animation>()[this.turnLeft.name].length, Mathf.Abs(this.turning));
        this.GetComponent<Animation>()[this.turnLeft.name].time = this.GetComponent<Animation>()[this.walk.name].time + this.turnOffset;
        this.GetComponent<Animation>()[this.turnRight.name].time = this.GetComponent<Animation>()[this.walk.name].time + this.turnOffset;
        this.rigid.velocity = (this.rigid.transform.forward * 2.5f) * this.walking;
        this.rigid.angularVelocity = ((Vector3.up * this.turning) * 100) * Mathf.Deg2Rad;
        float turningWeight = (this.rigid.angularVelocity.y * Mathf.Rad2Deg) / 100f;
        float forwardWeight = this.rigid.velocity.magnitude / 2.5f;
        this.GetComponent<Animation>()[this.turnLeft.name].weight = Mathf.Clamp01(-turningWeight);
        this.GetComponent<Animation>()[this.turnRight.name].weight = Mathf.Clamp01(turningWeight);
        this.GetComponent<Animation>()[this.walk.name].weight = Mathf.Clamp01(forwardWeight);
    }

    public virtual void OnGUI()
    {
        GUILayout.Label("Walking (0 to 1): " + this.walking.ToString("0.00"), new GUILayoutOption[] {});
        this.walking = GUILayout.HorizontalSlider(this.walking, 0, 1, new GUILayoutOption[] {GUILayout.Width(100)});
        if (GUI.changed)
        {
            this.turning = Mathf.Clamp(Mathf.Abs(this.turning), 0, 1 - this.walking) * Mathf.Sign(this.turning);
            GUI.changed = false;
        }
        GUILayout.Label("Turning (-1 to 1): " + this.turning.ToString("0.00"), new GUILayoutOption[] {});
        this.turning = GUILayout.HorizontalSlider(this.turning, -1, 1, new GUILayoutOption[] {GUILayout.Width(100)});
        if (Mathf.Abs(this.turning) < 0.1f)
        {
            this.turning = 0;
        }
        if (GUI.changed)
        {
            this.walking = Mathf.Clamp(this.walking, 0, 1 - Mathf.Abs(this.turning));
            GUI.changed = false;
        }
        GUILayout.Label("Offset to turning anims (-0.5 to 0.5): " + this.turnOffset.ToString("0.00"), new GUILayoutOption[] {});
        this.turnOffset = GUILayout.HorizontalSlider(this.turnOffset, -0.5f, 0.5f, new GUILayoutOption[] {GUILayout.Width(100)});
        if (Mathf.Abs(this.turnOffset) < 0.05f)
        {
            this.turnOffset = 0;
        }
    }

}