using System.Collections;
using UnityEngine;

public class TutorialEnemy : Enemy
{
    public static Enemy Create() {
        return Create("TutorialEnemy", 2, 500, 20);
    }

    public override IEnumerator Attack(Player p, Board b) {
        if (currState.turnCount % 2 == 1) {
            b.setOrb(0, 2, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(1, 2, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(2, 2, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 2, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(4, 2, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 0, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 1, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 3, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(3, 4, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
            b.setOrb(5, 2, ORB_VALUE.EMPTY);
            yield return new WaitForSeconds(0.1f);
        }
        else {
            float[] twoSpawnRate = new float[12];
            twoSpawnRate[2] = 1f;
            b.setOrbSpawnRates(twoSpawnRate);
            Vector2[] test = {new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0), new Vector2(4, 0), new Vector2(5, 0) };
            yield return StartCoroutine(b.rmvAllOrbs(test));
            b.setDefaultOrbSpawnRates();
        }
        yield return StartCoroutine(base.Attack(p, b));
    }

    public override IEnumerator takeDMG(int dmg, Player p, Board b) {
        yield return StartCoroutine(base.takeDMG(dmg, p, b));
    }
}
