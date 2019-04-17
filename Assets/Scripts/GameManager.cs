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

    public GameObject endGame;
    public Text winner; //GGs 

    public Text wait_Timer;

    public float waitTimer = 5f;

    public GameObject Spawner;
    /*
    GameObject Player0cam;
    GameObject Player1cam;
    GameObject Player2cam;
    */

    void Start()
    {
        rScore = 0;
        gScore = 0;
        bScore = 0;
       // endGame.SetActive(false);
    }

    void Update()
    {

        if (players >= 1 && DoOnce == false)
        {
            RpcSendScoreInfo();

            GameObject Player0 = GameObject.Find("Player0");
            Player0.GetComponent<PlayerController>().RpcsetWinnerText();
            //GameObject Player1 = GameObject.Find("Player1");
            //Player1.GetComponent<PlayerController>().RpcsetWinnerText();
            //GameObject Player2 = GameObject.Find("Player2");
            //Player2.GetComponent<PlayerController>().RpcsetWinnerText();

            if (waitTimer >= 5)
            {
                Player0.GetComponent<PlayerController>().RpcsetCountDownText("5");
                //Player1.GetComponent<PlayerController>().RpcsetCountDownText("5");
                //Player2.GetComponent<PlayerController>().RpcsetCountDownText("5");
            }
            if (waitTimer >= 4 && waitTimer <= 4.9)
            {
                Player0.GetComponent<PlayerController>().RpcsetCountDownText("4");
                //Player1.GetComponent<PlayerController>().RpcsetCountDownText("4");
                //Player2.GetComponent<PlayerController>().RpcsetCountDownText("4");
            }
            if (waitTimer >= 3 && waitTimer <= 3.9)
            {
                Player0.GetComponent<PlayerController>().RpcsetCountDownText("3");
                //Player1.GetComponent<PlayerController>().RpcsetCountDownText("3");
                //Player2.GetComponent<PlayerController>().RpcsetCountDownText("3");
            }
            if (waitTimer >= 2 && waitTimer <= 2.9)
            {
                Player0.GetComponent<PlayerController>().RpcsetCountDownText("2");
                //Player1.GetComponent<PlayerController>().RpcsetCountDownText("2");
                //Player2.GetComponent<PlayerController>().RpcsetCountDownText("2");
            }
            if (waitTimer >= 1 && waitTimer <= 1.9)
            {
                Player0.GetComponent<PlayerController>().RpcsetCountDownText("1");
                //Player1.GetComponent<PlayerController>().RpcsetCountDownText("1");
                //Player2.GetComponent<PlayerController>().RpcsetCountDownText("1");
            }
            if (waitTimer >= 0 && waitTimer <= 1.9)
            {

                Player0.GetComponent<PlayerController>().RpcsetCountDownText("0");
                //Player1.GetComponent<PlayerController>().RpcsetCountDownText("0");
                //Player2.GetComponent<PlayerController>().RpcsetCountDownText("0");

                Player0.GetComponent<PlayerController>().CountDownPanel.SetActive(false);
                //Player1.GetComponent<PlayerController>().CountDownPanel.SetActive(false);
                //Player2.GetComponent<PlayerController>().CountDownPanel.SetActive(false);
            }

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                Spawner.GetComponent<AmmoSpawner>().RpcspawnAmmo();

                DoOnce = true;
            }
        }

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
}


