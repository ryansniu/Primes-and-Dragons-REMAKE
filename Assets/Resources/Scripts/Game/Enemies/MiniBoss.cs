using System;
using System.Collections;
using UnityEngine;

public class MiniBoss : Enemy {
    public static Enemy Create(int num) {
        MiniBoss mb = Create("Mini Boss", num, MiniBossData.getHP(num), MiniBossData.getATK(num)).GetComponent<MiniBoss>();
        mb.setSprite(MiniBossData.getSprite(num));
        return mb;
    }
    protected override void loadAllHPBarIMGs() { enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Mini Boss"); }
    public override void setPosition(EnemyPosition pos) {
        base.setPosition(pos);
        switch (pos) {
            case EnemyPosition.LEFT_2: case EnemyPosition.LEFT_3:
                HPtrans.anchoredPosition = new Vector3(-268f, HPtrans.anchoredPosition.y, -14f);
                break;
            case EnemyPosition.RIGHT_2: case EnemyPosition.RIGHT_3:
                HPtrans.anchoredPosition = new Vector3(267f, HPtrans.anchoredPosition.y, -14f);
                break;
            default: break;
        }
    }
    public override IEnumerator Attack(Player p, Board b) {
        yield return StartCoroutine(MiniBossData.getAttack(this, p, b));
        yield return StartCoroutine(base.Attack(p, b));
    }
}

public class MiniBossData {
    public static int getHP(int num) {
        switch (num) {
            // Floor 15
            case 16: return 400;
            case 25: return 500;
            case 36: return 600;
            // Floor 30
            case 26: return 520;
            case 27: return 540;
            case 28: return 560;
            // Floor 45
            case 11: return 1000;
            case 13: return 3000;
            // Floor 46
            case 17: return 1500;
            case 19: return 1500;
            // Floor 47
            case 23: return 2300;
            case 29: return 2900;
            // Floor 48
            case 15: return 1500;
            case 21: return 1500;
            case 35: return 1500;
            // Floor 49
            case 3: return 9000;
            case 6: return 6000;
            case 9: return 3000;
            // Floor 50a
            case 31: return 1000;
            case 38: return 1000;
            case 45: return 1000;
            // Floor 50b
            case 97: return 666;
            case 99: return 666;
            // Floor 50c
            case 2: return 42069;
            // Invalid number
            default: return 0;
        }
    }
    public static int getATK(int num) {
        switch (num) {
            // Floor 15
            case 16: return 64;
            case 25: return 125;
            case 36: return 216;
            // Floor 30
            case 26: return 130;
            case 27: return 135;
            case 28: return 140;
            // Floor 45
            case 11: return 300;
            case 13: return 100;
            // Floor 46
            case 17: return 0;  //change currState.dmg
            case 19: return 0;  //change currState.dmg
            // Floor 47
            case 23: return 230;
            case 29: return 290;
            // Floor 48
            case 15: return 115;
            case 21: return 121;
            case 35: return 135;
            // Floor 49
            case 3: return 310;
            case 6: return 380;
            case 9: return 450;
            // Floor 50a
            case 31: return 101;
            case 38: return 101;
            case 45: return 101;
            // Floor 50b
            case 97: return 97;
            case 99: return 99;
            // Floor 50c
            case 2: return 0;  //change currState.dmg
            // Invalid number
            default: return 0;
        }
    }
    public static string getSprite(int num) {
        switch (num) {
            // Floor 15
            case 16: return "dat_boi";
            case 25: return "dat_boi";
            case 36: return "dat_boi";
            // Floor 30
            case 26: return "dat_boi";
            case 27: return "dat_boi";
            case 28: return "dat_boi";
            // Floor 45
            case 11: return "dat_boi";
            case 13: return "dat_boi";
            // Floor 46
            case 17: return "dat_boi";
            case 19: return "dat_boi";
            // Floor 47
            case 23: return "cheems";
            case 29: return "buff_doge";
            // Floor 48
            case 15: return "dat_boi";
            case 21: return "dat_boi";
            case 35: return "dat_boi";
            // Floor 49
            case 3: return "dat_boi";
            case 6: return "dat_boi";
            case 9: return "dat_boi";
            // Floor 50a
            case 31: return "dat_boi";
            case 38: return "dat_boi";
            case 45: return "dat_boi";
            // Floor 50b
            case 97: return "dat_boi";
            case 99: return "dat_boi";
            // Floor 50c
            case 2: return "dat_boi";
            // Invalid number
            default: return "dat_boi";
        }
    }

    public static IEnumerator getAttack(MiniBoss e, Player p, Board b) {
        switch (e.currState.number) {
            // Floor 15
            case 16: return Attack16(e, p, b);
            case 25: return Attack25(e, p, b);
            case 36: return Attack36(e, p, b);
            // Floor 30
            case 26: return Attack25(e, p, b);
            case 27: return Attack25(e, p, b);
            case 28: return Attack25(e, p, b);
            // Floor 45
            case 11: return Attack25(e, p, b);
            case 13: return Attack25(e, p, b);
            // Floor 46
            case 17: return Attack17(e, p, b);
            case 19: return Attack19(e, p, b);
            // Floor 47
            case 23: return Attack25(e, p, b);
            case 29: return Attack25(e, p, b);
            // Floor 48
            case 15: return Attack25(e, p, b);
            case 21: return Attack25(e, p, b);
            case 35: return Attack25(e, p, b);
            // Floor 49
            case 3: return Attack25(e, p, b);
            case 6: return Attack25(e, p, b);
            case 9: return Attack25(e, p, b);
            // Floor 50a
            case 31: return Attack25(e, p, b);
            case 38: return Attack25(e, p, b);
            case 45: return Attack25(e, p, b);
            // Floor 50b
            case 97: return Attack25(e, p, b);
            case 99: return Attack25(e, p, b);
            // Floor 50c
            case 2: return Attack2(e, p, b);
            // Invalid number
            default: return null;
        }
    }

    private static IEnumerator Attack16(MiniBoss e, Player p, Board b) {
        if(e.currState.currTurn % 3 == 0) {

        }
        yield return null;
    }
    private static IEnumerator Attack25(MiniBoss e, Player p, Board b) {
        yield return null;
    }
    private static IEnumerator Attack36(MiniBoss e, Player p, Board b) {
        yield return null;
    }

    private static IEnumerator Attack17(MiniBoss e, Player p, Board b) {
        e.currState.damage = 1000 * (1 - e.currState.currHealth/e.currState.maxHealth);
        yield return null;
    }
    private static IEnumerator Attack19(MiniBoss e, Player p, Board b) {
        e.currState.damage = 100 + e.currState.currTurn * 50;
        yield return null;
    }

    private static IEnumerator Attack2(MiniBoss e, Player p, Board b) {
        e.currState.damage = (int)Math.Pow(2, e.currState.currTurn);
        yield return null;
    }
}