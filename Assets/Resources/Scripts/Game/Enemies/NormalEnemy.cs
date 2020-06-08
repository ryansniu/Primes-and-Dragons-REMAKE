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
        NormalEnemy e = Create("Normal Enemy", num, hp, atk).GetComponent<NormalEnemy>();
        e.initSkills(floor, numEnemies);
        e.setSprite(sprite);
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

    public override IEnumerator Attack(Player p, Board b) {
        foreach (Vector2Int index in skillIndicies) yield return StartCoroutine(activateSkill(index, p , b));
        yield return StartCoroutine(base.Attack(p, b));
    }

    private IEnumerator activateSkill(Vector2Int index, Player p, Board b) {  // UGGO but what can you do
        if(index.x == 1) {  // Easy Skills
            switch (index.y) {
                // Floor < 15
                case 0: yield return StartCoroutine(clearPattern(b, BoardPattern.RANDOM, new Vector2Int(3, 11))); break;
                case 1: yield return StartCoroutine(clearPattern(b, BoardPattern.ROW)); break;
                case 2: yield return StartCoroutine(clearPattern(b, BoardPattern.COLUMN)); break;
                case 3: yield return StartCoroutine(dummySkill()); break;
                // Floor < 30
                case 4: yield return StartCoroutine(shuffleBoard(b)); break;
                case 5: yield return StartCoroutine(setPattern(b, BoardPattern.RANDOM, ORB_VALUE.EMPTY, new Vector2Int(3, 11))); break;
                case 6: yield return StartCoroutine(dummySkill()); break;
                case 7: yield return StartCoroutine(dummySkill()); break;
                // Floor < 45
                case 8: yield return StartCoroutine(setPattern(b, BoardPattern.RANDOM, ORB_VALUE.NULLIFY, new Vector2Int(1, 2))); break;
                case 9: yield return StartCoroutine(setPattern(b, BoardPattern.RANDOM, ORB_VALUE.STOP, new Vector2Int(1, 2))); break;
                case 10: yield return StartCoroutine(decrementPattern(b, BoardPattern.RANDOM, new Vector2Int(3, 6))); break;
            }
        } else if(index.x == 2) {  // Medium Skills
            switch (index.y) {
                // Floor < 15
                case 0: yield return StartCoroutine(dummySkill()); break;
                case 1: yield return StartCoroutine(clearPattern(b, BoardPattern.PLUS)); break;
                case 2: yield return StartCoroutine(clearPattern(b, BoardPattern.CROSS)); break;
                case 3: yield return StartCoroutine(clearPattern(b, BoardPattern.BOX)); break;
                case 4: yield return StartCoroutine(clearPattern(b, BoardPattern.SPIRAL)); break;
                // Floor < 30
                case 5: yield return StartCoroutine(setPattern(b, BoardPattern.ROW, ORB_VALUE.EMPTY)); break;
                case 6: yield return StartCoroutine(setPattern(b, BoardPattern.COLUMN, ORB_VALUE.EMPTY)); break;
                // Floor < 45
                case 7: yield return StartCoroutine(decrementPattern(b, BoardPattern.ROW)); break;
                case 8: yield return StartCoroutine(decrementPattern(b, BoardPattern.COLUMN)); break;
                case 9: yield return StartCoroutine(decrementPattern(b, BoardPattern.PLUS)); break;
                case 10: yield return StartCoroutine(decrementPattern(b, BoardPattern.CROSS)); break;
                case 11: yield return StartCoroutine(decrementPattern(b, BoardPattern.BOX)); break;
                case 12: yield return StartCoroutine(decrementPattern(b, BoardPattern.SPIRAL)); break;
                case 13: yield return StartCoroutine(setPattern(b, BoardPattern.RANDOM, ORB_VALUE.POISON, new Vector2Int(3, 11))); break;
                case 14: yield return StartCoroutine(setPattern(b, BoardPattern.RANDOM, ORB_VALUE.STOP, new Vector2Int(3, 6))); break;
                case 15: yield return StartCoroutine(dummySkill()); break;
            }
        }
        else if (index.x == 3) {  // Hard Skills
            switch (index.y) {
                // Floor < 15
                case 0: yield return StartCoroutine(dummySkill()); break;
                // Floor < 30
                case 1: yield return StartCoroutine(setPattern(b, BoardPattern.BOX, ORB_VALUE.STOP)); break;
                case 2: yield return StartCoroutine(setPattern(b, BoardPattern.PLUS, ORB_VALUE.POISON)); break;
                case 3: yield return StartCoroutine(dmgReflectSelf()); break;
                case 4: yield return StartCoroutine(dummySkill()); break;
                // Floor < 45
                case 5: yield return StartCoroutine(setPattern(b, BoardPattern.ROW, ORB_VALUE.STOP)); break;
                case 6: yield return StartCoroutine(setPattern(b, BoardPattern.COLUMN, ORB_VALUE.STOP)); break;
                case 7: yield return StartCoroutine(togglePlayerTimer(p)); break;
            }
        }
        yield return null;
    }

    private IEnumerator dummySkill() {  //can add player/board if needed
        //yield return StartCoroutine(useSkill("Testing!", 0.25f));
        yield return new WaitForSeconds(0.25f);
    }
    private IEnumerator healSelf(Player p, Board b) {
        //yield return StartCoroutine(useSkill("Heal", 0.25f));
        targetedAnimation(true);
        yield return StartCoroutine(takeDMG(50, p, b));
    }
    private IEnumerator dmgMitiSelf() {
        //yield return StartCoroutine(useSkill("DMG Miti", 0.25f));
        setBuff(currState.currTurn % 2 == 0 ? EnemyBuffs.DMG_MITI_50 : EnemyBuffs.NONE);
        yield return null;
    }
    private IEnumerator dmgReflectSelf() {
        //yield return StartCoroutine(useSkill("DMG Reflect", 0.25f));
        setBuff(currState.currTurn % 2 == 0 ? EnemyBuffs.DMG_REFLECT : EnemyBuffs.NONE);
        yield return null;
    }
    private IEnumerator togglePlayerTimer(Player p) {
        //yield return StartCoroutine(useSkill("Timer!", 0.25f));
        if (currState.currTurn % 2 == 0) p.setDOT(-10);
        yield return null;
    }


    private IEnumerator clearPattern(Board b, BoardPattern bp, Vector2Int randRange = new Vector2Int()) {
        List<Vector2Int> toClear = new List<Vector2Int>();
        Vector2Int pivot = new Vector2Int(RNG.Next(Board.COLUMNS), RNG.Next(Board.ROWS));
        string skillName = "clear ";
        switch (bp) {
            case BoardPattern.ROW:
                skillName +="row";
                for (int i = 0; i < 6; i++) toClear.Add(new Vector2Int(i, pivot.y));
                break;
            case BoardPattern.COLUMN:
                skillName += "column";
                for (int i = 0; i < 5; i++) toClear.Add(new Vector2Int(pivot.y, i));
                break;
            case BoardPattern.PLUS:
                skillName += "+";
                for (int i = 0; i < 6; i++) toClear.Add(new Vector2Int(i, pivot.y));
                for (int i = 0; i < 5; i++) if (i != pivot.y) toClear.Add(new Vector2Int(pivot.y, i));
                break;
            case BoardPattern.CROSS:
                skillName += "x";
                break;
            case BoardPattern.BOX:
                skillName += "box";
                for (int i = 0; i < 5; i++) toClear.Add(new Vector2Int(0, i));  // PURGE DUPLICATES
                for (int i = 0; i < 6; i++) toClear.Add(new Vector2Int(i, 0));
                for (int i = 0; i < 5; i++) toClear.Add(new Vector2Int(5, i));
                for (int i = 0; i < 6; i++) toClear.Add(new Vector2Int(i, 4));
                break;
            case BoardPattern.SPIRAL:
                skillName += "spiral";
                break;
            case BoardPattern.RANDOM:
                skillName += "RNG";
                int numRmv = RNG.Next(randRange.x, randRange.y);
                while(toClear.Count < numRmv) {
                    int rand = RNG.Next(Board.COLUMNS * Board.ROWS);
                    Vector2Int temp = new Vector2Int(rand / 5, rand % 5);
                    if(!toClear.Contains(temp)) toClear.Add(temp);
                }
                break;
        }
        //yield return StartCoroutine(useSkill(skillName, 0.05f * toClear.Count + Board.DISAPPEAR_DURATION));
        yield return StartCoroutine(b.removeOrbsInOrder(toClear));
    }
    private IEnumerator decrementPattern(Board b, BoardPattern bp, Vector2Int randRange = new Vector2Int()) {
        List<Vector2Int> toDecrement = new List<Vector2Int>();
        Vector2Int pivot = new Vector2Int(RNG.Next(Board.COLUMNS), RNG.Next(Board.ROWS));
        string skillName = "decrement ";
        float delay = 0.1f;
        switch (bp) {
            case BoardPattern.ROW:
                skillName += "row";
                for (int i = 0; i < 6; i++) toDecrement.Add(new Vector2Int(i, pivot.y));
                break;
            case BoardPattern.COLUMN:
                skillName += "col";
                for (int i = 0; i < 5; i++) toDecrement.Add(new Vector2Int(pivot.x, i));
                break;
            case BoardPattern.PLUS:
                skillName += "+";
                for (int i = 0; i < 6; i++) toDecrement.Add(new Vector2Int(i, pivot.y));
                for (int i = 0; i < 5; i++) if(i != pivot.y) toDecrement.Add(new Vector2Int(pivot.x, i));
                break;
            case BoardPattern.CROSS:
                skillName += "x";
                break;
            case BoardPattern.BOX:
                skillName += "box";
                for (int i = 0; i < 5; i++) toDecrement.Add(new Vector2Int(0, i));
                for (int i = 0; i < 6; i++) toDecrement.Add(new Vector2Int(i, 0));
                for (int i = 0; i < 5; i++) toDecrement.Add(new Vector2Int(5, i));
                for (int i = 0; i < 6; i++) toDecrement.Add(new Vector2Int(i, 4));
                break;
            case BoardPattern.SPIRAL:
                skillName += "spiral";
                break;
            case BoardPattern.RANDOM:
                skillName += "RNG";
                int numRmv = RNG.Next(randRange.x, randRange.y);
                while (toDecrement.Count < numRmv) {
                    int rand = RNG.Next(Board.COLUMNS * Board.ROWS);
                    Vector2Int temp = new Vector2Int(rand / 5, rand % 5);
                    if (!toDecrement.Contains(temp)) toDecrement.Add(temp);
                }
                break;
        }
        //yield return StartCoroutine(useSkill(skillName, toDecrement.Count * delay));
        yield return StartCoroutine(b.incrementOrbsInOrder(toDecrement, delay, -1));
    }
    private IEnumerator setPattern(Board b, BoardPattern bp, ORB_VALUE val, Vector2Int randRange = new Vector2Int()) {
        List<Vector2Int> toSet = new List<Vector2Int>();
        Vector2Int pivot = new Vector2Int(RNG.Next(Board.COLUMNS), RNG.Next(Board.ROWS));
        string skillName = "set ";
        float delay = 0.1f;
        switch (bp) {
            case BoardPattern.ROW:
                skillName += "row";
                for (int i = 0; i < 6; i++) toSet.Add(new Vector2Int(i, pivot.y));
                break;
            case BoardPattern.COLUMN:
                skillName += "col";
                for (int i = 0; i < 5; i++) toSet.Add(new Vector2Int(pivot.x, i));
                break;
            case BoardPattern.PLUS:
                skillName += "+";
                for (int i = 0; i < 6; i++) toSet.Add(new Vector2Int(i, pivot.y));
                for (int i = 0; i < 5; i++) if (i != pivot.y) toSet.Add(new Vector2Int(pivot.x, i));
                break;
            case BoardPattern.CROSS:
                skillName += "x";
                break;
            case BoardPattern.BOX:
                skillName += "box";
                for(int i = 0; i < 5; i++) toSet.Add(new Vector2Int(0, i));
                for (int i = 0; i < 6; i++) toSet.Add(new Vector2Int(i, 0));
                for (int i = 0; i < 5; i++) toSet.Add(new Vector2Int(5, i));
                for (int i = 0; i < 6; i++) toSet.Add(new Vector2Int(i, 4));
                break;
            case BoardPattern.SPIRAL:
                skillName += "spiral";
                break;
            case BoardPattern.RANDOM:
                skillName += "RNG";
                int numRmv = RNG.Next(randRange.x, randRange.y);
                while (toSet.Count < numRmv) {
                    int rand = RNG.Next(Board.COLUMNS * Board.ROWS);
                    Vector2Int temp = new Vector2Int(rand / 5, rand % 5);
                    if (!toSet.Contains(temp)) toSet.Add(temp);
                }
                break;
        }
        skillName += " w/ " + val;
        //yield return StartCoroutine(useSkill(skillName, toSet.Count * delay));
        yield return StartCoroutine(b.setOrbsInOrder(toSet, delay, val));
    }
    private IEnumerator shuffleBoard(Board b) {
        int numShuffles = 5;
        float duration = 0.5f;
        WaitForSeconds delay = new WaitForSeconds(duration / numShuffles);

        //yield return StartCoroutine(useSkill("Shuffle!", duration));
        for(int i = 0; i < numShuffles; i++) {
            b.shuffleBoard();
            yield return delay;
        }
    }
}