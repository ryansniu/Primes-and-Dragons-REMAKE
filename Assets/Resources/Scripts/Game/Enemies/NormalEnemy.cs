using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : Enemy {
    private static readonly int[] enemiesLvl1 = { 3, 4, 5, 6, 8, 9, 10, 12, 20, 25, 50 };
    private static readonly string[] s1 = { "tree", "burger", "fire1", "dragon1", "dragon2", "potato1", "tv1" };
    private static readonly int[] enemiesLvl2 = { 5, 6, 7, 8, 9, 10, 14, 15, 20, 22, 24, 25, 30, 32, 40, 50 };
    private static readonly string[] s2 = { "tree", "burger", "taco", "rice", "fire1", "fire2", "dragon2", "dragon3", "dragon4", "potato2", "tv1", "tv2" };
    private static readonly int[] enemiesLvl3 = { 7, 10, 11, 12, 14, 15, 16, 18, 21, 22, 26, 27, 30, 32, 35, 40, 45, 60 };
    private static readonly string[] s3 = { "burger", "taco", "rice", "fire2", "fire3", "dragon4", "dragon5", "potato3", "tv2", "tv3" };

    private List<Vector2Int> skillIndicies = new List<Vector2Int>();  // difficulty, floor level, skill number

    public static Enemy Create(int floor, int numEnemies) {
        int num = 0, hp = 0, atk = 0;
        string sprite = "dummy";
        if(floor < 15) {
            num = enemiesLvl1[RNG.Next(enemiesLvl1.Length)];
            hp = (100 + floor * 25);
            atk = (100 + floor * 6) / numEnemies;
            sprite = s1[RNG.Next(s1.Length)];
        } else if (floor < 30) {
            num = enemiesLvl2[RNG.Next(enemiesLvl2.Length)];
            hp = (200 + floor * 10);
            atk = (200 + floor * 3) / numEnemies;
            sprite = s2[RNG.Next(s2.Length)];
        } else if (floor < 45) {
            num = enemiesLvl3[RNG.Next(enemiesLvl3.Length)];
            hp = (300 + floor * 6);
            atk = (300 + floor * 2) / numEnemies;
            sprite = s3[RNG.Next(s3.Length)];
        }
        else {
            // Do nothing.
        }
        NormalEnemy e = Create("Normal Enemy", num, hp, atk, sprite).GetComponent<NormalEnemy>();
        e.initSkills(floor, numEnemies);
        return e;
    }
    private void initSkills(int floor, int numEnemies) {
        int easySkillRange = 4, medSkillRange = 5, hardSkillRange = 1;
        if (floor >= 15) {
            easySkillRange += 4;
            medSkillRange += 2;
            hardSkillRange += 4;
        }
        if (floor >= 30) {
            easySkillRange += 3;
            medSkillRange += 9;
            hardSkillRange += 3;
        }

        if (numEnemies == 3) {
            if (RNG.Next(2) == 0) skillIndicies.Add(new Vector2Int(1, RNG.Next(easySkillRange)));
        } else if(numEnemies == 2) {
            skillIndicies.Add(new Vector2Int(1, RNG.Next(easySkillRange)));
            if (RNG.Next(2) == 0) skillIndicies.Add(new Vector2Int(2, RNG.Next(medSkillRange)));
        } else if(numEnemies == 1) {
            if (RNG.Next(2) == 0) skillIndicies.Add(new Vector2Int(1, RNG.Next(easySkillRange)));
            skillIndicies.Add(new Vector2Int(2, RNG.Next(medSkillRange)));
            skillIndicies.Add(new Vector2Int(3, RNG.Next(hardSkillRange)));
        }
    }


    private IEnumerator activateSkill(Vector2Int index) {  // UGGO but what can you do
        if(index.x == 1) {  // Easy Skills
            switch (index.y) {
                default: break;
            }
        } else if(index.x == 2) {  // Medium Skills
            switch (index.y) {
                default: break;
            }
        }
        else if (index.x == 3) {  // Hard Skills
            switch (index.y) {
                default: break;
            }
        }
        yield return null;
    }
}