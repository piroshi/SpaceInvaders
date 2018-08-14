using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{
	int lives = 3;
	int score = 0;
	int level = 1;

	void Start () 
	{
		
	}
	

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			PlayerPrefs.SetInt("lives", lives);
			PlayerPrefs.SetInt("score", score);
			PlayerPrefs.SetInt("level", level);
			SceneManager.LoadScene("MainGame");
		}
		else if (Input.GetKeyDown(KeyCode.Q))
		{
			Application.Quit();
		}
	}
}
