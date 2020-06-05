using System;
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
            num = enemiesLvl1[RNG.Next(0, enemiesLvl1.Length)];
            hp = (100 + floor * 25);
            atk = (100 + floor * 6) / numEnemies;
            sprite = s1[RNG.Next(0, s1.Length)];
        } else if (floor < 30) {
            num = enemiesLvl2[RNG.Next(0, enemiesLvl2.Length)];
            hp = (200 + floor * 10);
            atk = (200 + floor * 3) / numEnemies;
            sprite = s2[RNG.Next(0, s2.Length)];
        } else if (floor < 45) {
            num = enemiesLvl3[RNG.Next(0, enemiesLvl3.Length)];
            hp = (300 + floor * 6);
            atk = (300 + floor * 2) / numEnemies;
            sprite = s3[RNG.Next(0, s3.Length)];
        }
        else {
            // Do nothing.
        }
        NormalEnemy e = Create("Normal Enemy", num, hp, atk).GetComponent<NormalEnemy>();
        e.initSkills(floor, numEnemies);
        e.setSprite(sprite);
        return e;
    }
    private void initSkills(int floor, int numEnemies) {
        if (numEnemies == 3) {
            int skillRange = 4;
            if (floor >= 15) skillRange += 3;
            if (floor >= 30) skillRange += 3;

            if (UnityEngine.Random.value <= 0.5f) skillIndicies.Add(new Vector2Int(1, UnityEngine.Random.Range(0, skillRange)));
        } else if(numEnemies == 2) {
            int skillRange = 3;
            if (floor >= 15) skillRange += 3;
            if (floor >= 30) skillRange += 3;
            skillIndicies.Add(new Vector2Int(1, UnityEngine.Random.Range(0, skillRange)));
            if (UnityEngine.Random.value <= 0.5f) skillIndicies.Add(new Vector2Int(2, UnityEngine.Random.Range(0, skillRange)));
        } else if(numEnemies == 1) {
            int skillRange = 3;
            if (floor >= 15) skillRange += 3;
            if (floor >= 30) skillRange += 3;

            if (UnityEngine.Random.value <= 0.5f) skillIndicies.Add(new Vector2Int(1, UnityEngine.Random.Range(0, skillRange)));
            skillIndicies.Add(new Vector2Int(2, UnityEngine.Random.Range(0, skillRange)));
            skillIndicies.Add(new Vector2Int(3, UnityEngine.Random.Range(0, skillRange)));
        }
    }

    public override IEnumerator Attack(Player p, Board b) {
        foreach (Vector2Int index in skillIndicies) yield return StartCoroutine(activateSkill(index, p , b));
        yield return StartCoroutine(base.Attack(p, b));
    }

    private IEnumerator activateSkill(Vector2Int index, Player p, Board b) {  // UGGO but what can you do
        if(index.x == 1) {  // Easy Skills
            switch (index.y) {
                case 0: yield return StartCoroutine(dummySkill()); break;
                case 1: yield return StartCoroutine(dummySkill()); break;
                case 2: yield return StartCoroutine(dummySkill()); break;
                case 3: yield return StartCoroutine(dummySkill()); break;
                case 4: yield return StartCoroutine(dummySkill()); break;
                case 5: yield return StartCoroutine(dummySkill()); break;
                case 6: yield return StartCoroutine(dummySkill()); break;
                case 7: yield return StartCoroutine(shuffleBoard(b)); break;
                case 8: yield return StartCoroutine(dummySkill()); break;
                case 9: yield return StartCoroutine(dummySkill()); break;
            }
        } else if(index.x == 2) {  // Medium Skills
            switch (index.y) {
                case 0: yield return StartCoroutine(dummySkill()); break;
                case 1: yield return StartCoroutine(dummySkill()); break;
                case 2: yield return StartCoroutine(healSelf(p, b)); break;
                case 3: yield return StartCoroutine(dummySkill()); break;
                case 4: yield return StartCoroutine(dummySkill()); break;
                case 5: yield return StartCoroutine(dummySkill()); break;
                case 6: yield return StartCoroutine(dummySkill()); break;
                case 7: yield return StartCoroutine(dummySkill()); break;
                case 8: yield return StartCoroutine(dummySkill()); break;
            }
        }
        else if (index.x == 3) {  // Hard Skills
            switch (index.y) {
                case 0: yield return StartCoroutine(dummySkill()); break;
                case 1: yield return StartCoroutine(dummySkill()); break;
                case 2: yield return StartCoroutine(dmgMitiSelf()); break;
                case 3: yield return StartCoroutine(dummySkill()); break;
                case 4: yield return StartCoroutine(dmgReflectSelf()); break;
                case 5: yield return StartCoroutine(dummySkill()); break;
                case 6: yield return StartCoroutine(dummySkill()); break;
                case 7: yield return StartCoroutine(togglePlayerTimer(p)); break;
                case 8: yield return StartCoroutine(dummySkill()); break;
            }
        }
        yield return null;
    }

    private IEnumerator dummySkill() {  //can add player/board if needed
        yield return StartCoroutine(useSkill("Testing!", 0.25f));
        yield return new WaitForSeconds(0.25f);
    }
    private IEnumerator healSelf(Player p, Board b) {
        yield return StartCoroutine(useSkill("Heal", 0.25f));
        toggleStatus(EnemyStatus.HEAL, true);
        yield return StartCoroutine(takeDMG(50, p, b));
        toggleStatus(EnemyStatus.HEAL, false);
    }
    private IEnumerator dmgMitiSelf() {
        yield return StartCoroutine(useSkill("DMG Miti", 0.25f));
        toggleStatus(EnemyStatus.DMG_MITI_50, currState.turnCount % 2 == 0);
    }
    private IEnumerator dmgReflectSelf() {
        yield return StartCoroutine(useSkill("DMG Reflect", 0.25f));
        toggleStatus(EnemyStatus.DMG_REFLECT, currState.turnCount % 2 == 0);
    }
    private IEnumerator togglePlayerTimer(Player p) {
        yield return StartCoroutine(useSkill("Timer!", 0.25f));
        if (currState.turnCount % 2 == 0) p.setDOT(-10);
    }


    private IEnumerator clearPattern(Board b, BoardPattern bp, Vector2Int pivot) {
        List<Vector2Int> toClear = new List<Vector2Int>();
        switch (bp) {
            case BoardPattern.ROW:
                int row = pivot.y;
                break;
            case BoardPattern.COLUMN:
                int col = pivot.y;
                break;
            case BoardPattern.PLUS:
                break;
            case BoardPattern.CROSS:
                break;
            case BoardPattern.BOX:
                break;
            case BoardPattern.SPIRAL:
                break;
            case BoardPattern.RANDOM:
                break;
        }
        yield return StartCoroutine(b.removeOrbsInOrder(toClear));
    }
    private IEnumerator decrementPattern(Board b, BoardPattern bp) {
        List<Vector2Int> toDecrement = new List<Vector2Int>();
        switch (bp) {
            case BoardPattern.ROW:
                break;
            case BoardPattern.COLUMN:
                break;
            case BoardPattern.PLUS:
                break;
            case BoardPattern.CROSS:
                break;
            case BoardPattern.BOX:
                break;
            case BoardPattern.SPIRAL:
                break;
            case BoardPattern.RANDOM:
                break;
        }
        yield return StartCoroutine(b.removeOrbsInOrder(toDecrement));
    }
    private IEnumerator setPattern(Board b, BoardPattern bp) {
        List<Vector2Int> toSet = new List<Vector2Int>();
        switch (bp) {
            case BoardPattern.ROW:
                break;
            case BoardPattern.COLUMN:
                break;
            case BoardPattern.PLUS:
                break;
            case BoardPattern.CROSS:
                break;
            case BoardPattern.BOX:
                break;
            case BoardPattern.SPIRAL:
                break;
            case BoardPattern.RANDOM:
                break;
        }
        yield return StartCoroutine(b.removeOrbsInOrder(toSet));
    }
    private IEnumerator shuffleBoard(Board b) {
        int numShuffles = 5;
        float duration = 0.5f;
        WaitForSeconds delay = new WaitForSeconds(duration / numShuffles);

        yield return StartCoroutine(useSkill("Shuffle!", duration));
        for(int i = 0; i < numShuffles; i++) {
            b.shuffleBoard();
            yield return delay;
        }
    }
}