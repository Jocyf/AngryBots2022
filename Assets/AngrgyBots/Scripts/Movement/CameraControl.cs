using UnityEngine;
using System.Collections;

[System.Serializable]
public class CameraControl : MonoBehaviour
{
    //public Joystick m_LeftJoystick; 
    public Vector2 m_LeftStickPosition;
    public Vector3 moved;
    public Transform cursorObject;
    public Transform m_Player;
    public Vector3 m_Offset2Player;
    // Use this for initialization
    public virtual void Start()
    {
        if (!this.m_Player)
        {
            Debug.LogError("No player found or player is not tagged!");
        }
        this.m_Offset2Player = this.transform.position - this.m_Player.position;
    }

    // Update is called once per frame
    public virtual void Update()//m_LeftStickPosition = m_LeftJoystick.position;
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            // Left stick update
            this.m_LeftStickPosition.x = Input.GetAxis("Horizontal");
            this.m_LeftStickPosition.y = Input.GetAxis("Vertical");
            // Make sure direction vector doesn't exceed length of 1
            if (this.m_LeftStickPosition.sqrMagnitude > 1)
            {
                this.m_LeftStickPosition.Normalize();
            }
        }
        else
        {
        }
    }

}