using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialEnemy : Enemy {
    public static Enemy Create() {
        return Create("Tutorial Enemy", 2, 500, 40);
    }

    public override IEnumerator Attack(Player p, Board b) {
        yield return StartCoroutine(base.Attack(p, b));
    }
}
