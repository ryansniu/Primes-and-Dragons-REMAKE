using System;
using System.Collections;
using UnityEngine;

public class FinalBoss : Enemy {
    public static Enemy Create() {
        return Create("Final Boss", 2, 42069, 222, "dummy");
    }
    protected override void loadAllHPBarIMGs() { enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Final Boss"); }
    protected override void addAllSkills() {
        EnemyOrbSkill equalRates = EnemyOrbSkill.Create(() => true, (ORB_VALUE o) => OrbSpawnRate.NORMAL, -1, skillTrans);
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

        EnemyAttack halveHP = EnemyAttack.Create(() => GameController.Instance.isTurnMod(2), false, () => Player.Instance.gameObject, () => (int)(-Player.Instance.getState().currHealth / 2), skillTrans);
        skillList.Add(halveHP);
    }
}
