using System;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner {
    private Random RNG = new Random();
    public const int MAX_SKILLS_PER_FLOOR = 5;
    public List<Enemy> getEnemies(int floor) {
        List<Enemy> enemies = new List<Enemy>();

        if (floor == 0) enemies.Add(TutorialEnemy.Create());
        else if (floor == 15) {
            enemies.Add(MiniBoss.Create(16));
            enemies.Add(MiniBoss.Create(25));
            enemies.Add(MiniBoss.Create(36));
        }
        else if (floor == 30) {
            enemies.Add(MiniBoss.Create(26));
            enemies.Add(MiniBoss.Create(27));
            enemies.Add(MiniBoss.Create(28));
        }
        else if (floor == 45) {
            enemies.Add(MiniBoss.Create(11));
            enemies.Add(MiniBoss.Create(13));
        }
        else if (floor == 46) {
            enemies.Add(MiniBoss.Create(17));
            enemies.Add(MiniBoss.Create(19));
        }
        else if (floor == 47) {
            enemies.Add(MiniBoss.Create(64));
            enemies.Add(MiniBoss.Create(81));
        }
        else if (floor == 48) {
            enemies.Add(MiniBoss.Create(89));
            enemies.Add(MiniBoss.Create(97));
        }
        else if (floor == 49) {
            enemies.Add(MiniBoss.Create(3));
            enemies.Add(MiniBoss.Create(6));
            enemies.Add(MiniBoss.Create(9));
        }
        else if (floor == 50) enemies.Add(FinalBoss.Create());
        else addNormalEnemies(floor, ref enemies);
        return enemies;
    }

    private void addNormalEnemies(int floor, ref List<Enemy> enemies) {
        int numEnemies = RNG.Next(0) + 1;
        List<int> easySkills = new List<int>(), medSkills = new List<int>(), hardSkills = new List<int>(), remainingEasySkills = new List<int>(), remainingMedSkills = new List<int>(), remainingHardSkills = new List<int>();
        for (int i = 0; i < (floor < 15 ? 3 : (floor < 30 ? 8 : 10)); i++) { remainingEasySkills.Add(i); remainingMedSkills.Add(i); remainingHardSkills.Add(i); }
        for (int i = 0; i < numEnemies; i++) enemies.Add(NormalEnemy.Create(floor, numEnemies));

        switch (numEnemies) {
            case 1:
                addRandomSkill(ref remainingHardSkills, ref hardSkills);
                addRandomSkill(ref remainingHardSkills, ref hardSkills);
                addRandomSkill(ref remainingMedSkills, ref medSkills);
                addRandomSkill(ref remainingEasySkills, ref easySkills);
                if (RNG.Next(2) == 0) addRandomSkill(ref remainingMedSkills, ref medSkills);
                else addRandomSkill(ref remainingEasySkills, ref easySkills);
                ((NormalEnemy)enemies[0]).setSkills(easySkills, medSkills, hardSkills);
                break;
            case 2:
                addRandomSkill(ref remainingHardSkills, ref hardSkills);
                addRandomSkill(ref remainingMedSkills, ref medSkills);
                ((NormalEnemy)enemies[0]).setSkills(easySkills, medSkills, hardSkills);

                easySkills = new List<int>(); medSkills = new List<int>(); hardSkills = new List<int>();
                addRandomSkill(ref remainingMedSkills, ref medSkills);
                addRandomSkill(ref remainingEasySkills, ref easySkills);
                addRandomSkill(ref remainingEasySkills, ref easySkills);
                ((NormalEnemy)enemies[1]).setSkills(easySkills, medSkills, hardSkills);
                break;
            case 3:
                addRandomSkill(ref remainingHardSkills, ref hardSkills);
                ((NormalEnemy)enemies[0]).setSkills(easySkills, medSkills, hardSkills);

                easySkills = new List<int>(); medSkills = new List<int>(); hardSkills = new List<int>();
                addRandomSkill(ref remainingMedSkills, ref medSkills);
                addRandomSkill(ref remainingEasySkills, ref easySkills);
                ((NormalEnemy)enemies[1]).setSkills(easySkills, medSkills, hardSkills);

                easySkills = new List<int>(); medSkills = new List<int>(); hardSkills = new List<int>();
                addRandomSkill(ref remainingEasySkills, ref easySkills);
                addRandomSkill(ref remainingEasySkills, ref easySkills);
                ((NormalEnemy)enemies[2]).setSkills(easySkills, medSkills, hardSkills);
                break;
        }
        enemies = enemies.OrderBy(a => RNG.Next()).ToList();
    }
    private void addRandomSkill(ref List<int> remainingSkills, ref List<int> skillList) {
        int temp = RNG.Next(remainingSkills.Count);
        skillList.Add(remainingSkills[temp]);
        remainingSkills.RemoveAt(temp);
    }
}
