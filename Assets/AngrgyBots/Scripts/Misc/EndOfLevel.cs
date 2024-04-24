using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(BoxCollider))]
public partial class EndOfLevel : MonoBehaviour
{
    public float timeToTriggerLevelEnd = 2f;
    public string endSceneName = "Escape";

    public virtual IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.StartCoroutine(FadeOutAudio());
            /* OLD */
            /*PlayerMoveController playerMove = other.gameObject.GetComponent<PlayerMoveController>();
            playerMove.enabled = false;
            yield return null;
            float timeWaited = 0f;
            FreeMovementMotor playerMotor = other.gameObject.GetComponent<FreeMovementMotor>();
            while (playerMotor.walkingSpeed > 0f)
            {
                playerMotor.walkingSpeed = playerMotor.walkingSpeed - (Time.deltaTime * 6f);
                if (playerMotor.walkingSpeed < 0f)
                {
                    playerMotor.walkingSpeed = 0f;
                }
                timeWaited = timeWaited + Time.deltaTime;
                yield return null;
            }
            playerMotor.walkingSpeed = 0f;*/

            PlayerMovementAndLook playerMove = other.gameObject.GetComponent<PlayerMovementAndLook>();
           
            yield return null;
            float timeWaited = 0f;
            while (playerMove.speed > 0f)
            {
                playerMove.speed -= Time.deltaTime * 6f;
                if (playerMove.speed < 0f)
                {
                    playerMove.speed = 0f;
                }
                timeWaited = timeWaited + Time.deltaTime;
                yield return null;
            }
            playerMove.speed = 0f;
            playerMove.enabled = false;

            yield return new WaitForSeconds(Mathf.Clamp(timeToTriggerLevelEnd - timeWaited, 0f, timeToTriggerLevelEnd));
            Camera.main.gameObject.SendMessage("WhiteOut");
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(endSceneName);
            //Application.LoadLevel(this.endSceneName);
        }
    }

    public virtual IEnumerator FadeOutAudio()
    {
        AudioListener al = Camera.main.gameObject.GetComponent<AudioListener>();
        if (al)
        {
            while (AudioListener.volume > 0f)
            {
                AudioListener.volume -= Time.deltaTime / timeToTriggerLevelEnd;
                yield return null;
            }
        }
    }



}