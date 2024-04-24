using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PauseMenuType { RESUME = 0, MUTE = 1, RESTART = 2, QUIT = 3 }

public class PauseMenuButton : MonoBehaviour
{
    public PauseMenuType pauseMenuTypeButton = PauseMenuType.RESUME;
 

    public void ButtonPressed()
    {
        switch (pauseMenuTypeButton)
        {
            case PauseMenuType.RESUME:
                DemoControlv2.Instance.ResumeGame();
                break;
            case PauseMenuType.MUTE:
                DemoControlv2.Instance.FlipMute();
                break;
            case PauseMenuType.RESTART:
                DemoControlv2.Instance.Restart();
                break;
            case PauseMenuType.QUIT:
                DemoControlv2.Instance.QuitGame();
                break;
        }
    }

}
