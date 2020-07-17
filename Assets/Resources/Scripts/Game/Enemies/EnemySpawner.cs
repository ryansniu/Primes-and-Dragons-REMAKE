using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        int numEnemies = RNG.Next(3) + 1;
        List<int> remainingEasySkills = new List<int>(), remainingMedSkills = new List<int>(), remainingHardSkills = new List<int>();
        for (int i = 0; i < (floor < 15 ? 3 : (floor < 30 ? 8 : 10)); i++) { remainingEasySkills.Add(i); remainingMedSkills.Add(i); remainingHardSkills.Add(i); }
        for (int i = 0; i < numEnemies; i++) enemies.Add(NormalEnemy.Create(floor, numEnemies));
        if (floor < 15) {
            switch (numEnemies) {
                case 1:
                    setNormalSkills((NormalEnemy)enemies[0], 1, 1, 1, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
                case 2:
                    setNormalSkills((NormalEnemy)enemies[0], 2, 0, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[1], 0, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
                case 3:
                    setNormalSkills((NormalEnemy)enemies[0], 1, 0, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[1], 1, 0, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[2], 1, 0, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
            }
        }
        else if (floor < 30) {
            switch (numEnemies) {
                case 1:
                    if(RNG.Next(2) == 0) setNormalSkills((NormalEnemy)enemies[0], 2, 2, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    else setNormalSkills((NormalEnemy)enemies[0], 1, 2, 1, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
                case 2:
                    setNormalSkills((NormalEnemy)enemies[0], 1, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[1], 1, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
                case 3:
                    setNormalSkills((NormalEnemy)enemies[0], 2, 0, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[1], 0, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[2], 0, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
            }
        }
        else if (floor < 45) {
            switch (numEnemies) {
                case 1:
                    if (RNG.Next(2) == 0) setNormalSkills((NormalEnemy)enemies[0], 1, 2, 2, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    else setNormalSkills((NormalEnemy)enemies[0], 2, 1, 2, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
                case 2:
                    setNormalSkills((NormalEnemy)enemies[0], 2, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[1], 0, 1, 1, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
                case 3:
                    setNormalSkills((NormalEnemy)enemies[0], 0, 0, 1, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[1], 2, 0, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    setNormalSkills((NormalEnemy)enemies[2], 1, 1, 0, ref remainingEasySkills, ref remainingMedSkills, ref remainingHardSkills);
                    break;
            }
        }
        enemies = enemies.OrderBy(a => RNG.Next()).ToList();
    }
    private void setNormalSkills(NormalEnemy ne, int numEasy, int numMed, int numHard, ref List<int> remainingEasySkills, ref List<int> remainingMedSkills, ref List<int> remainingHardSkills) {
        List<int> easySkills = new List<int>(), medSkills = new List<int>(), hardSkills = new List<int>();
        addRandomSkills(ref remainingEasySkills, ref easySkills, numEasy);
        addRandomSkills(ref remainingMedSkills, ref medSkills, numMed);
        addRandomSkills(ref remainingHardSkills, ref hardSkills, numHard);
        ne.setSkills(easySkills, medSkills, hardSkills);
    }
    private void addRandomSkills(ref List<int> remainingSkills, ref List<int> skillList, int numSkills) {
        for(; numSkills > 0 && remainingSkills.Count > 0; numSkills--) {
            int temp = RNG.Next(remainingSkills.Count);
            skillList.Add(remainingSkills[temp]);
            remainingSkills.RemoveAt(temp);
        }
    }
}
