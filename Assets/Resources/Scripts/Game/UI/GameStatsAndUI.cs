using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsAndUI : MonoBehaviour {
    private const double MAX_TIME = 3600 * 99 + 60 * 99 + 99;
    public static bool pauseManualDisable = false;

    public Button floorButton, timerButton;
    public Button upper1, upper2, pauseButton;
    public TextMeshProUGUI floorNum, timerText;

    public int currFloor = 0; // TO-DO: UI element for this
    public double elapsedTime = 0;
    private bool isRunning = false;

    void Update() {
        if (isRunning && !GameController.isPaused) updateText();
    }

    public void updateText() {
        elapsedTime = Math.Min(elapsedTime + Time.deltaTime, MAX_TIME);
        timerText.text = TimeSpan.FromSeconds(elapsedTime).ToString(@"hh\:mm\:ss");
        floorNum.text = string.Concat("floor: ", currFloor.ToString().PadLeft(2, '0'));
    }

    // I have given up on writing readable code. Good luck!
    public void toggle(bool isRun) { pauseButton.interactable = upper2.interactable = timerButton.interactable = upper1.interactable = floorButton.interactable = isRunning = isRun; }
}
