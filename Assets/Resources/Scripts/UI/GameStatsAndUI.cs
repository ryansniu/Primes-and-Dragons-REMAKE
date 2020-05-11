using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsAndUI : MonoBehaviour {
    private const double MAX_TIME = 3600 * 99 + 60 * 99 + 99;

    public int currFloor = 0;
    public Button floorButton;
    public TextMeshProUGUI floorNum;

    public double elapsedTime = 0;
    public Button timerButton;
    public TextMeshProUGUI timerText;

    public Button upper1;
    public Button upper2;
    public Button pauseButton;

    private bool isRunning = false;

    void Update() {
        if (isRunning && !GameController.isPaused) updateText();
    }

    public void updateText() {
        elapsedTime = Math.Min(elapsedTime + Time.deltaTime, MAX_TIME);
        timerText.text = TimeSpan.FromSeconds(elapsedTime).ToString(@"hh\:mm\:ss");
        floorNum.text = string.Concat("floor: ", currFloor.ToString().PadLeft(2, '0'));
    }

    public void toggle(bool isRun) {
        isRunning = isRun;
        floorButton.interactable = isRunning;
        upper1.interactable = isRunning;
        timerButton.interactable = isRunning;
        upper2.interactable = isRunning;
        pauseButton.interactable = isRunning;
    }
}
