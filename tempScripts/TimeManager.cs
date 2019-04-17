using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimeManager : NetworkBehaviour
{

    public Text timerText;
    public float gameTimer = 180f; //3MINS

    public override void OnStartClient()
    {
        gameTimer -= Time.deltaTime;
        timerText.text = "Time Left: " + gameTimer;
    }
}
