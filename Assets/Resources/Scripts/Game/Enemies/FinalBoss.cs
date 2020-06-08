using System.Collections;
using UnityEngine;

public class FinalBoss : Enemy {
    public static Enemy Create() {
        return Create("Final Boss", 2, 42069, 222);
    }
    protected override void loadAllHPBarIMGs() { enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Final Boss"); }
    public override IEnumerator Attack(Player p, Board b) {
        yield return StartCoroutine(base.Attack(p, b));
    }
}
