using System.Collections.Generic;
using UnityEngine;

public class SaveStateController : MonoBehaviour {
    public static SaveStateController Instance;

    [System.Serializable]
    private class SaveState {
        public static readonly string SAVE_DATA = "/sData.binary";
        public GameState gs;
        public BoardState bs;
        public PlayerState ps;
    }
    private SaveState currState = new SaveState();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        PlayerPrefs.SetInt("SaveExists", GameData.readFile(SaveState.SAVE_DATA) as SaveState == null ? 0 : 1);
    }

    public void saveCurrData() {
        currState.gs = GameController.Instance.getState();
        currState.bs = Board.Instance.getState();
        currState.ps = Player.Instance.getState();
        GameData.writeFile(SaveState.SAVE_DATA, currState);
        PlayerPrefs.SetInt("SaveExists", 1);
    }

    public void loadDataIntoGame() {
        SaveState loadedState = GameData.readFile(SaveState.SAVE_DATA) as SaveState;
        GameController.Instance.setState(loadedState.gs);
        Board.Instance.setState(loadedState.bs);
        Player.Instance.setState(loadedState.ps);
    }
}
