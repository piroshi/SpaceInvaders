using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour 
{
	// The bullet is used by the enemies and  the player
	[HideInInspector]
	public ShipType shipType;		//Who fired the bullet

	[HideInInspector]
	public GameManager gManager;	//Referecne to the GameManager

	int directionMultiplier = 1;	// Used to set the Y direction of the bullet, dependig if the bullet is Enemy's or Players's


	void Update () 
	{
		Move();
	}

	// Define direction of the bullet
	public void Init()
	{
		GetComponent<AudioSource>().Play();

			if (shipType == ShipType.ENEMY)
			{
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
			}
			else
			{
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0));
			}
	}

	// Advace in Y axis according to the direction defined
	void Move()
	{
		transform.Translate(0f, directionMultiplier * gManager.bulletSpeed, 0f);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// If the bullet reach the screen limit it will be destroy
		if (collision.gameObject.tag == "Top" || collision.gameObject.tag == "Bottom")
		{
			gManager.bulletPool.Destroy(transform);
		}
		// If the bullet hit an enemy both will be destroyed
		else if (collision.gameObject.tag == "Enemy" && shipType == ShipType.PLAYER)
		{
			collision.transform.GetComponent<EnemyControl>().Die();
			gManager.bulletPool.Destroy(transform);
		}
		// If the bullet hit the player both will be destroyed
		else if (collision.gameObject.tag == "Player" && shipType == ShipType.ENEMY)
		{
			collision.transform.GetComponent<PlayerControl>().Die();
			gManager.bulletPool.Destroy(transform);
		}
		//If the bullet hit another bullet both will be destroyed
		else if (collision.gameObject.tag == "Bullet")
		{
			gManager.bulletPool.Destroy(collision.transform);
			gManager.bulletPool.Destroy(transform);
		}

	}
}
