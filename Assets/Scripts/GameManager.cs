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

    public GameObject endGame;
    public Text winner; //GGs 

    public Text wait_Timer;

    public float waitTimer = 7f;

    public GameObject Spawner;

    public const float maxTime = 120;
    public float inGameTimer;
    static public string currentInGameTime;

    void Start()
    {
        rScore = 0;
        gScore = 0;
        bScore = 0;
        CountDownPanel.SetActive(false);
        RpcSetGameTimer("5");
        inGameTimer = maxTime;
        
        // endGame.SetActive(false);
    }

    void Update()
    {
        

        if (players >= 2 && DoOnce == false)
        {
            RpcSendScoreInfo();
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
            /*
            if (Player0)
            {
                Player0.GetComponent<PlayerController>().CountDownPanel.SetActive(true);
            }
            if (Player1)
            {
                Player1.GetComponent<PlayerController>().CountDownPanel.SetActive(true);
            }
            if (Player2)
            {
                Player2.GetComponent<PlayerController>().CountDownPanel.SetActive(true);
            }

        */
            if (waitTimer >= 5)
            {
                RpcSetGameTimer("5");
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().RpcsetCountDownText("5");
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().RpcsetCountDownText("5");
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().RpcsetCountDownText("5");
                }
                */
            }
            if (waitTimer >= 4 && waitTimer <= 5)
            {
                RpcSetGameTimer("4");
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().RpcsetCountDownText("4");
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().RpcsetCountDownText("4");
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().RpcsetCountDownText("4");
                }
                */
            }
            if (waitTimer >= 3 && waitTimer <= 4)
            {
                RpcSetGameTimer("3");
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().RpcsetCountDownText("3");
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().RpcsetCountDownText("3");
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().RpcsetCountDownText("3");
                }
                */
            }
            if (waitTimer >= 2 && waitTimer <= 3)
            {
                RpcSetGameTimer("2");
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().RpcsetCountDownText("2");
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().RpcsetCountDownText("2");
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().RpcsetCountDownText("2");
                }
                */
            }
            if (waitTimer >= 1 && waitTimer <= 2)
            {
                RpcSetGameTimer("1");
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().RpcsetCountDownText("1");
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().RpcsetCountDownText("1");
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().RpcsetCountDownText("1");
                }
                */
            }
            if (waitTimer >= 0 && waitTimer <= 1)
            {
                RpcSetGameTimer("0");
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().RpcsetCountDownText("0");
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().RpcsetCountDownText("0");
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().RpcsetCountDownText("0");
                }
                */

            }
            if (waitTimer <= 0)
            {
                /*
                if (Player0)
                {
                    Player0.GetComponent<PlayerController>().CountDownPanel.SetActive(false);
                }
                if (Player1)
                {
                    Player1.GetComponent<PlayerController>().CountDownPanel.SetActive(false);
                }
                if (Player2)
                {
                    Player2.GetComponent<PlayerController>().CountDownPanel.SetActive(false);
                }
                */
            }

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                Spawner.GetComponent<AmmoSpawner>().RpcspawnAmmo();

                DoOnce = true;
            }
        }

        if (GameStarted == true)
        {
            inGameTimer -= Time.deltaTime;

            string minutes = Mathf.Floor(inGameTimer / 60).ToString("00");
            string seconds = (inGameTimer % 60).ToString("00");
            RpcSetInGameTimer(minutes + ":" + seconds);
        }

        if (rScore>=10)
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
    public void RpcSendScoreInfo()
    {


        /*
        redScore.text = "Red Score: " + rScore;
        greenScore.text = "Green Score: " + gScore;
        blueScore.text = "Blue Score: " + bScore;
        */
    }

    [ClientRpc]
    public void RpcWaitTime()
    {
        /*
        waitTimer -= Time.deltaTime;
        print(Mathf.Round(waitTimer));

        wait_Timer.text = "Lobby Timer: " + Mathf.Round(waitTimer);

        if (waitTimer <= 0)
        {
            timer.GetComponent<GameTimer>().StartTimer();
            PlayerController.isFireEnabled = true;
            networkSpawner.GetComponent<AmmoSpawner>().RpcspawnAmmo();
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
        */
    }


    public void EndGameResults(string winnerText)
    {
        /*
        endGame.SetActive(true);
        winner.text = winnerText; 
        RpcEndGame(winnerText);
        Time.timeScale = 0;
        */
    }
    [ClientRpc]
    public void RpcEndGame(string winnerText)
    {
        /*
        endGame.SetActive(true);
        winner.text = winnerText;
        Time.timeScale = 0;
        */
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

}


