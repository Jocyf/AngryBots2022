using UnityEngine;
using System.Collections;
using TMPro;


public class GameOverUIv2 : MonoBehaviour
{
	const float kDeathCostFactor = 100.0f,
				kBuzzerKillPrize = 100.0f,
				kSpiderKillPrize = 50.0f,
				kMechKillPrize = 500.0f;

	public GameObject gameOverUIPanel;
	public float delay = 5.5f;
	//public float fadeSpeed = 1.0f;
    public TextMeshProUGUI buzzerKillsText, spiderKillsText, mechKillsText, deathsText, timeText, pointsText;
    
    //private float backgroundFade = 0.0f, guiFade = 0.0f;
    private int recordedTime;
    private int deaths, buzzerKills, spiderKills, mechKills, points;
	private bool isRestarting = false;


	private IEnumerator Start ()
	{
		//const float kBackgroundTarget = 0.5f, kGUITarget = 1.0f;

		CalculateScore ();
        ShowScore();

        yield return new WaitForSeconds (delay);

        gameOverUIPanel.SetActive (true);
        
        /*do
		{
			backgroundFade = Mathf.Clamp (backgroundFade + Time.deltaTime * fadeSpeed, 0.0f, kBackgroundTarget);
			yield return null;
		}
		while (backgroundFade < kBackgroundTarget);

		do
		{
			guiFade = Mathf.Clamp (guiFade + Time.deltaTime * fadeSpeed, 0.0f, kGUITarget);
			yield return null;
		}
		while (guiFade < kGUITarget);*/

        Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
        Debug.Log("GameOverUIv2: Timescale: " + Time.timeScale);
    }


	private void CalculateScore ()
	{
		recordedTime = (int)GameScore.GameTime;

		deaths = GameScore.Deaths;
		buzzerKills = GameScore.GetKills ("KamikazeBuzzer");
		spiderKills = GameScore.GetKills ("EnemySpider");
		mechKills = GameScore.GetKills ("EnemyMech") + GameScore.GetKills ("ConfusedEnemyMech");

		points = (int)(buzzerKills * kBuzzerKillPrize + spiderKills * kSpiderKillPrize + mechKills * kMechKillPrize);

		if (deaths != 0) { points /= (int)(deaths * kDeathCostFactor); }
	}

    private void ShowScore()
	{
		buzzerKillsText.text = buzzerKills.ToString();
		spiderKillsText.text = spiderKills.ToString(); ;
		mechKillsText.text = mechKills.ToString();
        deathsText.text = deaths.ToString();
		timeText.text = string.Format("{0}m {1}s", (int)recordedTime / 60, (int)recordedTime % 60);
        pointsText.text = points.ToString();
    }

    /*private IEnumerator DoRestart ()
	{
		yield return null;
		isRestarting = true;
		yield return null;
		DemoControlv2.Instance.Restart ();
	}*/

}
