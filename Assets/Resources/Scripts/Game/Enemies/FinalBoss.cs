using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : Enemy {
    private List<Vector2Int> specialSkillInfo = new List<Vector2Int>();
    public static Enemy Create() {
        return Create("Final Boss", 2, 9999, 222, "dummy");
    }
    protected override void loadAllHPBarIMGs() { enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Final Boss"); }
    protected override void addAllSkills() {
        EnemyOrbSkill equalRates = EnemyOrbSkill.Create(() => true, (ORB_VALUE o) => o <= ORB_VALUE.NINE ? OrbSpawnRate.NORMAL : OrbSpawnRate.DECREASED, -1, skillTrans);
        skillList.Add(equalRates);

        EnemyTimer evenGrowth = EnemyTimer.Create(() => true, 2f, -1, skillTrans);
        evenGrowth.addDOTSkill(() => -2 * (int)GameController.Instance.getState().timeOnTurn);
        skillList.Add(evenGrowth);

        EnemyHPBuff randHPBuff = EnemyHPBuff.Create(() => GameController.Instance.isTurnMod(4), default, () => this, 2, skillTrans);
        randHPBuff.toggleIsRandom(true);
        skillList.Add(randHPBuff);

        EnemyBoardSkill healsToNonDigit = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.isTurnMod(3, 2), (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans);
        healsToNonDigit.addSetSkill(0f, (Orb o) => (ORB_VALUE)(10 + RNG.Next(4)));
        skillList.Add(healsToNonDigit);

        EnemyBoardSkill randShuffle = EnemyBoardSkill.ShuffleSkill(() => RNG.Next(2) == 0 && !GameController.Instance.isTurnMod(3), 16, 0.04f, skillTrans);
        skillList.Add(randShuffle);

        FinalBossSkill fbSkill = FinalBossSkill.CreateFinal(skillTrans);
        skillList.Add(fbSkill);

        EnemyBoardSkill clearAllZeros = EnemyBoardSkill.MarkIfSkill(() => fbSkill.rmvZero, (Orb o) => o.getOrbValue() == ORB_VALUE.ZERO, 0.1f, skillTrans);
        clearAllZeros.addRmvSkill(0f);
        skillList.Add(clearAllZeros);

        EnemyTimer decRandDigit = EnemyTimer.Create(() => fbSkill.decRand, 2f, 3, skillTrans);
        decRandDigit.addIncSkill((Orb o) => -1);
        skillList.Add(decRandDigit);

        EnemyOrbSkill noneEvens = EnemyOrbSkill.Create(() => fbSkill.noneEven, (ORB_VALUE o) => fbSkill.evensToMakeNone.Contains(o) ? OrbSpawnRate.NONE : OrbSpawnRate.NORMAL, 3, skillTrans);
        skillList.Add(noneEvens);

        EnemyTimer healPoisonSwap = EnemyTimer.Create(() => fbSkill.swapPoison, 2f, 3, skillTrans);
        healPoisonSwap.addSetSkill((Orb o) => {
            switch (o.getOrbValue()) {
                case ORB_VALUE.ZERO: return ORB_VALUE.POISON;
                case ORB_VALUE.POISON: return ORB_VALUE.ZERO;
                default: return o.getOrbValue();
            }
        });
        skillList.Add(healPoisonSwap);

        EnemyTimer digitToEmpty = EnemyTimer.Create(() => fbSkill.setEmpty, 2f, 3, skillTrans);
        digitToEmpty.addSetSkill((Orb o) => ORB_VALUE.EMPTY);
        skillList.Add(digitToEmpty);

        EnemyTimer randClear = EnemyTimer.Create(() => fbSkill.clearRand, 2f, 3, skillTrans);
        randClear.addRmvSkill((Orb o) => o.isDigit());
        skillList.Add(randClear);

        EnemyAttack halvePlayerHP = EnemyAttack.Create(() => GameController.Instance.isTurnMod(2), false, () => Player.Instance.gameObject, () => (int)(-Player.Instance.getState().currHealth / 2), skillTrans);
        skillList.Add(halvePlayerHP);
    }
}

public class FinalBossSkill : EnemySkill {
    private string skillID = null;
    public bool rmvZero, decRand, noneEven, swapPoison, setEmpty, clearRand;
    public List<ORB_VALUE> evensToMakeNone = new List<ORB_VALUE>();

    private WaitForSeconds setDelay = new WaitForSeconds(0.1f);
    public static FinalBossSkill CreateFinal(Transform parent) {
        FinalBossSkill fbSkill = Create(parent).AddComponent<FinalBossSkill>();
        fbSkill.initValues(EnemySkillType.MARK, () => true, 3, 0);
        return fbSkill;
    }
    public override string getSkillText(bool useFirstSkill) {
        return "FINAL";
    }

    public override IEnumerator onActivate(Enemy e) {
        yield return StartCoroutine(base.onActivate(e));
        skillID = e.getSkillID(this, activatedTurn);
        for (int c = 0; c < Board.COLUMNS; c++) {
            yield return StartCoroutine(Board.Instance.markOrbAt(c, 0, skillID, 0f));
            Orb o = Board.Instance.getOrb(c, 0);
            List<Vector2Int> markOrder = new List<Vector2Int>();
            switch (o.getOrbValue()) {
                case ORB_VALUE.ZERO:
                    rmvZero = true;
                    yield return StartCoroutine(e.takeDMG(2222));
                    break;
                case ORB_VALUE.ONE: case ORB_VALUE.THREE: case ORB_VALUE.FIVE: case ORB_VALUE.SEVEN: case ORB_VALUE.NINE:
                    decRand = true;
                    yield return StartCoroutine(Board.Instance.markAllOrbsIf(skillID + "ODD", (Orb temp) => temp.getOrbValue() == o.getOrbValue(), 0.1f));
                    yield return StartCoroutine(Board.Instance.incrementAllMarkedOrbsBy(skillID + "ODD", (Orb temp) => -1, 0f, null));
                    break;
                case ORB_VALUE.TWO: case ORB_VALUE.FOUR: case ORB_VALUE.SIX: case ORB_VALUE.EIGHT:
                    noneEven = true;
                    evensToMakeNone.Add(o.getOrbValue());
                    yield return StartCoroutine(Board.Instance.markAllOrbsIf(skillID + "EVEN", (Orb temp) => temp.isEven() && temp.getOrbValue() != ORB_VALUE.ZERO, 0.1f));
                    yield return StartCoroutine(Board.Instance.removeAllMarkedOrbsBy(skillID + "EVEN", 0f));
                    break;
                case ORB_VALUE.POISON:
                    swapPoison = true;
                    for (int r = 0; r < Board.ROWS; r++) markOrder.Add(new Vector2Int(c, r));
                    yield return StartCoroutine(Board.Instance.markOrbsInOrder(skillID + "POISON", markOrder, 0.1f));
                    yield return StartCoroutine((Board.Instance.setAllMarkedOrbsBy(skillID + "POISON", (Orb temp) => ORB_VALUE.POISON, 0f, markOrder)));
                    Board.Instance.unmarkAllOrbsBy(skillID + "POISON");
                    o.toggleOrbMarker(skillID, false);
                    break;
                case ORB_VALUE.EMPTY:
                    setEmpty = true;
                    for (int r = 0; r < Board.ROWS; r++) {
                        if (c - r >= 0) markOrder.Add(new Vector2Int(c - r, r));
                        markOrder.Add(new Vector2Int(c, r));
                        if (c + r < Board.COLUMNS) markOrder.Add(new Vector2Int(c + r, r));
                    }
                    yield return StartCoroutine(Board.Instance.markOrbsInOrder(skillID + "EMPTY", markOrder, 0.1f));
                    yield return StartCoroutine((Board.Instance.setAllMarkedOrbsBy(skillID + "EMPTY", (Orb temp) => ORB_VALUE.EMPTY, 0f, markOrder)));
                    Board.Instance.unmarkAllOrbsBy(skillID + "EMPTY");
                    o.toggleOrbMarker(skillID, false);
                    break;
                case ORB_VALUE.STOP:
                    clearRand = true;
                    for (int r = 0; r < Board.ROWS; r++) markOrder.Add(new Vector2Int(c, r));
                    yield return StartCoroutine(Board.Instance.markOrbsInOrder(skillID + "STOP", markOrder, 0.1f));
                    yield return StartCoroutine((Board.Instance.setAllMarkedOrbsBy(skillID + "STOP", (Orb temp) => ORB_VALUE.STOP, 0f, markOrder)));
                    Board.Instance.unmarkAllOrbsBy(skillID + "STOP");
                    o.toggleOrbMarker(skillID, false);
                    break;
                case ORB_VALUE.NULLIFY:
                    yield return StartCoroutine(e.takeDMG(-e.getState().currHealth / 2));
                    break;
            }
            yield return setDelay;
        }
        yield return StartCoroutine(Board.Instance.removeAllMarkedOrbsBy(skillID, 0.1f));
    }
    public override void onDestroy(Enemy e) {
        rmvZero = decRand = noneEven = swapPoison = setEmpty = clearRand = false;
        evensToMakeNone.Clear();
        base.onDestroy(e);
    }
}
