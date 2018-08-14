using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	//Editable Parameters
	public float enemySpeed = 0.009f;		// X speed of enemies on screen
	public float speedIncrement = 0.0013f;	// Increment of enemies' speed every time an enemy dies
	public int enemyRows = 4;				// number of rows of enemies on screen
	public int enemyCols = 11;				// number of enemies per row
	public float offSetEnemies = 0.8f;		// distance between enemies
	public float yDisplacement = -0.08f;	// enemies' displacement in Y axis when one of them reach one screen limit
	public float playerSpeed = 0.1f;		// X speed of the player
	public float timeBBullets = 0.5f;		// Time between every bullet of the player
	public float bulletSpeed = 0.05f;		// Speed of the bullets

	//References from the editor
	public Transform enemyPrefab;			//Prefab of the enemy to instantiate
	public Transform PlayerPrefab;			//Prefab of the player ship to instantiate
	public PoolControl bulletPool;			//Reference to the pool of bullets
	public Transform leftLimit;				//Reference to the transform that marks the the left limit  of the screen
	public Transform rightLimit;			//Reference to the transform that marks the the right limit  of the screen
	public Vector3 playerIniPos = new Vector3(0f, -4f, 0f);		//Initial position where the player will be spawed

	public Text livesText;					//Reference to the text of UI that shows remaining lives
	public Text scoreText;					//Reference to the text of UI that shows current score
	public Text levelText;					//Reference to the text of UI that shows current level
	public Text middleText;					//Reference to the text of UI that shows the big text on the middle of the screen

	//Non editable parameters
	int _lives;     // Player remaining ships
	int _score;     // Current score
	int _level;     // Current level
	int _remainingEnemies;
	float pauseTime = 3f;	

	//Other variables
	List<Transform> enemiesInLevel;     //List of enemies, when an enemy is destroyed his references is null
					
	Vector3 firstEnemyPos = new Vector3(-4f, 3f, 0f);	//Initial position where the enemies will be spawed
	Transform player;					//Reference on the game of the player
	AudioSource explosionAU;			//References of the AudioSorce used to play an explosion sound
				
	float currentPauseTime = 0;			// current time since the pause started
	[HideInInspector]
	public bool movingLeft = false;		//flag to know if the enemies are moving left or right

	[HideInInspector]
	public GameState gameState;			//Current state of the game

	//variable with SET & GET used externaly and/or with adition action when the values changes

	public int RemainigEnemies
	{
		get
		{
			return _remainingEnemies;
		}
		set
		{
			_remainingEnemies = value;
		}
	}

	public int Lives
	{
		get
		{
			return _lives;
		}
		set
		{
			if (value >= 0 && value < 100)
			{
				_lives = value;
				ChangeLivesText(_lives);
			}
		}
	}

	public int Score
	{
		get
		{
			return _score;
		}
		set
		{
			if (value >= 0 && value < 999999)
			{
				_score = value;
				CheckNewLive();
				ChangeScoreText(_score);
			}
		}
	}

	public int Level
	{
		get
		{
			return _level;
		}
		set
		{
			if (value > 0 && value < 100)
			{
				_level = value;
				ChangeLevelText(_level);
			}
		}
	}


	void Start () 
	{
		Init();
	}

	void Update () 
	{

		//These are the only game states used to Game Manager
		switch (gameState)
		{
			case GameState.PAUSE:
				Pause();
				break;
			case GameState.GAMEOVER:
				GameOver();
				break;
		}

		//Q to quit the game
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Application.Quit();
		}
	}

	//Initialize game values and references
	void Init()
	{
			if (bulletPool == null)
				bulletPool = transform.GetComponent<PoolControl>();
			if (bulletPool == null)
				Debug.LogError("Error: BulletPool not Found");

		gameState = GameState.INTRO;  //Intro state didn't need aditiona behaviour but keep the game paused
		StartCoroutine(CreateSquadron());
		explosionAU = transform.Find("EnemyAudio").GetComponent<AudioSource>();

			if (explosionAU == null)
				Debug.LogError("ERROR: EnemyAudio");

		//Level = 1;
		//Score = 0;
		//Lives = 3;

		Level = PlayerPrefs.GetInt("level", 0);
		Score = PlayerPrefs.GetInt("score", 0);
		Lives = PlayerPrefs.GetInt("lives", 0);

		//initialSpeed += (speedIncrement * 2);
		enemySpeed += (speedIncrement * Level*2);

		//initialSpeed = enemySpeed;
	}

	/// <summary>
	/// Verifies if the last enemy was destroyed
	/// </summary>
	public void CheckEndLevel()
	{
		if (RemainigEnemies <= 0)
			LoadNextLevel();
	}

	/// <summary>
	/// Play an explosion sound
	/// </summary>
	public void PlayDieSound()
	{
		explosionAU.Play();
	}

	/// <summary>
	/// Every 20000 points the player wins a live
	/// </summary>
	void CheckNewLive()
	{
		if (Score % 20000 == 0 && Score > 0)
		{
			Lives += 1;
			GetComponent<AudioSource>().Play();
		}
	}

	/// <summary>
	/// Creates the enemy squadron.
	/// </summary>
	IEnumerator CreateSquadron()
	{
		enemiesInLevel = new List<Transform>();
		int id =0;
		
			for (int y = 0; y < enemyRows; y++)
			{
				for (int x = 0; x < enemyCols; x++)
				{
					
					Transform newShip = Instantiate(enemyPrefab, new Vector3(firstEnemyPos.x + (offSetEnemies * x), firstEnemyPos.y - (offSetEnemies * y), 0f), Quaternion.identity);
					newShip.gameObject.name = "Ship" + id;	
					newShip.GetComponent<EnemyControl>().EnemyID = id;
					newShip.GetComponent<EnemyControl>().gManager = this;


						if (y == enemyRows - 1)
						newShip.GetComponent<EnemyControl>().Leader = true;
					
					enemiesInLevel.Add(newShip);
					yield return new WaitForSeconds(0.02f);
					id++;
				}
			}

		RemainigEnemies = id;
		EnterInGame();
	}

	/// <summary>
	/// Changes the squadron direction.
	/// </summary>
	public void ChangeSquadronDirection()
	{
		for (int i = 0; i < enemiesInLevel.Count; i++)
		{
			if (enemiesInLevel[i] != null)
			{
				enemiesInLevel[i].GetComponent<EnemyControl>().MoveY();

			}

		}
		movingLeft = !movingLeft;
		enemySpeed *= -1;
	}

	/// <summary>
	/// Enter to InGame state
	/// </summary>
	void EnterInGame()
	{
		gameState = GameState.INGAME;
		middleText.gameObject.SetActive(false);

		player = Instantiate(PlayerPrefab, playerIniPos, Quaternion.identity);
		player.GetComponent<PlayerControl>().gManager = this;
		player.GetComponent<PlayerControl>().gManager.bulletPool = bulletPool;
	}

	//Methods that change the UI text of their corresponding variables
	void ChangeLivesText(int livesValue)
	{
		livesText.text = "X " + livesValue.ToString();
	}
	void ChangeScoreText(int newScore)
	{
		scoreText.text = "Score: "+ newScore.ToString();
	}
	void ChangeLevelText(int newLevel)
	{
		levelText.text = "Level: "+ newLevel.ToString();
	}

	//Inlcrese enemy speed every time an enemy is eliminated
	public void Accelerate()
	{
		if (enemySpeed > 0)
			enemySpeed += speedIncrement;
		else if(enemySpeed < 0)
			enemySpeed -= speedIncrement;
		

	}

	//Verify the the player hasn't go out of the screen borders
	public bool IsInBorders(Transform obj,float xDisplament)
	{
		if (obj.position.x + xDisplament > leftLimit.GetComponent<Collider2D>().bounds.max.x && obj.position.x + xDisplament < rightLimit.GetComponent<Collider2D>().bounds.min.x)
		{
			return true;
		}
		else
			return false;
	}

	//If enemy leader (The ship below the other in every colum) is destroyed the one behind it is asigned as leader so it will be able to Fire
	public bool AssignNewLeader(int lastLeader)
	{
			if (RemainigEnemies >= 1)
			{

				int newLeaderIndex = lastLeader - enemyCols;

				if (newLeaderIndex < 0)
					return false;

				if (enemiesInLevel[newLeaderIndex] == null)
					AssignNewLeader(newLeaderIndex);
				else
				{
					enemiesInLevel[newLeaderIndex].GetComponent<EnemyControl>().SetAsLeader();
					return true;
				}
			}

		return true;
	}

	//Reaload the scene and save the values
	void LoadNextLevel()
	{
		Level += 1;
		PlayerPrefs.SetInt("lives", Lives);
		PlayerPrefs.SetInt("score", Score);
		PlayerPrefs.SetInt("level", Level);

		SceneManager.LoadScene("MainGame");

	}
	//Check if all the players ships has been destroyed
	public void CheckGameover()
	{
		explosionAU.Play();
		bulletPool.DestroyAll();
		Lives -= 1;

			if (Lives <= 0)
			{
				EnterToGameOver();
			}
			else
			{
				currentPauseTime = Time.time;
				gameState = GameState.PAUSE;
			}
	}

	//Prepare to enter to GameOver state
	public void EnterToGameOver()
	{
		gameState = GameState.GAMEOVER;
		PlayerPrefs.SetInt("lives", 3);
		PlayerPrefs.SetInt("score", 0);
		PlayerPrefs.SetInt("level", 1);

		middleText.text = "Game Over";
		middleText.transform.Find("Message").gameObject.SetActive(true);
		middleText.gameObject.SetActive(true);
	}

	//Pause the game afterthe player receive a hit
	void Pause()
	{
		if (Time.time - currentPauseTime > pauseTime)
		{
			player.gameObject.SetActive(true);
			gameState = GameState.INGAME;

		}
	}
	//Game over state. Just wait for the player to M or Q
	void GameOver()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			SceneManager.LoadScene("Menu");
		}
	}

}

public enum GameState { INTRO, INGAME,PAUSE,WINING,GAMEOVER}
public enum ShipType { PLAYER, ENEMY };
