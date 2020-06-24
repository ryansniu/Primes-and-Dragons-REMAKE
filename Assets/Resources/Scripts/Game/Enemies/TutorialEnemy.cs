using System;
using UnityEngine;

public class TutorialEnemy : Enemy {
    public static Enemy Create() {
        return Create("Tutorial Enemy", 2, 500, 40, "tv1");
    }
    protected override void addAllSkills() {
        skillList.Add(EnemyAttack.Create(() => GameController.Instance.getState().turnCount % 2 == 0, true, () => gameObject, () => 50, skillTrans));
        skillList.Add(EnemyHPBuff.Create(() => GameController.Instance.getState().turnCount % 3 == 0, EnemyBuffs.DMG_REFLECT, 2, skillTrans));
        skillList.Add(EnemyTimer.Create(() => GameController.Instance.getState().turnCount % 3 == 1, () => -GameController.Instance.getState().turnCount, skillTrans));

        OrbSpawnRate[] osr = Board.getDefaultOrbSpawnRates();
        for (int i = 0; i < 10; i++) if (i % 2 == 0) osr[i] = OrbSpawnRate.NONE;
        skillList.Add(EnemyOrbSkill.Create(() => GameController.Instance.getState().turnCount % 3 == 2, osr, 2, skillTrans));

        Func<Orb, bool> isEven = (Orb o) => o.getIntValue() % 2 == 0;
        EnemyBoardSkill test = EnemyBoardSkill.MarkIfSkill(() => GameController.Instance.getState().turnCount % 3 == 0, getUniqueID(), isEven, 0.1f, skillTrans, 2);
        test.addIncSkill(0.2f, (Orb o) => -1);
        skillList.Add(test);
        base.addAllSkills();
    }
}
