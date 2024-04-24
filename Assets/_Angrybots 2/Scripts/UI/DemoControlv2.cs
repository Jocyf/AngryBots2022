using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoControlv2 : MonoBehaviour
{
    public bool isPaused = false;
    public bool audioEnabled = true;

    #region SingletonPersistent
    private static DemoControlv2 instance;

    public static DemoControlv2 Instance { get { return instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void OnDestroy()
    {
        if (this == instance) { instance = null; }
    }
    #endregion

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void PauseGame() { Time.timeScale = 0.0f; isPaused = true; }

    public void ResumeGame() { Time.timeScale = 1.0f; isPaused = false; }

    public void UpdateAudio() { AudioListener.volume = audioEnabled ? 1.0f : 0.0f; }

    public void FlipMute() { audioEnabled = !audioEnabled; UpdateAudio(); }

    public void QuitGame() { Application.Quit(); }

    public void Restart()
    {
        ResumeGame();
        GameScore.Reset();
        SceneManager.LoadScene(0);
    }

    
    
    /*private void Start() { UpdateAudio(); }*/

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { UpdateAudio(); }

}
