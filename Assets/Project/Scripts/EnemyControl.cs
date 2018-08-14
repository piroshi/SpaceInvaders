using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour 
{

	private int enemyID;			//used to identify an enemy inside the squadron
	private bool _leader = false;	// more botton of the screen. Can Fire

	float ranMinValue = 3;			//Minimum time every bullet take to be fired
	float ranMaxValue = 5;			//Maximum time every bullet kake to be fired
	float newRanTime = 0;			//random time generated between limis values
	float lastShootTime = 0;		//Time when the lasd bullet was fired

	public Transform bulletSpawner;	//Reference to the Transform where enemy bullet will be fired

	[HideInInspector]
	public GameManager gManager;	//Reference to the GameManager

	//variable with SET & GET used externaly 

	public int EnemyID
	{
		get
		{
			return enemyID;
		}
		set
		{
			enemyID = value;
		}
	}

	public bool Leader
	{
		get
		{
			return _leader;
		}
		set
		{
			_leader = value;
		}
	}

	void Start () 
	{
		Init();

	}

	void Update () 
	{
		//Only 2 game state are used by the enemies
		switch (gManager.gameState)
		{
			case GameState.INGAME:
				MoveX();
				Fire();
				break;
			case GameState.PAUSE:
				setNewShootTime();
				break;
		}
	}

	//Initializa enemy
	public void Init()
	{
		setNewShootTime();
	}

	// Generate a Random number between the limits 
	public void setNewShootTime()
	{
		lastShootTime = Time.time;
		newRanTime = Random.Range(ranMinValue, ranMaxValue);
	}
	//Move enemy over X axis acording to the gameManager speed and direccion
	public void MoveX()
	{
		transform.Translate(new Vector3(gManager.enemySpeed, 0f, 0f),Space.World);
	}
	// Move enemy over Y axis acording to the gameManager YDisplacement
	public void MoveY()
	{
		transform.Translate(new Vector3(0f, gManager.yDisplacement, 0f), Space.World);
	}

	//If the enemy ship is a leader will fire a bullet every time defined in setNewShootTime()
	public void Fire()
	{
		if (Leader)
		{
			if (Time.time - lastShootTime >= newRanTime)
			{
				setNewShootTime();
				Transform newBullet = gManager.bulletPool.Instantiate();
				newBullet.GetComponent<BulletControl>().shipType = ShipType.ENEMY;
				newBullet.GetComponent<BulletControl>().gManager = gManager;
				newBullet.transform.position = bulletSpawner.position;
				newBullet.GetComponent<BulletControl>().Init();
			}
		}
	}

	//Convert current ship in Leader
	public void SetAsLeader()
	{
		Leader = true;
		setNewShootTime();
	}

	// Destroy current enemy
	public void Die()
	{
		gManager.Score += 100;
		gManager.RemainigEnemies --;
		gManager.Accelerate();
		gManager.CheckEndLevel();
		gManager.PlayDieSound();
		Destroy(gameObject);

			if (Leader)
				gManager.AssignNewLeader(enemyID);
	}

	//Basicaly the enemies only verify if they collide with the screen borders to change direction or with the player
	private void OnCollisionEnter2D(Collision2D collision)
	{


		// XOR operator value's table:
		// MovingLeft   |  Hit with the Right border|	Change direction
		// _____________|___________________________|___________________
		//		False	|			False			|			False
		//		True	|			False			|			True
		//		True	|			True			|			False
		//		False	|			True			|			True


		if (((collision.transform.tag == "Right") ^ gManager.movingLeft) && collision.transform.tag != "Bullet")
		{
			//Debug.Log(collision.gameObject.tag + " , " + collision.gameObject.name);

			gManager.ChangeSquadronDirection();
		}
		else if (collision.transform.tag == "Bottom")
		{
			gManager.EnterToGameOver();
		}
		else if (collision.transform.tag == "Player")
		{
			collision.transform.GetComponent<PlayerControl>().Die();
			Die();
		}
	}
}
