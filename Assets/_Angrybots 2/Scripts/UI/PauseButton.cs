using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void PauseGame() { DemoControlv2.Instance.PauseGame(); }

    public void ResumeGame() { DemoControlv2.Instance.ResumeGame(); }
}
