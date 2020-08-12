using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniBoss : Enemy {
    public static Enemy Create(int num) => Create("Mini Boss", num, MiniBossData.getHP(num), MiniBossData.getATK(num), MiniBossData.getSprite(num)).GetComponent<MiniBoss>();
    
    protected override void loadAllHPBarIMGs() => enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Mini Boss");
    public override void setPosition(EnemyPosition pos) {
        base.setPosition(pos);
        switch (pos) {
            case EnemyPosition.LEFT_2: case EnemyPosition.LEFT_3:
                HPtrans.anchoredPosition = new Vector3(-268f, HPtrans.anchoredPosition.y, -14f);
                break;
            case EnemyPosition.RIGHT_2: case EnemyPosition.RIGHT_3:
                HPtrans.anchoredPosition = new Vector3(267f, HPtrans.anchoredPosition.y, -14f);
                break;
            default: break;
        }
    }
    protected override void addAllSkills() {
        switch (currState.number) {
            // Floor 15
            case 16: addSkills16(); break;
            case 25: addSkills25(); break;
            case 36: addSkills36(); break;
            // Floor 30
            case 26: addSkills26(); break;
            case 27: addSkills27(); break;
            case 28: addSkills28(); break;
            // Floor 45
            case 11: addSkills11(); break;
            case 13: addSkills13(); break;
            // Floor 46
            case 17: addSkills17(); break;
            case 19: addSkills19(); break;
            // Floor 47
            case 64: addSkills64(); break;
            case 81: addSkills81(); break;
            // Floor 48
            case 89: addSkills89(); break;
            case 97: addSkills97(); break;
            // Floor 49
            case 3: addSkills3(); break;
            case 6: addSkills6(); break;
            case 9: addSkills9(); break;
            // Invalid number
            default: base.addAllSkills(); break;
        }
    }
    private void addSkills16() {
        EnemyBoardSkill setFoursToEmpty = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 2), (Orb o) => o.getOrbValue() == ORB_VALUE.FOUR, 0.1f, skillTrans);
        setFoursToEmpty.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
        skillList.Add(setFoursToEmpty);

        Func<ORB_VALUE, OrbSpawnRate> incNullify = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.NULLIFY ? OrbSpawnRate.INCREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(() => GameController.Instance.isTurnMod(3), incNullify, 1, skillTrans));

        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3, 1), false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills25() {
        EnemyBoardSkill setFivesToAnti = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3), (Orb o) => o.getOrbValue() == ORB_VALUE.FIVE, 0.1f, skillTrans);
        setFivesToAnti.addSetSkill(0.1f, (Orb o) => ORB_VALUE.POISON);
        skillList.Add(setFivesToAnti);

        EnemyBoardSkill setHealToNullify = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 1), (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans);
        setHealToNullify.addSetSkill(0.1f, (Orb o) => ORB_VALUE.NULLIFY);
        skillList.Add(setHealToNullify);

        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3, 2), false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills36() {
        EnemyBoardSkill setSixToStop = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 1), (Orb o) => o.getOrbValue() == ORB_VALUE.SIX, 0.1f, skillTrans);
        setSixToStop.addSetSkill(0.1f, (Orb o) => ORB_VALUE.STOP);
        skillList.Add(setSixToStop);

        EnemyBoardSkill clearNullify = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 2), (Orb o) => o.getOrbValue() == ORB_VALUE.NULLIFY, 0.1f, skillTrans);
        clearNullify.addRmvSkill(0.1f);
        skillList.Add(clearNullify);

        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3), false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills26() {
        Func<Enemy> getTarget = () => GameController.Instance.getCurrEnemies()[RNG.Next(GameController.Instance.getCurrEnemies().Count)];
        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(3), EnemyBuffs.DMG_ABSORB, getTarget, 1, skillTrans));

        EnemyBoardSkill decSix = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 1), (Orb o) => o.getOrbValue() == ORB_VALUE.SIX, 0.1f, skillTrans);
        decSix.addIncSkill(0.1f, (Orb o) => -1);
        skillList.Add(decSix);

        EnemyTimer DOT6 = EnemyTimer.Create(() => GameController.Instance.isTurnMod(3, 2), 1f, 1, skillTrans);
        DOT6.addDOTSkill(() => -6);
        skillList.Add(DOT6);
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3), false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills27() {
        Func<Enemy> getTarget = () => GameController.Instance.getCurrEnemies()[RNG.Next(GameController.Instance.getCurrEnemies().Count)];
        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(3, 1), EnemyBuffs.DMG_MITI_50, getTarget, 1, skillTrans));

        EnemyBoardSkill decSeven = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 2), (Orb o) => o.getOrbValue() == ORB_VALUE.SEVEN, 0.1f, skillTrans);
        decSeven.addIncSkill(0.1f, (Orb o) => -2);
        skillList.Add(decSeven);

        EnemyTimer DOT7 = EnemyTimer.Create(() => GameController.Instance.isTurnMod(3), 1f, 1, skillTrans);
        DOT7.addDOTSkill(() => -7);
        skillList.Add(DOT7);
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3, 1), false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills28() {
        Func<Enemy> getTarget = () => GameController.Instance.getCurrEnemies()[RNG.Next(GameController.Instance.getCurrEnemies().Count)];
        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(3, 2), EnemyBuffs.DMG_REFLECT, getTarget, 1, skillTrans));

        EnemyBoardSkill decEight = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3), (Orb o) => o.getOrbValue() == ORB_VALUE.EIGHT, 0.1f, skillTrans);
        decEight.addIncSkill(0.1f, (Orb o) => -3);
        skillList.Add(decEight);

        EnemyTimer DOT8 = EnemyTimer.Create(() => GameController.Instance.isTurnMod(3, 1), 1f, 1, skillTrans);
        DOT8.addDOTSkill(() => -8);
        skillList.Add(DOT8);
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3, 2), false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills11() {
        Func<Enemy> getEnemy = () => {
            foreach (Enemy e in GameController.Instance.getCurrEnemies()) if (e.getEnemyID() != getEnemyID()) return e;
            return null;
        };
        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(2), EnemyBuffs.DMG_ABSORB, getEnemy, 1, skillTrans));

        Func<Orb, bool> randDigits = (Orb o) => {
            System.Random rand = new System.Random(getRandomSeedByTurn());
            for(int i = 0; i < 10; i++) {
                List<Orb> currDigit = new List<Orb>();
                for (int j = 0; j < Board.COLUMNS; j++) {
                    for (int k = 0; k < Board.ROWS; k++) {
                        Orb z = Board.Instance.getOrb(j, k);
                        if (z.getOrbValue() == (ORB_VALUE)i) currDigit.Add(z);
                    }
                }
                if (o.getIntValue() == i && currDigit[rand.Next(currDigit.Count)] == o) return true;
            }
            return false;
        };
        EnemyBoardSkill oneEmptyPerDigit = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(2), randDigits, 0.1f, skillTrans);
        oneEmptyPerDigit.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
        skillList.Add(oneEmptyPerDigit);

        Func<ORB_VALUE, OrbSpawnRate> incAnti = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.POISON ? OrbSpawnRate.INCREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(() => GameController.Instance.isTurnMod(2, 1), incAnti, 1, skillTrans));

        skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => (currState.currHealth - currState.maxHealth) / 2 - 100, skillTrans));
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(2), false, () => gameObject, () => currState.currHealth / 2, skillTrans));
    }
    private void addSkills13() {
        Func<Enemy> getEnemy = () => {
            foreach (Enemy e in GameController.Instance.getCurrEnemies()) if (e.getEnemyID() != getEnemyID()) return e;
            return null;
        };
        Func<GameObject> getGameObject = () => {
            foreach (Enemy e in GameController.Instance.getCurrEnemies()) if (e.getEnemyID() != getEnemyID()) return e.gameObject;
            return null;
        };
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(2, 1), false, getGameObject, () => -getEnemy().getState().currHealth/2, skillTrans));
        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(2, 1), EnemyBuffs.DMG_REFLECT, getEnemy, 1, skillTrans));

        EnemyBoardSkill incAll = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(2), (Orb o) => true, 0f, skillTrans);
        incAll.addIncSkill(0.05f, (Orb o) => 1);
        skillList.Add(incAll);

        Func<ORB_VALUE, OrbSpawnRate> incNullify = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.NULLIFY ? OrbSpawnRate.INCREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(() => GameController.Instance.isTurnMod(2), incNullify, 1, skillTrans));

        skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => -GameController.Instance.getCurrTurn() * 50 - 100, skillTrans));
    }
    private void addSkills17() {
        Func<List<Vector2Int>> randOrbPerCol = () => {
            System.Random rand = new System.Random(getRandomSeedByTurn());
            List<int> remainingRows = new List<int>();
            for (int i = 0; i < Board.ROWS; i++) remainingRows.Add(i);
            remainingRows.Add(rand.Next(Board.ROWS));
            List<Vector2Int> chosenOrbs = new List<Vector2Int>();
            for (int c = 0; c < Board.COLUMNS; c++) {
                int randRow = rand.Next(remainingRows.Count);
                chosenOrbs.Add(new Vector2Int(c, remainingRows[randRow]));
                remainingRows.RemoveAt(randRow);
            }
            return chosenOrbs;
        };
        EnemyBoardSkill stopCol = EnemyBoardSkill.MarkOrderSkill(() => GameController.Instance.isTurnMod(3), randOrbPerCol, 0.1f, skillTrans, 1);
        stopCol.addSetSkill(0.1f, (Orb o) => ORB_VALUE.STOP);
        skillList.Add(stopCol);

        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(4), EnemyBuffs.DMG_MITI_50, () => this, 1, skillTrans));
        EnemyTimer DOT = EnemyTimer.Create(() => true, 2f, -1, skillTrans);
        DOT.addDOTSkill(() => 5);
        DOT.addTimeDelay(1f);
        skillList.Add(DOT);
        skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => (int)(-GameController.Instance.getTimeOnFloor() * 2), skillTrans));
    }
    private void addSkills19() {
        Func<List<Vector2Int>> randOrbPerRow = () => {
            System.Random rand = new System.Random(getRandomSeedByTurn());
            List<int> remainingCols = new List<int>();
            for (int i = 0; i < Board.COLUMNS; i++) remainingCols.Add(i);
            List<Vector2Int> chosenOrbs = new List<Vector2Int>();
            for (int r = Board.ROWS - 1; r >= 0; r--) {
                int randCol = rand.Next(remainingCols.Count);
                chosenOrbs.Add(new Vector2Int(remainingCols[randCol], r));
                remainingCols.RemoveAt(randCol);
            }
            return chosenOrbs;
        };
        EnemyBoardSkill antiRow = EnemyBoardSkill.MarkOrderSkill(() => GameController.Instance.isTurnMod(3, 1), randOrbPerRow, 0.1f, skillTrans, 1);
        antiRow.addSetSkill(0.1f, (Orb o) => ORB_VALUE.POISON);
        skillList.Add(antiRow);

        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(4, 2), EnemyBuffs.DMG_MITI_50, () => this, 1, skillTrans));
        EnemyTimer DOT = EnemyTimer.Create(() => true, 2f, -1, skillTrans);
        DOT.addDOTSkill(() => 2);
        skillList.Add(DOT);
        skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => (int)(-GameController.Instance.getTimeOnFloor() * 5), skillTrans));
    }
    private void addSkills64() {
        Func<bool> wtu0 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(4) : GameController.Instance.isTurnMod(2);
        EnemyBoardSkill healsToAnti = EnemyBoardSkill.MarkIfSkill(wtu0, (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans);
        healsToAnti.addSetSkill(0.1f, (Orb o) => ORB_VALUE.POISON);
        skillList.Add(healsToAnti);

        Func<bool> wtu1 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(4, 2) : GameController.Instance.isTurnMod(2, 1);
        EnemyBoardSkill fivesToEmpty = EnemyBoardSkill.MarkIfSkill(wtu1, (Orb o) => o.getOrbValue() == ORB_VALUE.FIVE, 0.1f, skillTrans);
        fivesToEmpty.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
        skillList.Add(fivesToEmpty);

        Func<bool> wtu2 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2) : true;
        Func<Orb, bool> is248 = (Orb o) => o.isUnmarked() && (o.getOrbValue() == ORB_VALUE.TWO || o.getOrbValue() == ORB_VALUE.FOUR || o.getOrbValue() == ORB_VALUE.EIGHT);
        EnemyBoardSkill mark248 = EnemyBoardSkill.MarkIfSkill(wtu2, is248, 0.1f, skillTrans);
        mark248.addMarkRemainSkill();
        skillList.Add(mark248);
        Func<int> getDmg = () => {
            List<Orb> markedOrbs = Board.Instance.getAllMarkedOrbsBy(getSkillID(mark248, mark248.getLastActivatedTurn()), null);
            int sum = 0;
            foreach (Orb o in markedOrbs) sum += o.getIntValue();
            return -sum * markedOrbs.Count;
        };
        Func<bool> wtu2a = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2, 1) : true;
        skillList.Add(EnemyAttack.Create(wtu2a, false, () => Player.Instance.gameObject, getDmg, skillTrans));
        Func<Orb, bool> markedBy248 = (Orb o) => o.getIsMarkedBy(getSkillID(mark248, mark248.getLastActivatedTurn()));
        EnemyBoardSkill clear248 = EnemyBoardSkill.MarkIfSkill(wtu2a, markedBy248, 0, skillTrans);
        clear248.addRmvSkill(0.1f);
        skillList.Add(clear248);

        Func<bool> wtu3 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2) : false;
        EnemyBoardSkill markPivot = null;
        Func<Orb, bool> getPivot = (Orb o) => {
            for (int c = 0; c < Board.COLUMNS; c++) for (int r = 0; r < Board.ROWS; r++) if (Board.Instance.getOrb(c, r).getIsMarkedBy(getSkillID(markPivot, markPivot.getActivatedTurn()))) return false;
            System.Random rand = new System.Random(getRandomSeedByTurn());
            List<Orb> potentialOrbs = new List<Orb>();
            for (int c = 0; c < Board.COLUMNS; c++) for (int r = 0; r < Board.ROWS; r++) if (Board.Instance.getOrb(c, r).isUnmarked()) potentialOrbs.Add(Board.Instance.getOrb(c, r));
            int pivotIndex = rand.Next(potentialOrbs.Count);
            if (potentialOrbs.Count > 0 && o == potentialOrbs[pivotIndex]) return true;
            return false;
        };
        markPivot = EnemyBoardSkill.MarkIfSkill(wtu3, getPivot, 0.1f, skillTrans, 0);
        markPivot.addMarkRemainSkill();
        skillList.Add(markPivot);
        Func<bool> wtu3a = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2, 1) : false;
        Func<List<Vector2Int>> plusPivot = () => {
            List<Vector2Int> plus = new List<Vector2Int>();
            Vector2Int pivot = new Vector2Int(-1, -1);
            for (int c = 0; c < Board.COLUMNS; c++) for (int r = 0; r < Board.ROWS; r++) if (Board.Instance.getOrb(c, r).getIsMarkedBy(getSkillID(markPivot, markPivot.getLastActivatedTurn()))) pivot = new Vector2Int(c, r);
            if (pivot.x != -1 && pivot.y != -1) {
                bool reachedEnd = false;
                for (int offset = 0; !reachedEnd; offset++) {
                    reachedEnd = true;
                    if (pivot.x + offset < Board.COLUMNS) {
                        plus.Add(new Vector2Int(pivot.x + offset, pivot.y));
                        reachedEnd = false;
                    }
                    if (pivot.y + offset < Board.ROWS) {
                        plus.Add(new Vector2Int(pivot.x, pivot.y + offset));
                        reachedEnd = false;
                    }
                    if (pivot.x - offset >= 0) {
                        plus.Add(new Vector2Int(pivot.x - offset, pivot.y));
                        reachedEnd = false;
                    }
                    if (pivot.y - offset >= 0) {
                        plus.Add(new Vector2Int(pivot.x, pivot.y - offset));
                        reachedEnd = false;
                    }
                }
            }
            plus = plus.Distinct().ToList();
            return plus;
        };
        EnemyBoardSkill clearPlusPivot = EnemyBoardSkill.MarkOrderSkill(wtu3a, plusPivot, 0.1f, skillTrans);
        clearPlusPivot.addRmvSkill(0.1f);
        skillList.Add(clearPlusPivot);

        Func<bool> wtu4 = () => GameController.Instance.getCurrEnemies().Count != 2;
        EnemyBoardSkill clearPlus = EnemyBoardSkill.MarkOrderSkill(wtu4, null, 0.1f, skillTrans);
        clearPlus.addRmvSkill(0.1f);
        skillList.Add(clearPlus);
    }
    private void addSkills81() {
        Func<bool> wtu0 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(4, 1) : GameController.Instance.isTurnMod(2);
        EnemyBoardSkill onesToStop = EnemyBoardSkill.MarkIfSkill(wtu0, (Orb o) => o.getOrbValue() == ORB_VALUE.ONE, 0.1f, skillTrans);
        onesToStop.addSetSkill(0.1f, (Orb o) => ORB_VALUE.STOP);
        skillList.Add(onesToStop);

        Func<bool> wtu1 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(4, 3) : GameController.Instance.isTurnMod(2, 1);
        EnemyBoardSkill sevensToNullify = EnemyBoardSkill.MarkIfSkill(wtu1, (Orb o) => o.getOrbValue() == ORB_VALUE.SEVEN, 0.1f, skillTrans);
        sevensToNullify.addSetSkill(0.1f, (Orb o) => ORB_VALUE.NULLIFY);
        skillList.Add(sevensToNullify);

        Func<bool> wtu2 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2, 1) : true;
        Func<Orb, bool> is369 = (Orb o) => o.isUnmarked() && (o.getOrbValue() == ORB_VALUE.THREE || o.getOrbValue() == ORB_VALUE.SIX || o.getOrbValue() == ORB_VALUE.NINE);
        EnemyBoardSkill mark369 = EnemyBoardSkill.MarkIfSkill(wtu2, is369, 0.1f, skillTrans);
        mark369.addMarkRemainSkill();
        skillList.Add(mark369);
        Func<int> getDmg = () => {
            List<Orb> markedOrbs = Board.Instance.getAllMarkedOrbsBy(getSkillID(mark369, mark369.getLastActivatedTurn()), null);
            int sum = 0;
            foreach (Orb o in markedOrbs) sum += o.getIntValue();
            return -sum * markedOrbs.Count;
        };
        Func<bool> wtu2a = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2) : true;
        skillList.Add(EnemyAttack.Create(wtu2a, false, () => Player.Instance.gameObject, getDmg, skillTrans));
        Func<Orb, bool> markedBy369 = (Orb o) => o.getIsMarkedBy(getSkillID(mark369, mark369.getLastActivatedTurn()));
        EnemyBoardSkill clear369 = EnemyBoardSkill.MarkIfSkill(wtu2a, markedBy369, 0, skillTrans);
        clear369.addRmvSkill(0.1f);
        skillList.Add(clear369);

        Func<bool> wtu3 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2, 1) : false;
        EnemyBoardSkill markPivot = null;
        Func<Orb, bool> getPivot = (Orb o) => {
            for (int c = 0; c < Board.COLUMNS; c++) for (int r = 0; r < Board.ROWS; r++) if (Board.Instance.getOrb(c, r).getIsMarkedBy(getSkillID(markPivot, markPivot.getActivatedTurn()))) return false;
            System.Random rand = new System.Random(getRandomSeedByTurn());
            List<Orb> potentialOrbs = new List<Orb>();
            for (int c = 0; c < Board.COLUMNS; c++) for (int r = 0; r < Board.ROWS; r++) if (Board.Instance.getOrb(c, r).isUnmarked()) potentialOrbs.Add(Board.Instance.getOrb(c, r));
            int pivotIndex = rand.Next(potentialOrbs.Count);
            if (potentialOrbs.Count > 0 && o == potentialOrbs[pivotIndex]) return true;
            return false;
        };
        markPivot = EnemyBoardSkill.MarkIfSkill(wtu3, getPivot, 0.1f, skillTrans, 0);
        markPivot.addMarkRemainSkill();
        skillList.Add(markPivot);
        Func<bool> wtu3a = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2) : false;
        Func<List<Vector2Int>> plusPivot = () => {
            List<Vector2Int> plus = new List<Vector2Int>();
            Vector2Int pivot = new Vector2Int(-1, -1);
            for (int c = 0; c < Board.COLUMNS; c++) for (int r = 0; r < Board.ROWS; r++) if (Board.Instance.getOrb(c, r).getIsMarkedBy(getSkillID(markPivot, markPivot.getLastActivatedTurn()))) pivot = new Vector2Int(c, r);
            if (pivot.x != -1 && pivot.y != -1) {
                bool reachedEnd = false;
                for (int offset = 0; !reachedEnd; offset++) {
                    reachedEnd = true;
                    if (pivot.x + offset < Board.COLUMNS) {
                        plus.Add(new Vector2Int(pivot.x + offset, pivot.y));
                        reachedEnd = false;
                    }
                    if (pivot.y + offset < Board.ROWS) {
                        plus.Add(new Vector2Int(pivot.x, pivot.y + offset));
                        reachedEnd = false;
                    }
                    if (pivot.x - offset >= 0) {
                        plus.Add(new Vector2Int(pivot.x - offset, pivot.y));
                        reachedEnd = false;
                    }
                    if (pivot.y - offset >= 0) {
                        plus.Add(new Vector2Int(pivot.x, pivot.y - offset));
                        reachedEnd = false;
                    }
                }
            }
            plus = plus.Distinct().ToList();
            return plus;
        };
        EnemyBoardSkill decPlusPivot = EnemyBoardSkill.MarkOrderSkill(wtu3a, plusPivot, 0.1f, skillTrans);
        decPlusPivot.addRmvSkill(0.1f);
        skillList.Add(decPlusPivot);

        Func<bool> wtu4 = () => GameController.Instance.getCurrEnemies().Count != 2;
        EnemyBoardSkill decPlus = EnemyBoardSkill.MarkOrderSkill(wtu4, null, 0.1f, skillTrans);
        decPlus.addRmvSkill(0.1f);
        skillList.Add(decPlus);
    }
    private void addSkills89() {
        Func<ORB_VALUE, OrbSpawnRate> decNullify = (ORB_VALUE orbVal) => orbVal == ORB_VALUE.NULLIFY ? OrbSpawnRate.DECREASED : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(() => true, decNullify, -1, skillTrans));

        EnemyBoardSkill setNinesToStop = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(2), (Orb o) => o.getOrbValue() == ORB_VALUE.NINE, 0.1f, skillTrans);
        setNinesToStop.addSetSkill(0.1f, (Orb o) => ORB_VALUE.STOP);
        skillList.Add(setNinesToStop);
        Func<List<Vector2Int>> rand2Rows = () => {
            List<Vector2Int> rows = new List<Vector2Int>();
            int row1 = (GameController.Instance.getCurrTurn()) % Board.ROWS, row2 = (GameController.Instance.getCurrTurn() + 2) % Board.ROWS;
            for (int c = 0; c < Board.COLUMNS; c++) {
                rows.Add(new Vector2Int(c, row1));
                rows.Add(new Vector2Int(c, row2));
            }
            return rows;
        };
        EnemyBoardSkill inc2Rows = EnemyBoardSkill.MarkOrderSkill(() => GameController.Instance.isTurnMod(2, 1), rand2Rows, 0.1f, skillTrans);
        inc2Rows.addIncSkill(0.1f, (Orb o) => 1);
        skillList.Add(inc2Rows);

        Func<bool> wtu1 = () => GameController.Instance.isTurnMod(2, 1);
        EnemyBoardSkill markAllStops = EnemyBoardSkill.MarkIfSkill(wtu1, (Orb o) => o.getOrbValue() == ORB_VALUE.STOP, 0.1f, skillTrans);
        markAllStops.addMarkRemainSkill();
        skillList.Add(markAllStops);
        Func<int> getHeals = () => 33 * Board.Instance.getAllMarkedOrbsBy(getSkillID(markAllStops, markAllStops.getLastActivatedTurn()), null).Count;
        skillList.Add(EnemyAttack.Create(wtu1, true, () => gameObject, getHeals, skillTrans));
        Func<ORB_VALUE, OrbSpawnRate> maxOverFive = (ORB_VALUE orbVal) => ((int)orbVal >= 5 && (int)orbVal <= 9) ? OrbSpawnRate.MAX : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(wtu1, maxOverFive, 0, skillTrans));
        EnemyBoardSkill clearStops = EnemyBoardSkill.MarkIfSkill(wtu1, (Orb o) => o.getOrbValue() == ORB_VALUE.STOP, 0, skillTrans);
        clearStops.addRmvSkill(0.1f);
        skillList.Add(clearStops);

        Func<bool> wtu2 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2) : true;
        EnemyBoardSkill mark2Rows = EnemyBoardSkill.MarkOrderSkill(wtu2, rand2Rows, 0.1f, skillTrans);
        skillList.Add(mark2Rows);
        Func<int> getDmg = () => {
            int sum1 = 0, sum2 = 0;
            int row1 = (GameController.Instance.getCurrTurn()) % Board.ROWS, row2 = (GameController.Instance.getCurrTurn() + 2) % Board.ROWS;
            for (int c = 0; c < Board.COLUMNS; c++) {
                if (Board.Instance.getOrb(c, row1).isDigit()) sum1 += Board.Instance.getOrb(c, row1).getIntValue();
                if (Board.Instance.getOrb(c, row2).isDigit()) sum2 += Board.Instance.getOrb(c, row2).getIntValue();
            }
            return -sum1 * sum2;
        };
        skillList.Add(EnemyAttack.Create(wtu2, false, () => Player.Instance.gameObject, getDmg, skillTrans));
    }
    private void addSkills97() {
        Func<ORB_VALUE, OrbSpawnRate> noneOverFive = (ORB_VALUE orbVal) => ((int)orbVal >= 5 && (int)orbVal <= 9) ? OrbSpawnRate.NONE : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(() => true, noneOverFive, -1, skillTrans));

        Func<List<Vector2Int>> rand2Cols = () => {
            List<Vector2Int> cols = new List<Vector2Int>();
            int col1 = (GameController.Instance.getCurrTurn()) % Board.COLUMNS, col2 = (GameController.Instance.getCurrTurn() + 2) % Board.COLUMNS;
            for (int r = Board.ROWS - 1; r >= 0; r--) {
                cols.Add(new Vector2Int(col1, r));
                cols.Add(new Vector2Int(col2, r));
            }
            return cols;
        };
        EnemyBoardSkill inc2Cols = EnemyBoardSkill.MarkOrderSkill(() => GameController.Instance.isTurnMod(2), rand2Cols, 0.1f, skillTrans);
        inc2Cols.addIncSkill(0.1f, (Orb o) => 1);
        skillList.Add(inc2Cols);
        EnemyBoardSkill setNinesToAnti = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(2, 1), (Orb o) => o.getOrbValue() == ORB_VALUE.NINE, 0.1f, skillTrans);
        setNinesToAnti.addSetSkill(0.1f, (Orb o) => ORB_VALUE.POISON);
        skillList.Add(setNinesToAnti);

        Func<bool> wtu1 = () => GameController.Instance.isTurnMod(2);
        EnemyBoardSkill markAllStops = EnemyBoardSkill.MarkIfSkill(wtu1, (Orb o) => o.getOrbValue() == ORB_VALUE.POISON, 0.1f, skillTrans);
        markAllStops.addMarkRemainSkill();
        skillList.Add(markAllStops);
        Func<int> getHeals = () => 33 * Board.Instance.getAllMarkedOrbsBy(getSkillID(markAllStops, markAllStops.getLastActivatedTurn()), null).Count;
        skillList.Add(EnemyAttack.Create(wtu1, true, () => gameObject, getHeals, skillTrans));
        Func<ORB_VALUE, OrbSpawnRate> maxOverFive = (ORB_VALUE orbVal) => ((int)orbVal >= 5 && (int)orbVal <= 9) ? OrbSpawnRate.MAX : Board.getDefaultOrbSpawnRates()[(int)orbVal];
        skillList.Add(EnemyOrbSkill.Create(wtu1, maxOverFive, 0, skillTrans));
        EnemyBoardSkill clearStops = EnemyBoardSkill.MarkIfSkill(wtu1, (Orb o) => o.getOrbValue() == ORB_VALUE.POISON, 0, skillTrans);
        clearStops.addRmvSkill(0.1f);
        skillList.Add(clearStops);

        Func<bool> wtu2 = () => GameController.Instance.getCurrEnemies().Count == 2 ? GameController.Instance.isTurnMod(2, 1) : true;
        EnemyBoardSkill mark2Cols = EnemyBoardSkill.MarkOrderSkill(wtu2, rand2Cols, 0.1f, skillTrans);
        skillList.Add(mark2Cols);
        Func<int> getDmg = () => {
            int sum1 = 0, sum2 = 0;
            int col1 = (GameController.Instance.getCurrTurn()) % Board.COLUMNS, col2 = (GameController.Instance.getCurrTurn() + 2) % Board.COLUMNS;
            for (int r = Board.ROWS - 1; r >= 0; r--) {
                if (Board.Instance.getOrb(col1, r).isDigit()) sum1 += Board.Instance.getOrb(col1, r).getIntValue();
                if (Board.Instance.getOrb(col2, r).isDigit()) sum2 += Board.Instance.getOrb(col2, r).getIntValue();
            }
            return -sum1 * sum2;
        };
        skillList.Add(EnemyAttack.Create(wtu2, false, () => Player.Instance.gameObject, getDmg, skillTrans));
    }
    // to-do: go ham on turns when minibosses die for attack and skill 3
    private void addSkills3() {
        EnemyTimer decOnPlayer = EnemyTimer.Create(() => true, 3f, -1, skillTrans);
        decOnPlayer.addIncSkill((Orb o) => -1);
        skillList.Add(decOnPlayer);

        EnemyBoardSkill setFoursToNullify = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(4), (Orb o) => o.getOrbValue() == ORB_VALUE.FOUR, 0.1f, skillTrans);
        setFoursToNullify.addSetSkill(0.1f, (Orb o) => ORB_VALUE.NULLIFY);
        skillList.Add(setFoursToNullify);

        Func<bool> wtu = () => { 
            List<Enemy> eList = GameController.Instance.getCurrEnemies();
            foreach(Enemy e in eList) if (e.getState().number == 6) return false;
            return GameController.Instance.isTurnMod(4, 3);
        };
        EnemyBoardSkill setHealsToAnti = EnemyBoardSkill.MarkIfSkill(wtu, (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans);
        setHealsToAnti.addSetSkill(0.1f, (Orb o) => ORB_VALUE.POISON);
        skillList.Add(setHealsToAnti);


        Func<Orb, bool> allOrbsLessThan = (Orb o) => {
            int range = 3 + (3 - GameController.Instance.getCurrEnemies().Count);
            return o.isDigit() && o.getIntValue() <= range;
        };
        EnemyBoardSkill markLess = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3), allOrbsLessThan, 0.1f, skillTrans);
        skillList.Add(markLess);
        Func<int> getDmg = () => {
            List<Orb> markedOrbs = Board.Instance.getAllMarkedOrbsBy(getSkillID(markLess, markLess.getActivatedTurn()), null);
            int sum = 0;
            foreach (Orb o in markedOrbs) if (o.isDigit()) sum += o.getIntValue();
            return sum * -9;
        };
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3), false, () => Player.Instance.gameObject, getDmg, skillTrans));
    }
    private void addSkills6() {
        EnemyTimer rmvOnPlayer = EnemyTimer.Create(() => true, 6f, -1, skillTrans);
        rmvOnPlayer.addRmvSkill((Orb o) => o.isDigit() && o.isEven());
        skillList.Add(rmvOnPlayer);

        Func<ORB_VALUE, OrbSpawnRate> decreasedEvens = (ORB_VALUE orbVal) => {
            if ((int)orbVal <= 9) return (int)orbVal % 2 == 0 ? OrbSpawnRate.DECREASED : OrbSpawnRate.INCREASED;
            return Board.getDefaultOrbSpawnRates()[(int)orbVal];
        };
        skillList.Add(EnemyOrbSkill.Create(() => true, decreasedEvens, -1, skillTrans));

        Func<bool> wtu = () => GameController.Instance.getCurrEnemies().Count == 1 ? GameController.Instance.isTurnMod(2) : false;
        EnemyBoardSkill set369ToEmpty = EnemyBoardSkill.MarkIfSkill(wtu, (Orb o) => o.getOrbValue() == ORB_VALUE.THREE || o.getOrbValue() == ORB_VALUE.SIX || o.getOrbValue() == ORB_VALUE.NINE, 0.1f, skillTrans);
        set369ToEmpty.addSetSkill(0.1f, (Orb o) => ORB_VALUE.EMPTY);
        skillList.Add(set369ToEmpty);

        EnemyBoardSkill markAll = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 1), (Orb o) => true, 0.1f, skillTrans);
        skillList.Add(markAll);
        Func<int> getDmg = () => {
            List<Orb> markedOrbs = Board.Instance.getAllMarkedOrbsBy(getSkillID(markAll, markAll.getActivatedTurn()), null);
            int sum = 0;
            foreach (Orb o in markedOrbs) if (o.isDigit()) sum += o.getIntValue();
            return sum * -6;
        };
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3, 1), false, () => Player.Instance.gameObject, getDmg, skillTrans));
    }
    private void addSkills9() {
        EnemyTimer setOnPlayer = EnemyTimer.Create(() => true, 9f, -1, skillTrans);
        setOnPlayer.addSetSkill((Orb o) => o.getOrbValue() == ORB_VALUE.ZERO ? ORB_VALUE.NINE : o.getOrbValue());
        skillList.Add(setOnPlayer);

        EnemyBoardSkill setFivesToNullify = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(4, 2), (Orb o) => o.getOrbValue() == ORB_VALUE.FIVE, 0.1f, skillTrans);
        setFivesToNullify.addSetSkill(0.1f, (Orb o) => ORB_VALUE.NULLIFY);
        skillList.Add(setFivesToNullify);

        Func<bool> wtu = () => {
            List<Enemy> eList = GameController.Instance.getCurrEnemies();
            foreach (Enemy e in eList) if (e.getState().number == 6) return false;
            return GameController.Instance.isTurnMod(4, 1);
        };
        EnemyBoardSkill setNinesToStop = EnemyBoardSkill.MarkIfSkill(wtu, (Orb o) => o.getOrbValue() == ORB_VALUE.NINE, 0.1f, skillTrans);
        setNinesToStop.addSetSkill(0.1f, (Orb o) => ORB_VALUE.STOP);
        skillList.Add(setNinesToStop);

        Func<Orb, bool> allOrbsGreaterThan = (Orb o) => {
            int range = 6 - (3 - GameController.Instance.getCurrEnemies().Count);
            return o.isDigit() && o.getIntValue() >= range;
        };
        EnemyBoardSkill markGreater = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 2), allOrbsGreaterThan, 0.1f, skillTrans);
        skillList.Add(markGreater);
        Func<int> getDmg = () => {
            List<Orb> markedOrbs = Board.Instance.getAllMarkedOrbsBy(getSkillID(markGreater, markGreater.getActivatedTurn()), null);
            int sum = 0;
            foreach (Orb o in markedOrbs) if (o.isDigit()) sum += o.getIntValue();
            return sum * -3;
        };
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.isTurnMod(3, 2), false, () => Player.Instance.gameObject, getDmg, skillTrans));
    }
}

public class MiniBossData {
    public static int getHP(int num) {
        switch (num) {
            // Floor 15
            case 16: return 4000;
            case 25: return 5000;
            case 36: return 3000;
            // Floor 30
            case 26: return 1300;
            case 27: return 1350;
            case 28: return 1400;
            // Floor 45
            case 11: return 1100;
            case 13: return 3300;
            // Floor 46
            case 17: return 1700;
            case 19: return 1900;
            // Floor 47
            case 64: return 4000;
            case 81: return 3000;
            // Floor 48
            case 89: return 666;
            case 97: return 666;
            // Floor 49
            case 3: return 9000;
            case 6: return 6000;
            case 9: return 3000;
            // Invalid number
            default: return 0;
        }
    }
    public static int getATK(int num) {
        switch (num) {
            // Floor 15
            case 16: return 64;
            case 25: return 125;
            case 36: return 216;
            // Floor 30
            case 26: return 130;
            case 27: return 135;
            case 28: return 140;
            // Other number
            default: return 0;
        }
    }
    public static string getSprite(int num) {
        switch (num) {
            // Floor 15
            case 16: return "dat_boi";
            case 25: return "dat_boi";
            case 36: return "dat_boi";
            // Floor 30
            case 26: return "dat_boi";
            case 27: return "dat_boi";
            case 28: return "dat_boi";
            // Floor 45
            case 11: return "dat_boi";
            case 13: return "dat_boi";
            // Floor 46
            case 17: return "dat_boi";
            case 19: return "dat_boi";
            // Floor 47
            case 64: return "cheems";
            case 81: return "buff_doge";
            // Floor 48
            case 89: return "dat_boi";
            case 97: return "dat_boi";
            // Floor 49
            case 3: return "dat_boi";
            case 6: return "dat_boi";
            case 9: return "dat_boi";
            // Invalid number
            default: return "dat_boi";
        }
    }
}