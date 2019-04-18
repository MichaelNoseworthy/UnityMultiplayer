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
    public static int rScore;
    public static int gScore;
    public static int bScore;
    public static bool GameStarted = false;
    public Text GameTimer;
    public GameObject CountDownPanel;
    static public bool gameWon = false;

    public GameObject endGame;
    static public string Winner;

    public Text wait_Timer;

    public float waitTimer = 7f;

    public GameObject Spawner;

    public const float maxTime = 180;
    static public float inGameTimer;
    static public string currentInGameTime;

    void Start()
    {
        rScore = 0;
        gScore = 0;
        bScore = 0;
        CountDownPanel.SetActive(false);
        RpcSetGameTimer("5");
        inGameTimer = maxTime;
    }

    void Update()
    {

        if (inGameTimer < 0)
        {
            RpcSignalGameOver();
        }
        if (players >= 3 && DoOnce == false)
        {

            if (waitTimer <= 0.2)
            {
                RpcStartGame();
            }
            CountDownPanel.SetActive(true);

            GameObject Player0 = GameObject.Find("Player0");
            if (Player0)
            {
                Player0.GetComponent<PlayerController>().RpcsetWinnerText();
            }
            GameObject Player1 = GameObject.Find("Player1");
            if (Player1)
            {
                Player1.GetComponent<PlayerController>().RpcsetWinnerText();
            }
            GameObject Player2 = GameObject.Find("Player2");
            if (Player2)
            {
                Player2.GetComponent<PlayerController>().RpcsetWinnerText();
            }
            if (waitTimer >= 5)
            {
                RpcSetGameTimer("5");
            }
            if (waitTimer >= 4 && waitTimer <= 5)
            {
                RpcSetGameTimer("4");
            }
            if (waitTimer >= 3 && waitTimer <= 4)
            {
                RpcSetGameTimer("3");
            }
            if (waitTimer >= 2 && waitTimer <= 3)
            {
                RpcSetGameTimer("2");
            }
            if (waitTimer >= 1 && waitTimer <= 2)
            {
                RpcSetGameTimer("1");
            }
            if (waitTimer >= 0 && waitTimer <= 1)
            {
                RpcSetGameTimer("0");

            }
            if (waitTimer <= 0)
            {
            }

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0 && hasAuthority)
            {
                Spawner.GetComponent<AmmoSpawner>().RpcspawnAmmo();

                DoOnce = true;
            }
        }

        if (GameStarted == true)
        {
            if (inGameTimer >0)
            inGameTimer -= Time.deltaTime;

            string minutes = Mathf.Floor(inGameTimer / 60).ToString("00");
            string seconds = (inGameTimer % 60).ToString("00");
            RpcSetInGameTimer(minutes + ":" + seconds);
        }

    }



    [ClientRpc]
    public void RpcStartGame()
    {
        GameStarted = true;
        GameObject Player0 = GameObject.Find("Player0");
        if (Player0)
        {
            Player0.GetComponent<PlayerController>().canMove = true;
        }
        GameObject Player1 = GameObject.Find("Player1");
        if (Player1)
        {
            Player1.GetComponent<PlayerController>().canMove = true;
        }
        GameObject Player2 = GameObject.Find("Player2");
        if (Player2)
        {
            Player2.GetComponent<PlayerController>().canMove = true;
        }
    }

    [ClientRpc]
    public void RpcSetGameTimer(string value)
    {
        GameTimer.text = value;
    }

    [ClientRpc]
    public void RpcSetInGameTimer(string value)
    {
        currentInGameTime = value;
    }

    [ClientRpc]
    public void RpcSignalGameOver()
    {

        string value = "Green Team Wins!";

        int tempInt = gScore;

        if (rScore > tempInt)
        {
            tempInt = rScore;
            value = "Red Team Wins!";
        }

        if (bScore > tempInt)
        {
            tempInt = bScore;
            value = "Blue Team Wins!";
        }

        Winner = value;
        gameWon = true;
    }

    
}

