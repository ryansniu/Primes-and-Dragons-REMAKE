using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveState : MonoBehaviour {
    public int fs;
    public Board bs;
    public List<Enemy> es;
    public Player ps;
    public void saveGame(int currFloor, Board board, List<Enemy> currEnemies, Player player) {
        fs = currFloor;
        bs = board;
        es = currEnemies;
        ps = player;
    }
    public void loadGame(ref int currFloor, ref Board board, ref List<Enemy> currEnemies, ref Player player) {
        currFloor = fs;
        board = bs;
        currEnemies = es;
        player = ps;
    }
}
