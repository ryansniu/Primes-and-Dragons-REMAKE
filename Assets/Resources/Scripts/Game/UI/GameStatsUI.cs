using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsUI : MonoBehaviour {
    [SerializeField] private Button floorButton, timerButton, upper1, upper2, pauseButton;
    [SerializeField] private TextMeshProUGUI floorNum, timerText;

    public void updateText(GameState gs) {
        timerText.text = TimeSpan.FromSeconds(gs.elapsedTime).ToString(@"hh\:mm\:ss");
        floorNum.text = string.Concat("floor: ", gs.floor.ToString().PadLeft(2, '0'));
    }

    // I have given up on writing readable code. Good luck!
    public void toggleAll(bool isRunning) { pauseButton.interactable = upper2.interactable = timerButton.interactable = upper1.interactable = floorButton.interactable = isRunning; }
    public void togglePauseButton(bool isRunning) { pauseButton.interactable = isRunning; }
}
