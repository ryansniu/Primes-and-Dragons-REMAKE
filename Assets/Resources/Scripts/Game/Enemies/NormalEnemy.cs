using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : Enemy {
    private int level = 0;

    private static readonly int[] enemiesLvl1 = { 3, 4, 5, 6, 8, 9, 10, 12, 20, 25, 50 };
    private static readonly string[] s1 = { "tree", "burger", "fire1", "dragon1", "dragon2", "potato1", "tv1" };
    private static readonly int[] enemiesLvl2 = { 5, 6, 7, 8, 9, 10, 14, 15, 20, 22, 24, 25, 30, 32, 40, 50 };
    private static readonly string[] s2 = { "tree", "burger", "taco", "rice", "fire1", "fire2", "dragon2", "dragon3", "dragon4", "potato2", "tv1", "tv2" };
    private static readonly int[] enemiesLvl3 = { 7, 10, 11, 12, 14, 15, 16, 18, 21, 22, 26, 27, 30, 32, 35, 40, 45, 60 };
    private static readonly string[] s3 = { "burger", "taco", "rice", "fire2", "fire3", "dragon4", "dragon5", "potato3", "tv2", "tv3" };
    public static Enemy Create(int floor, int numEnemies, int lvl) {
        int num = 0, hp = 0, atk = 0;
        string sprite = "dummy";
        switch (lvl) {
            case 1:
                num = enemiesLvl1[RNG.Next(0, enemiesLvl1.Length)];
                sprite = s1[RNG.Next(0, s1.Length)];
                hp = (100 + floor * 25);
                atk = (100 + floor * 6) / numEnemies;
                break;
            case 2:
                num = enemiesLvl2[RNG.Next(0, enemiesLvl2.Length)];
                sprite = s2[RNG.Next(0, s2.Length)];
                hp = (200 + floor * 10);
                atk = (200 + floor * 3) / numEnemies;
                break;
            case 3:
                num = enemiesLvl3[RNG.Next(0, enemiesLvl3.Length)];
                sprite = s3[RNG.Next(0, s3.Length)];
                hp = (300 + floor * 6);
                atk = (300 + floor * 2) / numEnemies;
                break;
            default:
                break;
        }
        NormalEnemy e = Create("Normal Enemy", num, hp, atk).GetComponent<NormalEnemy>();
        e.level = lvl;
        e.setSprite(sprite);
        return e;
    }

    public override IEnumerator Attack(Player p, Board b) {
        yield return StartCoroutine(useSkill("Testing!", 1f));
        yield return new WaitForSeconds(1f);  // placeholder
        yield return StartCoroutine(base.Attack(p, b));
    }

    private IEnumerator replaceOrbs(Func<Orb, Orb> condition) {
        yield return null;
    }
    private IEnumerator clearOrbs(Func<Orb, bool> condition) {
        yield return null;
    }
    private IEnumerator heal(Enemy e, int heals) {
        yield return null;
    }
    private IEnumerator activateBuff(EnemyStatus status) {
        yield return null;
    }
    private IEnumerator activateTimer(int dmg) {
        yield return null;
    }
}
