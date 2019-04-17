using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public int MaxPlayers = 3;
    public static int players = 0;
    public GameObject timer;
    // Use this for initialization
    private bool DoOnce = false;
    private bool Restart = false;

    public Text redScore;
    public Text blueScore;
    public Text greenScore;
    public static int rScore;
    public static int gScore;
    public static int bScore;

    public GameObject endGame;
    public Text winner; //GGs 

    public Text wait_Timer;

    public float waitTimer = 5f;

    public GameObject networkSpawner;

    GameObject Player0cam;
    GameObject Player1cam;
    GameObject Player2cam;


    void Start()
    {
        rScore = 0;
        gScore = 0;
        bScore = 0;
        endGame.SetActive(false);
    }

    void Update()
    {

        redScore.text = "Red Score: " + rScore;
        greenScore.text = "Green Score: " + gScore;
        blueScore.text = "Blue Score: " + bScore;

        if(rScore>=10)
        {
            winner.text = "Red Wins!";
            RpcEndGame("Red Wins!");
        }
        if (gScore >= 10)
        {
            winner.text = "Green Wins!";
            RpcEndGame("Green Wins!");
        }
        if (bScore >= 10)
        {
            winner.text = "Blue Wins!";
            RpcEndGame("Blue Wins!");
        }

        Debug.Log("players = " + players);
        if (players == MaxPlayers && !DoOnce)
        {
            if (isServer)
            {

            }

            RpcWaitTime();

        }
    }

    [ClientRpc]
    public void RpcWaitTime()
    {
        waitTimer -= Time.deltaTime;
        print(Mathf.Round(waitTimer));

        wait_Timer.text = "Lobby Timer: " + Mathf.Round(waitTimer);

        if (waitTimer <= 0)
        {
            timer.GetComponent<Timer>().StartTimer();
            PlayerController.canFire = true;
            networkSpawner.GetComponent<EnemySpawner>().RpcspawnAmmo();
            GameObject camera = GameObject.Find("LobbyCam");
            camera.SetActive(false);
            GameObject Player0 = GameObject.Find("Player0");
            Player0.transform.position = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            GameObject Player1 = GameObject.Find("Player1");
            Player1.transform.position = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            GameObject Player2 = GameObject.Find("Player2");
            Player2.transform.position = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            DoOnce = true;

        }
    }


    public void EndGameResults(string winnerText)
    {
        endGame.SetActive(true);
        winner.text = winnerText; 
        RpcEndGame(winnerText);
        Time.timeScale = 0;
    }
    [ClientRpc]
    public void RpcEndGame(string winnerText)
    {
        endGame.SetActive(true);
        winner.text = winnerText;
        Time.timeScale = 0;
    }
}


