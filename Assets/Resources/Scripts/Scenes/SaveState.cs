using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveState{
    private int fs = 0;
    private double ts = 0;
    private BoardState bs;
    private List<EnemyState> es;
    private PlayerState ps;
    private bool saveExists = false;

    public void init() {
        if (!Directory.Exists("Saves")) return;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create("Saves/save.binary");
        if (saveFile.Length == 0) {
            saveFile.Close();
            return;
        }

        SaveState loadedState = (SaveState)formatter.Deserialize(saveFile);
        saveFile.Close();

        fs = loadedState.fs;
        ts = loadedState.ts;
        bs = loadedState.bs;
        es = loadedState.es;
        ps = loadedState.ps;
        saveExists = true;
    }

    public void saveGame(int currFloor, double elapsedTime, Board board, List<Enemy> currEnemies, Player player) {
        fs = currFloor;
        ts = elapsedTime;
        bs = board.getState();
        ps = player.getState();
        es = new List<EnemyState>();
        foreach(Enemy e in currEnemies) es.Add(e.getState());

        if (!Directory.Exists("Saves")) Directory.CreateDirectory("Saves");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create("Saves/save.binary");
        formatter.Serialize(saveFile, this);
        saveFile.Close();

        saveExists = true;
    }
    public void loadGame(ref int currFloor, ref double elapsedTime, ref Board board, ref List<Enemy> currEnemies, ref Player player) {
        currFloor = fs;
        elapsedTime = ts;
        if(bs != null) board.setState(bs);
        if (ps != null) player.setState(ps);
        if (es != null) {
            currEnemies = new List<Enemy>();
            foreach (EnemyState e in es) {
                Enemy temp = Enemy.Create(e.prefab, e.number, e.maxHealth, e.damage);
                temp.setState(e);
                currEnemies.Add(temp);
            }
        }
    }
    public bool doesSaveExist() { return saveExists; }
}
