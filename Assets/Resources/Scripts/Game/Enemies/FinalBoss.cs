using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : Enemy {
    public static Enemy Create() {
        return Create("Final Boss", 2, 42069, 222);
    }

    public override IEnumerator Attack(Player p, Board b) {
        yield return StartCoroutine(base.Attack(p, b));
    }

    public override IEnumerator takeDMG(int dmg, Player p, Board b) {
        yield return StartCoroutine(base.takeDMG(dmg, p, b));
    }
}
