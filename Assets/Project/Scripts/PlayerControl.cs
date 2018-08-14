using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour 
{
	[HideInInspector]
	public GameManager gManager;	//Reference to the GameMagnager

	public Transform bulletSpawner;	//Reference to the Trasform where player bullet will be come out


	float lastBulletTime =0;		//used to count the time between evere bullet fired

	void Update () 
	{
		PlayerInput();
	}

	// Check player inputs
	void PlayerInput()
	{
		float xDisplacement = Input.GetAxis("Horizontal") * gManager.playerSpeed;

		if(gManager.IsInBorders(transform, xDisplacement) )
		transform.Translate(xDisplacement, 0f, 0f);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Fire();
		}
	}
	// Fire a bullet
	void Fire()
	{
		if (Time.time - lastBulletTime > gManager.timeBBullets)
		{
		    Transform newBullet = gManager.bulletPool.Instantiate();
			newBullet.GetComponent<BulletControl>().shipType = ShipType.PLAYER;
			newBullet.GetComponent<BulletControl>().gManager = gManager;
			newBullet.transform.position = bulletSpawner.position;
			newBullet.GetComponent<BulletControl>().Init();
			lastBulletTime = Time.time;	
		}
	}

	//disable the player gameObject and report to the gameMagager
	public void Die()
	{
		gManager.CheckGameover();
		gameObject.SetActive(false);
	}
}
