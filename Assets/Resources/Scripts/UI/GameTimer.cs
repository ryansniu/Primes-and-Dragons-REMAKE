using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {
    private const double MAX_TIME = 3600 * 99 + 60 * 99 + 99;
    public double elapsedTime = 0;
    public TextMeshProUGUI timerText;
    public Button timerButton;
    private bool isRunning = false;

    void Update() {
        if (isRunning && !GameController.isPaused) updateText();
    }

    public void updateText() {
        elapsedTime = Math.Min(elapsedTime + Time.deltaTime, MAX_TIME);
        timerText.text = TimeSpan.FromSeconds(elapsedTime).ToString(@"hh\:mm\:ss");
    }

    public void toggle(bool isRun) {
        isRunning = isRun;
        timerButton.interactable = isRunning;
    }
}
