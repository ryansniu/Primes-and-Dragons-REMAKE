using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner {
    private System.Random rng = new System.Random(DateTime.Now.Millisecond);
    public List<Enemy> getEnemies(int floor) {
        List<Enemy> enemies = new List<Enemy>();

        if (floor == 0) {
            enemies.Add(TutorialEnemy.Create());
        }
        else if (floor < 15) {
            int len = rng.Next(1, 3);
            for (int i = 0; i < len; i++) enemies.Add(NormalEnemy.Create(floor, len));
        }
        else if (floor == 15) {
            enemies.Add(MiniBoss.Create(16));
            enemies.Add(MiniBoss.Create(25));
            enemies.Add(MiniBoss.Create(36));
        }
        else if (floor < 30) {
            int len = rng.Next(1, 4);
            for (int i = 0; i < len; i++) enemies.Add(NormalEnemy.Create(floor, len));
        }
        else if (floor == 30) {
            enemies.Add(MiniBoss.Create(26));
            enemies.Add(MiniBoss.Create(27));
            enemies.Add(MiniBoss.Create(28));
        }
        else if (floor < 45) {
            int len = rng.Next(2, 4);
            for (int i = 0; i < len; i++) enemies.Add(NormalEnemy.Create(floor, len));
        }
        else if (floor == 45) {
            enemies.Add(MiniBoss.Create(11));
            enemies.Add(MiniBoss.Create(13));
        }
        else if (floor == 46) {
            enemies.Add(MiniBoss.Create(17));
            enemies.Add(MiniBoss.Create(19));
        }
        else if (floor == 47) {
            enemies.Add(MiniBoss.Create(29));
            enemies.Add(MiniBoss.Create(23));
        }
        else if (floor == 48) {
            enemies.Add(MiniBoss.Create(15));
            enemies.Add(MiniBoss.Create(21));
            enemies.Add(MiniBoss.Create(35));
        }
        else if (floor == 49) {
            enemies.Add(MiniBoss.Create(3));
            enemies.Add(MiniBoss.Create(6));
            enemies.Add(MiniBoss.Create(9));
        }
        else if (floor == 50) {
            //TODO
            // phase 1
            // phase 2
            // phase 3
        }
        return enemies;
    }
}
