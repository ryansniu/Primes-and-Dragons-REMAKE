using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsUI : MonoBehaviour {
    [SerializeField] private Button floorButton = default, timerButton = default, upper1 = default, upper2 = default, pauseButton = default;
    [SerializeField] private TextMeshProUGUI floorNum = default, timerText = default;

    public void updateText(GameState gs) {
        timerText.text = TimeSpan.FromSeconds(gs.elapsedTime).ToString(@"hh\:mm\:ss");
        floorNum.text = string.Concat("floor: ", gs.floor.ToString().PadLeft(2, '0'));
    }

    // I have given up on writing readable code. Good luck!
    public void toggleAll(bool isRunning) => pauseButton.interactable = upper2.interactable = timerButton.interactable = upper1.interactable = floorButton.interactable = isRunning;
    public void togglePauseButton(bool isRunning) => pauseButton.interactable = isRunning;
}
