using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameScore : MonoBehaviour
{
	private static GameScore instance;

    #region SingletonPersistent
    public static GameScore Instance { get { return instance; } }

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

    void OnDestroy ()
	{
        if (this == instance) { instance = null; }
    }
	#endregion

	public string playerLayerName = "Player", enemyLayerName = "Enemies";


	[Space(10)]
	[Header("Runtime Whatchers")]
	public int deaths = 0;
	public int numKills = 0;
    public Dictionary<string, int> kills = new Dictionary<string, int> ();
	public float startTime = 0.0f;
	

	
	public static int Deaths
	{
		get
		{
			if (Instance == null)
			{
				return 0;
			}
			
			return Instance.deaths;
		}
	}
	
	
	public static ICollection<string> KillTypes
	{
		get
		{
			if (Instance == null)
			{
				return new string[0];
			}
				
			return Instance.kills.Keys;
		}
	}
	
	
	public static int GetKills (string type)
	{
		if (Instance == null || !Instance.kills.ContainsKey (type))
		{
			return 0;
		}
		
		return Instance.kills[type];
	}
	
	
	public static float GameTime
	{
		get
		{
			if (Instance == null)
			{
				return 0.0f;
			}
			
			return Time.time - Instance.startTime;
		}
	}

	public static void Reset()
	{
        Instance.deaths = 0;
        Instance.numKills = 0;
        Instance.kills.Clear() ;
        Instance.startTime = 0.0f;
	}


	public static void RegisterDeath (GameObject deadObject)
	{
		if (Instance == null)
		{
			Debug.Log ("Game score not loaded");
			return;
		}
		
		int
			playerLayer = LayerMask.NameToLayer (Instance.playerLayerName),
			enemyLayer = LayerMask.NameToLayer (Instance.enemyLayerName);
			
		if (deadObject.layer == playerLayer)
		{
			Instance.deaths++;
		}
		else if (deadObject.layer == enemyLayer)
		{
			Instance.kills[deadObject.name] = Instance.kills.ContainsKey(deadObject.name) ? Instance.kills[deadObject.name] + 1 : 1;
            Instance.numKills++;
			Debug.Log("Enemy Killed: " + deadObject.name + " - kills: " + GetKills(deadObject.name));
        }
	}

    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
		if (startTime == 0.0f)
		{
			startTime = Time.time;
		}
	}
}
