using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Timer : NetworkBehaviour {

	public Text timer;

	public const float maxTime = 100;

    [SyncVar(hook = "OnChangeTime")]
	public float currentTime = maxTime;

	public GameObject endGamePanel;

	private bool DoOnce = false;

	bool gameStarted = false;
	// Use this for initialization
	void Start () {
		timer.gameObject.SetActive (true);	
	}

	public void ReduceTime(float amount)
	{
		if (!isServer)
		{
			return;
		}

		currentTime -= amount;

		if (currentTime <= 0.0f)
		{
            //GameManager.winner.text = "Game Over!";
            RpcEndGame();
		}

		if (currentTime <= 30.0f &&!DoOnce)
		{
			DoOnce = true;
		}
	}

	public void EndGameResults()
	{
		endGamePanel.SetActive (true);
        Time.timeScale = 0;
		RpcEndGame ();
	}

	[ClientRpc]
	public void RpcEndGame()
	{
		endGamePanel.SetActive (true);
        Time.timeScale = 0;
    }

    public void StartTimer()
	{
		gameStarted = true;
	}

	public void RestartTimer()
	{
		currentTime = maxTime;
	}

	void OnChangeTime(float currentTime)
	{
		timer.text = "Time Left: " + Mathf.Round(currentTime).ToString() + "s";
	}
	
	// Update is called once per frame
	void Update ()
	{


		if(gameStarted)
		    ReduceTime(0.01f);
	}
}
