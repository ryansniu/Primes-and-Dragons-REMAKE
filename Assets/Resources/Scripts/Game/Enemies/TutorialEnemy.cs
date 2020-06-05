using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialEnemy : Enemy {
    public static Enemy Create() {
        return Create("Tutorial Enemy", 2, 500, 40);
    }

    public override IEnumerator Attack(Player p, Board b) {
        // Damage Reflect
        if (currState.turnCount % 2 == 0) {
            toggleStatus(EnemyStatus.DMG_MITI_50, false);
            toggleStatus(EnemyStatus.DMG_REFLECT, true);
        }
        else {
            toggleStatus(EnemyStatus.DMG_REFLECT, false);
            toggleStatus(EnemyStatus.DMG_MITI_50, true);
        }

        // Heal self
        if (currState.turnCount % 2 == 1) {
            toggleStatus(EnemyStatus.HEAL, true);
            yield return StartCoroutine(takeDMG(50, p, b));
            toggleStatus(EnemyStatus.HEAL, false);
        }
        // Setting damage over time
        if (currState.turnCount % 2 == 0) {
            p.setDOT(-10);
        }

        yield return StartCoroutine(base.Attack(p, b));
    }
}
