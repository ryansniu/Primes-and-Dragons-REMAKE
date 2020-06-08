using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : Enemy {
    public static Enemy Create() {
        return Create("Tutorial Enemy", 2, 500, 40);
    }

    public override IEnumerator Attack(Player p, Board b) {
        if (currState.currTurn % 2 == 0) {
            setBuff(EnemyBuffs.DMG_REFLECT);
            //yield return StartCoroutine(useSkill("DMG Reflect", 0.25f));  // TO-DO: default 0 for min time
        }
        else {
            setBuff(EnemyBuffs.DMG_MITI_50);
            //yield return StartCoroutine(useSkill("DMG Miti", 0.25f));
        }

        // Heal self
        if (currState.currTurn % 2 == 1) {
            targetedAnimation(true);
            //yield return StartCoroutine(useSkill("Heal", 0.25f));
            yield return StartCoroutine(takeDMG(50, p, b));
        }

        if (currState.currTurn % 3 == 0) {
            //yield return StartCoroutine(useSkill("Testing!", 1f));
            yield return StartCoroutine(b.setOrbAt(0, 2, ORB_VALUE.STOP, 0.1f));
            yield return StartCoroutine(b.setOrbAt(1, 2, ORB_VALUE.STOP, 0.1f));
            yield return StartCoroutine(b.setOrbAt(2, 2, ORB_VALUE.STOP, 0.1f));
            yield return StartCoroutine(b.setOrbAt(3, 2, ORB_VALUE.STOP, 0.1f));
            yield return StartCoroutine(b.setOrbAt(4, 2, ORB_VALUE.POISON, 0.1f));
            yield return StartCoroutine(b.setOrbAt(3, 0, ORB_VALUE.EMPTY, 0.1f));
            yield return StartCoroutine(b.setOrbAt(3, 1, ORB_VALUE.EMPTY, 0.1f));
            yield return StartCoroutine(b.setOrbAt(3, 3, ORB_VALUE.NULLIFY, 0.1f));
            yield return StartCoroutine(b.setOrbAt(3, 4, ORB_VALUE.EMPTY, 0.1f));
            yield return StartCoroutine(b.setOrbAt(5, 2, ORB_VALUE.POISON, 0.1f));
        }
        else if (currState.currTurn % 3 == 1) {
            //yield return StartCoroutine(useSkill("evens--", 1f));
            List<Orb> evens = b.getAllOrbsIf((Orb o) => { return o.isEven(); });
            yield return StartCoroutine(b.markAllOrbsIf((Orb o) => { return o.isEven(); }, 0.05f));
            foreach (Orb even in evens) {
                even.incrementValue(-1);
                even.toggleOrbMarker(false);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else {
            //yield return StartCoroutine(useSkill("odds--", 1f));
            List<Orb> odds = b.getAllOrbsIf((Orb o) => { return o.isOdd(); });
            yield return StartCoroutine(b.markAllOrbsIf((Orb o) => { return o.isOdd(); }, 0.05f));
            foreach (Orb odd in odds) {
                odd.incrementValue(1);
                odd.toggleOrbMarker(false);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Setting damage over time
        if (currState.currTurn % 3 == 1) {
            //yield return StartCoroutine(useSkill("Timer!", 0.25f));
            p.setDOT(-10);
        }
        yield return StartCoroutine(base.Attack(p, b));
    }
}
