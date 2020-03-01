using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour {
    private const double MAX_TIME = 3600 * 99 + 60 * 99 + 99;
    public double elapsedTime = 0;
    public bool isRunning = false;
    public TextMeshPro timerText;
    void Update() {
        if (isRunning && !GameController.isPaused) updateText();
        timerText.color = isRunning ? Color.white : Color.grey;
    }

    public void updateText() {
        elapsedTime = System.Math.Min(elapsedTime + Time.deltaTime, MAX_TIME);
        timerText.text = TimeSpan.FromSeconds(elapsedTime).ToString(@"hh\:mm\:ss");
    }
}
