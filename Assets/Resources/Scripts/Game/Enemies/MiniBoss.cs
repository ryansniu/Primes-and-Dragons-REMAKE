using UnityEngine;

public class MiniBoss : Enemy {
    public static Enemy Create(int num) => Create("Mini Boss", num, MiniBossData.getHP(num), MiniBossData.getATK(num), MiniBossData.getSprite(num)).GetComponent<MiniBoss>();
    
    protected override void loadAllHPBarIMGs() => enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Mini Boss");
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

    protected override void addAllSkills() {
        switch (currState.number) {
            // Floor 15
            case 16: addSkills16(); break;
            case 25: addSkills25(); break;
            case 36: addSkills36(); break;
            // Floor 30
            case 26: addSkills26(); break;
            case 27: addSkills27(); break;
            case 28: addSkills28(); break;
            // Floor 45
            case 11: addSkills11(); break;
            case 13: addSkills13(); break;
            // Floor 46
            case 17: addSkills17(); break;
            case 19: addSkills19(); break;
            // Floor 47
            case 23: addSkills23(); break;
            case 29: addSkills29(); break;
            // Floor 48
            case 15: addSkills15(); break;
            case 21: addSkills21(); break;
            case 35: addSkills35(); break;
            // Floor 49
            case 3: addSkills3(); break;
            case 6: addSkills6(); break;
            case 9: addSkills9(); break;
            // Invalid number
            default: base.addAllSkills(); break;
        }
    }
    private void addSkills16() {
        base.addAllSkills();
    }
    private void addSkills25() {
        base.addAllSkills();
    }
    private void addSkills36() {
        base.addAllSkills();
    }
    private void addSkills26() {
        base.addAllSkills();
    }
    private void addSkills27() {
        base.addAllSkills();
    }
    private void addSkills28() {
        base.addAllSkills();
    }
    private void addSkills11() {
        base.addAllSkills();
    }
    private void addSkills13() {
        base.addAllSkills();
    }
    private void addSkills17() {
        skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills19() {
        skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    }
    private void addSkills23() {
        base.addAllSkills();
    }
    private void addSkills29() {
        base.addAllSkills();
    }
    private void addSkills15() {
        base.addAllSkills();
    }
    private void addSkills21() {
        base.addAllSkills();
    }
    private void addSkills35() {
        base.addAllSkills();
    }
    private void addSkills3() {
        base.addAllSkills();
    }
    private void addSkills6() {
        base.addAllSkills();
    }
    private void addSkills9() {
        base.addAllSkills();
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
            // Invalid number
            default: return "dat_boi";
        }
    }
}