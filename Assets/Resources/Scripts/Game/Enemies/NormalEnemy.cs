using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalEnemy : Enemy {
    private static readonly int[] enemiesLvl1 = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 15, 20, 24, 25, 40, 50, 60, 80 };
    private static readonly string[] s1 = { "tree", "burger", "fire1", "dragon1", "dragon2", "potato1", "tv1" };
    private static readonly int[] enemiesLvl2 = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 20, 22, 24, 25, 30, 32, 40, 50, 60, 80 };
    private static readonly string[] s2 = { "tree", "burger", "taco", "rice", "fire1", "fire2", "dragon2", "dragon3", "dragon4", "potato2", "tv1", "tv2" };
    private static readonly int[] enemiesLvl3 = { 7, 9, 12, 14, 15, 16, 18, 21, 22, 24, 30, 32, 33, 34, 35, 38, 39, 40, 45, 55, 60, 65, 70, 77, 75, 90, 91 };
    private static readonly string[] s3 = { "burger", "taco", "rice", "fire2", "fire3", "dragon4", "dragon5", "potato3", "tv2", "tv3" };
    private Func<List<Vector2Int>> getOneLine, getFiveLines, getRandomPattern;

    public static NormalEnemy Create(int floor, int numEnemies) {
        int num = 0, hp = 0, atk = 0;
        string sprite = "dummy";
        if (floor < 15) {
            num = enemiesLvl1[RNG.Next(enemiesLvl1.Length)];
            hp = (100 + floor * 25);
            atk = (100 + floor * 6) / numEnemies;
            sprite = s1[RNG.Next(s1.Length)];
        }
        else if (floor < 30) {
            num = enemiesLvl2[RNG.Next(enemiesLvl2.Length)];
            hp = (200 + floor * 10);
            atk = (200 + floor * 3) / numEnemies;
            sprite = s2[RNG.Next(s2.Length)];
        }
        else if (floor < 45) {
            num = enemiesLvl3[RNG.Next(enemiesLvl3.Length)];
            hp = (300 + floor * 6);
            atk = (300 + floor * 2) / numEnemies;
            sprite = s3[RNG.Next(s3.Length)];
        }
        return Create("Normal Enemy", num, hp, atk, sprite).GetComponent<NormalEnemy>();
    }
    public void setSkills(List<int> easySkills, List<int> medSkills, List<int> hardSkills) {
        currState.easySkills = easySkills;
        currState.medSkills = medSkills;
        currState.hardSkills = hardSkills;
        addAllSkills();
    }
    private void initHelperSkills() {
        if (getOneLine == null) {
            getOneLine = () => {
                System.Random rand = new System.Random(getRandomSeedByTurn());
                List<Vector2Int> result = new List<Vector2Int>();
                bool isRow = rand.Next(2) == 0;
                int lineIndex = rand.Next(isRow ? Board.ROWS : Board.COLUMNS);
                if (isRow) for (int i = 0; i < Board.COLUMNS; i++) result.Add(new Vector2Int(i, lineIndex));
                else for (int i = Board.ROWS - 1; i >= 0; i--) result.Add(new Vector2Int(lineIndex, i));
                return result;
            };
        }
        if(getFiveLines == null) {
            getFiveLines = () => {
                System.Random rand = new System.Random(getRandomSeedByTurn());
                List<Vector2Int> result = new List<Vector2Int>();
                List<int> remainingLines = new List<int>();
                for (int i = 0; i < Board.ROWS + Board.COLUMNS; i++) remainingLines.Add(i);
                for (int i = 0; i < 5; i++) {
                    int line = rand.Next(remainingLines.Count);
                    if (remainingLines[line] < Board.ROWS) for (int j = 0; j < Board.COLUMNS; j++) result.Add(new Vector2Int(j, remainingLines[line]));  //random row
                    else for (int j = Board.ROWS - 1; j >= 0; j--) result.Add(new Vector2Int(remainingLines[line] - Board.ROWS, j));  //random col
                    remainingLines.RemoveAt(line);
                }
                return result;
            };
        }
        if(getRandomPattern == null) { // please stop looking here
            getRandomPattern = () => { 
                System.Random rand = new System.Random(getRandomSeedByTurn());
                List<Vector2Int> result = new List<Vector2Int>();
                int patternType = rand.Next(3);
                Vector2Int pivot;
                bool dir = RNG.Next(2) == 0;
                switch (patternType) {
                    case 0:  // plus
                        pivot = new Vector2Int(rand.Next(Board.COLUMNS), rand.Next(Board.ROWS));
                        if (dir) {
                            if (RNG.Next(2) == 0) for (int i = 0; i < Board.COLUMNS; i++) result.Add(new Vector2Int(i, pivot.y));
                            else for (int i = Board.COLUMNS - 1; i >= 0; i--) result.Add(new Vector2Int(i, pivot.y));
                            if (RNG.Next(2) == 0) for (int i = 0; i < Board.ROWS; i++) result.Add(new Vector2Int(pivot.x, i));
                            else for (int i = Board.ROWS - 1; i >= 0; i--) result.Add(new Vector2Int(pivot.x, i));
                        }
                        else {
                            if (RNG.Next(2) == 0) for (int i = 0; i < Board.ROWS; i++) result.Add(new Vector2Int(pivot.x, i));
                            else for (int i = Board.ROWS - 1; i >= 0; i--) result.Add(new Vector2Int(pivot.x, i));
                            if (RNG.Next(2) == 0) for (int i = 0; i < Board.COLUMNS; i++) result.Add(new Vector2Int(i, pivot.y));
                            else for (int i = Board.COLUMNS - 1; i >= 0; i--) result.Add(new Vector2Int(i, pivot.y));
                        }
                        break;
                    case 1:  // cross
                        pivot = new Vector2Int(rand.Next(Board.COLUMNS - 2) + 1, Board.ROWS / 2);
                        int xOffset = dir ? 1 : -1;
                        int yOffset = RNG.Next(2) == 0 ? 1 : -1;
                        for (int i = -Board.ROWS / 2; i <= Board.ROWS / 2; i++) result.Add(new Vector2Int(pivot.x + i * xOffset, pivot.y + i * yOffset));
                        for (int i = -Board.ROWS / 2; i <= Board.ROWS / 2; i++) result.Add(new Vector2Int(pivot.x + i * xOffset, pivot.y - i * yOffset));
                        result.RemoveAll(v => v.x < 0 || v.x >= Board.COLUMNS || v.y < 0 || v.y >= Board.ROWS);
                        break;
                    case 2:  // box
                        int corner = rand.Next(4);
                        for (int i = 0; i < 4; i++) {
                            switch ((i + corner) % 4) {
                                case 0:
                                    if (dir) for (int j = 0; j < Board.ROWS; j++) result.Add(new Vector2Int(0, j));
                                    else for (int j = Board.ROWS - 1; j >= 0; j--) result.Add(new Vector2Int(0, j));
                                    break;
                                case 1:
                                    if (dir) for (int j = 0; j < Board.COLUMNS; j++) result.Add(new Vector2Int(j, Board.ROWS - 1));
                                    else for (int j = 0; j < Board.COLUMNS; j++) result.Add(new Vector2Int(j, 0));
                                    break;
                                case 2:
                                    if (dir) for (int j = Board.ROWS - 1; j >= 0; j--) result.Add(new Vector2Int(Board.COLUMNS - 1, j));
                                    else for (int j = 0; j < Board.ROWS; j++) result.Add(new Vector2Int(Board.COLUMNS - 1, j));
                                    break;
                                case 3:
                                    if (dir) for (int j = Board.COLUMNS - 1; j >= 0; j--) result.Add(new Vector2Int(j, 0));
                                    else for (int j = Board.COLUMNS - 1; j >= 0; j--) result.Add(new Vector2Int(j, Board.ROWS - 1));
                                    break;
                            }
                        }
                        break;
                }
                result = result.Distinct().ToList();
                return result;
            };
        }
    }
    protected override void addAllSkills() {
        initHelperSkills();
        skillList.Clear();
        if (currState.easySkills != null) foreach (int i in currState.easySkills) addEasySkill(i);
        else currState.easySkills = new List<int>();
        if (currState.medSkills != null) foreach (int i in currState.medSkills) addMedSkill(i);
        else currState.medSkills = new List<int>();
        if (currState.hardSkills != null) foreach (int i in currState.hardSkills) addHardSkill(i);
        else currState.hardSkills = new List<int>();
        base.addAllSkills();
    }
    private void addEasySkill(int index) {
        System.Random rand = new System.Random(getRandomSeedByFloor());
        Func<bool> wtu = () => true;
        Func<ORB_VALUE, OrbSpawnRate> newSpawnRates;
        switch (index) {
            case 0:
                int numShuffles = rand.Next(1, 3);
                wtu = () => GameController.Instance.isTurnMod(numShuffles, RNG.Next(numShuffles));
                skillList.Add(EnemyBoardSkill.ShuffleSkill(wtu, numShuffles * 5, 0.075f, skillTrans));
                break;
            case 1:
                EnemyBoardSkill clearOneLine = EnemyBoardSkill.MarkOrderSkill(wtu, getOneLine, 0.1f, skillTrans);
                clearOneLine.addRmvSkill(0);
                skillList.Add(clearOneLine);
                break;
            case 2:
                bool healSelfOnly = RNG.Next(2) == 0;
                int percentHeal = rand.Next(1, 6);
                wtu = () => GameController.Instance.isTurnMod(percentHeal, RNG.Next(percentHeal));
                Func<GameObject> getTarget = () => healSelfOnly ? gameObject : GameController.Instance.getCurrEnemies()[RNG.Next(GameController.Instance.getCurrEnemies().Count)].gameObject;
                Func<int> getDmg = () => getTarget().GetComponent<Enemy>().getState().maxHealth * percentHeal / 10;
                skillList.Add(EnemyAttack.Create(wtu, true, getTarget, getDmg, skillTrans));
                break;
            case 3:
                EnemyBoardSkill setOneLineEmpty = EnemyBoardSkill.MarkOrderSkill(wtu, getOneLine, 0.1f, skillTrans, 1);
                setOneLineEmpty.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
                skillList.Add(setOneLineEmpty);
                break;
            case 4:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill clearFiveLines = EnemyBoardSkill.MarkOrderSkill(wtu, getFiveLines, 0.1f, skillTrans, 1);
                clearFiveLines.addRmvSkill(0);
                skillList.Add(clearFiveLines);
                break;
            case 5:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill clearAllHeals = EnemyBoardSkill.MarkIfSkill(wtu, (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans, 1);
                clearAllHeals.addRmvSkill(0);
                skillList.Add(clearAllHeals);
                break;
            case 6:
                newSpawnRates = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.NULLIFY ? OrbSpawnRate.DECREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, -1, skillTrans));
                break;
            case 7:
                newSpawnRates = (ORB_VALUE orbVal) => orbVal == (ORB_VALUE)rand.Next(1, 10) ? OrbSpawnRate.MAX : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, 1, skillTrans));
                break;
            case 8:
                EnemyBuffs hpBuff = (EnemyBuffs)(rand.Next(3) + 1);
                Func<Enemy> getTarget2 = () => GameController.Instance.getCurrEnemies()[RNG.Next(GameController.Instance.getCurrEnemies().Count)];
                skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(2, RNG.Next(2)), hpBuff, getTarget2, 1, skillTrans));
                break;
            case 9:
                EnemyBoardSkill decOneLine = EnemyBoardSkill.MarkOrderSkill(wtu, getOneLine, 0.1f, skillTrans);
                decOneLine.addIncSkill(0.1f, (Orb o) => -1);
                skillList.Add(decOneLine);
                break;
        }
    }
    private void addMedSkill(int index) {
        System.Random rand = new System.Random(getRandomSeedByFloor());
        Func<bool> wtu = () => true;
        Func<ORB_VALUE, OrbSpawnRate> newSpawnRates;

        switch (index) {
            case 0:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill clearPattern = EnemyBoardSkill.MarkOrderSkill(wtu, getRandomPattern, 0.1f, skillTrans, 1);
                clearPattern.addRmvSkill(0);
                skillList.Add(clearPattern);
                break;
            case 1:
                int numHealReduce = rand.Next(1, 3);
                newSpawnRates = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.ZERO ? OrbSpawnRate.DECREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                wtu = () => GameController.Instance.isTurnMod(2 * numHealReduce, RNG.Next(numHealReduce));
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, numHealReduce, skillTrans));
                break;
            case 2:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                Func<Orb, bool> markAllOneDigit = (Orb o) => {
                    System.Random rand2 = new System.Random(getRandomSeedByTurn());
                    int digitToMark = rand2.Next(1, 10);
                    return o.getIntValue() == digitToMark;
                };
                Func<Orb, ORB_VALUE> changeDigit = (Orb o) => {
                    System.Random rand2 = new System.Random(getRandomSeedByTurn());
                    List<ORB_VALUE> possibleOrbVals = new List<ORB_VALUE>();
                    for (int i = 1; i < 10; i++) possibleOrbVals.Add((ORB_VALUE)i);
                    possibleOrbVals.Remove(o.getOrbValue());
                    return possibleOrbVals[rand2.Next(possibleOrbVals.Count)];
                };
                EnemyBoardSkill setOneDigitAsRand = EnemyBoardSkill.MarkIfSkill(wtu, markAllOneDigit, 0.1f, skillTrans);
                setOneDigitAsRand.addSetSkill(0.1f, changeDigit);
                skillList.Add(setOneDigitAsRand);
                break;
            case 3:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill replaceWithEmpty = EnemyBoardSkill.MarkOrderSkill(wtu, getRandomPattern, 0.1f, skillTrans, 1);
                replaceWithEmpty.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
                skillList.Add(replaceWithEmpty);
                break;
            case 4:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill replaceWithStop = EnemyBoardSkill.MarkOrderSkill(wtu, getOneLine, 0.1f, skillTrans, 1);
                replaceWithStop.addSetSkill(0.1f, (Orb o) => ORB_VALUE.STOP);
                skillList.Add(replaceWithStop);
                break;
            case 5:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill allHealsToEmpty = EnemyBoardSkill.MarkIfSkill(wtu, (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans, 1);
                allHealsToEmpty.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
                skillList.Add(allHealsToEmpty);
                break;
            case 6:
                newSpawnRates = (ORB_VALUE orbVal) => OrbSpawnRate.NORMAL;
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, -1, skillTrans));
                break;
            case 7:
                int numEmptyInc = rand.Next(1, 3);
                newSpawnRates = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.EMPTY ? OrbSpawnRate.INCREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                wtu = () => GameController.Instance.isTurnMod(2 * numEmptyInc, RNG.Next(numEmptyInc));
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, numEmptyInc, skillTrans));
                break;
            case 8:
                EnemyBuffs hpBuff = (EnemyBuffs)(rand.Next(3) + 1);
                skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(3, RNG.Next(3)), hpBuff, () => this, 1, skillTrans));
                break;
            case 9:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill decFiveLines = EnemyBoardSkill.MarkOrderSkill(wtu, getFiveLines, 0.1f, skillTrans, 1);
                decFiveLines.addIncSkill(0.1f, (Orb o) => -1);
                skillList.Add(decFiveLines);
                break;
        }
    }
    private void addHardSkill(int index) {
        System.Random rand = new System.Random(getRandomSeedByFloor());
        Func<bool> wtu = () => true;
        Func<ORB_VALUE, OrbSpawnRate> newSpawnRates;
        bool markZeroAndFives = currState.number % 5 == 0;
        bool isAnti = RNG.Next(2) == 0;

        switch (index) {
            case 0:
                int numNullify = rand.Next(1, 3);
                wtu = () => GameController.Instance.isTurnMod(2 * numNullify, RNG.Next(numNullify));
                Func<Orb, bool> randZeros = (Orb o) => {
                    List<Orb> currZeros = new List<Orb>();
                    for(int i = 0; i < Board.COLUMNS; i++) {
                        for(int j = 0; j < Board.ROWS; j++) {
                            Orb z = Board.Instance.getOrb(i, j);
                            if (z.getOrbValue() == ORB_VALUE.ZERO) currZeros.Add(z);
                        }
                    }
                    for (int i = 0; i < numNullify && currZeros.Count > 0; i++) {
                        int orb = rand.Next(currZeros.Count);
                        if (currZeros[orb] == o) return true;
                        currZeros.RemoveAt(orb);
                    }
                    return false;
                };
                EnemyBoardSkill zerosToNullify = EnemyBoardSkill.MarkIfSkill(wtu, randZeros, 0.1f, skillTrans);
                zerosToNullify.addSetSkill(0.1f, (Orb o) => ORB_VALUE.NULLIFY);
                skillList.Add(zerosToNullify);
                break;
            case 1:
                int numOrbReduce = rand.Next(1, 4);
                newSpawnRates = (ORB_VALUE orbVal) => {
                    if (markZeroAndFives) return orbVal == ORB_VALUE.ZERO || orbVal == ORB_VALUE.FIVE ? OrbSpawnRate.DECREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                    return orbVal <= ORB_VALUE.NINE && (int)orbVal % 2 == 0 ? OrbSpawnRate.DECREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                };
                wtu = () => GameController.Instance.isTurnMod(2 * numOrbReduce, RNG.Next(numOrbReduce));
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, numOrbReduce, skillTrans));
                break;
            case 2:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                Func<Orb, bool> evensOrFives = (Orb o) => o.isDigit() && o.getIntValue() % (markZeroAndFives ? 5 : 2) == 0;
                EnemyBoardSkill clearEvensOrFives = EnemyBoardSkill.MarkIfSkill(wtu, evensOrFives, 0.1f, skillTrans);
                clearEvensOrFives.addRmvSkill(0);
                skillList.Add(clearEvensOrFives);
                break;
            case 3:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill replaceWithAntiOrStop = EnemyBoardSkill.MarkOrderSkill(wtu, getRandomPattern, 0.1f, skillTrans, 1);
                replaceWithAntiOrStop.addSetSkill(0.1f, (Orb o) => isAnti ? ORB_VALUE.POISON : ORB_VALUE.STOP);
                skillList.Add(replaceWithAntiOrStop);
                break;
            case 4:
                wtu = () => GameController.Instance.isTurnMod(4, RNG.Next(4));
                EnemyBoardSkill emptyFiveLines = EnemyBoardSkill.MarkOrderSkill(wtu, getFiveLines, 0.1f, skillTrans, 1);
                emptyFiveLines.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
                skillList.Add(emptyFiveLines);
                break;
            case 5:
                wtu = () => GameController.Instance.isTurnMod(3, RNG.Next(3));
                EnemyBoardSkill allHealsToAntiOrStop = EnemyBoardSkill.MarkIfSkill(wtu, (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans, 1);
                allHealsToAntiOrStop.addSetSkill(0.1f, (Orb o) => isAnti ? ORB_VALUE.POISON : ORB_VALUE.STOP);
                skillList.Add(allHealsToAntiOrStop);
                break;
            case 6:
                int numNoneZeroTurns = rand.Next(1, 3);
                newSpawnRates = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.ZERO ? OrbSpawnRate.NONE : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                wtu = () => GameController.Instance.isTurnMod(2 * numNoneZeroTurns, RNG.Next(numNoneZeroTurns));
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, numNoneZeroTurns, skillTrans));
                break;
            case 7:
                int antiOrStop = rand.Next(2, 4);
                newSpawnRates = (ORB_VALUE orbVal) => orbVal == (antiOrStop == 2 ? ORB_VALUE.POISON : ORB_VALUE.STOP) ? OrbSpawnRate.INCREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
                wtu = () => GameController.Instance.isTurnMod(antiOrStop, RNG.Next(antiOrStop - 1));
                skillList.Add(EnemyOrbSkill.Create(wtu, newSpawnRates, 1, skillTrans));
                break;
            case 8:
                int numTimerTurns = rand.Next(1, 3);
                wtu = () => GameController.Instance.isTurnMod(2 * numTimerTurns, RNG.Next(numTimerTurns));
                EnemyTimer numDOT = EnemyTimer.Create(wtu, Mathf.Clamp(currState.number/10f, 0f, 10f), numTimerTurns, skillTrans);
                numDOT.addDOTSkill(() => -currState.number);
                skillList.Add(numDOT);
                break;
            case 9:
                int numDecrement = rand.Next(1, 4);
                wtu = () => GameController.Instance.isTurnMod(numDecrement + 2, RNG.Next(numDecrement));
                EnemyBoardSkill decrementPattern = EnemyBoardSkill.MarkOrderSkill(wtu, getRandomPattern, 0.1f, skillTrans, numDecrement);
                decrementPattern.addIncSkill(0.1f, (Orb o) => -numDecrement);
                skillList.Add(decrementPattern);
                break;
        }
    }
}