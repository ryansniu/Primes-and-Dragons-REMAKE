using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveState{
    private int fs = 0;
    private Board bs;
    private List<Enemy> es;
    private Player ps;

    public void init() {
        if (!Directory.Exists("Saves")) return;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create("Saves/save.binary");
        if (saveFile.Length == 0) return;

        SaveState loadedState = (SaveState)formatter.Deserialize(saveFile);
        saveFile.Close();

        fs = loadedState.fs;
        bs = loadedState.bs;
        es = loadedState.es;
        ps = loadedState.ps;
    }

    public void saveGame(int currFloor, Board board, List<Enemy> currEnemies, Player player) {
        fs = currFloor;
        bs = board;
        es = currEnemies;
        ps = player;

        if (!Directory.Exists("Saves")) Directory.CreateDirectory("Saves");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create("Saves/save.binary");
        formatter.Serialize(saveFile, this);
        saveFile.Close();
    }
    public void loadGame(ref int currFloor, ref Board board, ref List<Enemy> currEnemies, ref Player player) {
        currFloor = fs;
        if(bs != null) board = bs;
        if(es != null) currEnemies = es;
        if(ps != null) player = ps;
    }
}
