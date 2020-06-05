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

        // Setting orb values
        if (currState.turnCount % 2 == 0) {
            yield return StartCoroutine(useSkill("Testing!", 1f));
            b.setOrb(0, 2, ORB_VALUE.STOP);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(1, 2, ORB_VALUE.STOP);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(2, 2, ORB_VALUE.STOP);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 2, ORB_VALUE.STOP);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(4, 2, ORB_VALUE.POISON);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 0, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 1, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 3, ORB_VALUE.NULLIFY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 4, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(5, 2, ORB_VALUE.POISON);
            yield return new WaitForSeconds(0.1f);
        }
        else {  //clearing orbs and modifying orb spawn rates
            yield return StartCoroutine(useSkill("Clearing!", 0.5f));
            float[] twoSpawnRate = new float[12];
            twoSpawnRate[2] = 1f;
            b.setOrbSpawnRates(twoSpawnRate);
            Vector2[] test = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0), new Vector2(4, 0), new Vector2(5, 0) };
            yield return StartCoroutine(b.rmvAllOrbs(test));
            b.setDefaultOrbSpawnRates();
        }

        // Incrementing and decrementing orbs
        if(currState.turnCount % 2 == 0) {
            List<Orb> evens = b.getAllOrbsIf((Orb o) => { return o.getValue() % 2 == 0 && o.getValue() < 10; });
            b.markAllOrbs(evens, true);
            foreach(Orb even in evens) {
                even.incrementValue(-1);
                even.toggleOrbMarker(false);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else { 
            List<Orb> odds = b.getAllOrbsIf((Orb o) => { return o.getValue() % 2 == 1 && o.getValue() < 10; });
            b.markAllOrbs(odds, true);
            foreach (Orb odd in odds) {
                odd.incrementValue(1);
                odd.toggleOrbMarker(false);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Setting damage over time
        if (currState.turnCount % 2 == 0) {
            p.setDOT(-10);
        }

        yield return StartCoroutine(base.Attack(p, b));
    }

    public override IEnumerator takeDMG(int dmg, Player p, Board b) {
        yield return StartCoroutine(base.takeDMG(dmg, p, b));
    }
}
