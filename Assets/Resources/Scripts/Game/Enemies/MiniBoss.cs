using System.Collections;
using UnityEngine;

public class MiniBoss : Enemy {
    public static Enemy Create(int num) {
        MiniBoss mb = Create("Mini Boss", num, MiniBossData.getHP(num), MiniBossData.getATK(num)).GetComponent<MiniBoss>();
        mb.setSprite(MiniBossData.getSprite(num));
        return mb;
    }

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

    public override IEnumerator takeDMG(int dmg, Player p, Board b) {
        yield return StartCoroutine(MiniBossData.getTakeDMG(this, dmg, p, b));
        yield return StartCoroutine(base.takeDMG(dmg, p, b));
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
            case 16: return "dummy";
            case 25: return "dummy";
            case 36: return "dummy";
            // Floor 30
            case 26: return "dummy";
            case 27: return "dummy";
            case 28: return "dummy";
            // Floor 45
            case 11: return "dummy";
            case 13: return "dummy";
            // Floor 46
            case 17: return "dummy";
            case 19: return "dummy";
            // Floor 47
            case 23: return "cheems";
            case 29: return "buff_doge";
            // Floor 48
            case 15: return "dummy";
            case 21: return "dummy";
            case 35: return "dummy";
            // Floor 49
            case 3: return "dummy";
            case 6: return "dummy";
            case 9: return "dummy";
            // Floor 50a
            case 31: return "dummy";
            case 38: return "dummy";
            case 45: return "dummy";
            // Floor 50b
            case 97: return "dummy";
            case 99: return "dummy";
            // Floor 50c
            case 2: return "dummy";
            // Invalid number
            default: return "dummy";
        }
    }

    public static IEnumerator getAttack(MiniBoss e, Player p, Board b) {
        switch (e.currState.number) {
            // Floor 15
            case 16: return Attack16(e, p, b);
            case 25: return null;
            case 36: return null;
            // Floor 30
            case 26: return null;
            case 27: return null;
            case 28: return null;
            // Floor 45
            case 11: return null;
            case 13: return null;
            // Floor 46
            case 17: return null;
            case 19: return null;
            // Floor 47
            case 23: return null;
            case 29: return null;
            // Floor 48
            case 15: return null;
            case 21: return null;
            case 35: return null;
            // Floor 49
            case 3: return null;
            case 6: return null;
            case 9: return null;
            // Floor 50a
            case 31: return null;
            case 38: return null;
            case 45: return null;
            // Floor 50b
            case 97: return null;
            case 99: return null;
            // Floor 50c
            case 2: return null;
            // Invalid number
            default: return null;
        }
    }

    public static IEnumerator getTakeDMG(MiniBoss e, int dmg, Player p, Board b) {
        switch (e.currState.number) {
            // Floor 15
            case 16: return TakeDMG16(e, dmg, p, b);
            case 25: return null;
            case 36: return null;
            // Floor 30
            case 26: return null;
            case 27: return null;
            case 28: return null;
            // Floor 45
            case 11: return null;
            case 13: return null;
            // Floor 46
            case 17: return null;
            case 19: return null;
            // Floor 47
            case 23: return null;
            case 29: return null;
            // Floor 48
            case 15: return null;
            case 21: return null;
            case 35: return null;
            // Floor 49
            case 3: return null;
            case 6: return null;
            case 9: return null;
            // Floor 50a
            case 31: return null;
            case 38: return null;
            case 45: return null;
            // Floor 50b
            case 97: return null;
            case 99: return null;
            // Floor 50c
            case 2: return null;
            // Invalid number
            default: return null;
        }
    }

    private static IEnumerator Attack16(MiniBoss e, Player p, Board b) {
        yield return null;
    }
    private static IEnumerator TakeDMG16(MiniBoss e, int dmg, Player p, Board b) {
        yield return null;
    }
}