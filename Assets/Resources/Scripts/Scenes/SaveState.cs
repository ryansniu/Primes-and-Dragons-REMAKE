using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveState{
    private readonly string SAVE_DATA = "/sData.binary";
    private GameState gs;
    private List<EnemyState> es;
    private BoardState bs;
    private PlayerState ps;

    public void saveCurrData() {
        gs = GameController.Instance.getState();
        bs = Board.Instance.getState();
        ps = Player.Instance.getState();
        es = new List<EnemyState>();
        foreach(Enemy e in GameController.Instance.getCurrEnemies()) es.Add(e.getState());
        GameData.writeFile(SAVE_DATA, this);
        PlayerPrefs.SetInt("SaveExists", 1);
    }

    public void loadDataIntoGame() {
        SaveState loadedState = GameData.readFile(SAVE_DATA) as SaveState;
        GameController.Instance.setState(loadedState.gs);
        Board.Instance.setState(loadedState.bs);
        Player.Instance.setState(loadedState.ps);
        foreach (EnemyState e in loadedState.es) {
            Enemy temp = Enemy.Create(e.prefab, e.number, e.maxHealth, e.damage, e.spriteName);
            temp.setState(e);
            GameController.Instance.loadEnemy(temp);
        }
    }
}
