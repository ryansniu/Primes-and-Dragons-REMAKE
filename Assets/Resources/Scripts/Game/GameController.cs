using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static bool isPaused = false;
    public static bool loadSaveFile = false;
    public GameStatsAndUI GSaUI;

    private EnemySpawner es = new EnemySpawner();
    private List<Enemy> currEnemies;
    public SpriteRenderer currEnemyBG;
    private Sprite[] enemyBGs;

    public Player player;
    public Board board;
    public DamageBar damageBar;
    public GameObject endAnim;

    void Awake() {
        enemyBGs = Resources.LoadAll<Sprite>("Sprites/Main Screen/Enemy Board");
    }
    void Start() {
        isPaused = false;
        if (loadSaveFile) SaveStateMonoBehaviour.Instance.SaveInstance.loadGame(ref GSaUI.currFloor, ref GSaUI.elapsedTime, ref board, ref currEnemies, ref player);
        StartCoroutine(LoadingScreen.Instance.HideDelay());
        StartCoroutine(TurnRoutine());
    }
    public void SaveGame() {
        SaveStateMonoBehaviour.Instance.SaveInstance.saveGame(GSaUI.currFloor, GSaUI.elapsedTime, board, currEnemies, player);
    }

    private IEnumerator TurnRoutine() {
        do {
            yield return StartCoroutine(initRound());
            do {
                yield return StartCoroutine(PlayerTurn());
                yield return StartCoroutine(EnemyTurn());
            } while (player.isAlive() && currEnemies.Count > 0);
            GSaUI.currFloor++;
        } while (GSaUI.currFloor <= 50 && player.isAlive());
        yield return StartCoroutine(gameEnd(player.isAlive() && GSaUI.currFloor == 50));
    }

    public IEnumerator initRound() {
        GSaUI.updateText();
        adjustBackground();
        if (!loadSaveFile) currEnemies = es.getEnemies(GSaUI.currFloor);
        else loadSaveFile = false;
        displayEnemies();
        yield return StartCoroutine(adjustPlayerStats());
        adjustOrbRates();
    }
    private void adjustBackground() {
        int currEnemyBGIndex = 0;
        if (GSaUI.currFloor > 0) currEnemyBGIndex++;
        if (GSaUI.currFloor > 15) currEnemyBGIndex++;
        if (GSaUI.currFloor > 30) currEnemyBGIndex++;
        if (GSaUI.currFloor > 45) currEnemyBGIndex++;
        if (GSaUI.currFloor == 50) currEnemyBGIndex++;
        currEnemyBG.sprite = enemyBGs[currEnemyBGIndex];
    }
    private void adjustOrbRates() {
        //currFloor = 0;  board.orbSpawnRates
    }
    private IEnumerator adjustPlayerStats() {
        int maxHealth = 40;
        if (GSaUI.currFloor > 0) maxHealth += 100;
        if (GSaUI.currFloor > 15) maxHealth += 250;
        if (GSaUI.currFloor > 30) maxHealth += 250;
        if (GSaUI.currFloor > 45) maxHealth += 500;
        if (GSaUI.currFloor == 50) maxHealth += 500;
        yield return StartCoroutine(player.setMaxHealth(maxHealth));
    }
    private IEnumerator PlayerTurn() {
        //getting input
        yield return StartCoroutine(board.toggleForeground(false));
        GSaUI.toggle(true);
        yield return StartCoroutine(board.getInput());
        GSaUI.toggle(false);
        string inputNum = board.getInputNum(false);
        BigInteger actualNum = board.getInputNum(true).Equals("") ? new BigInteger(1) : BigInteger.Parse(board.getInputNum(true));

        //checking if the input is divisible by any enemy
        bool anyDMGdealt = false;
        foreach (Enemy e in currEnemies) {
            if (actualNum % e.currState.number == 0) {
                anyDMGdealt = true;
                e.toggleFlashingRed(true);  //flashing red animaion start
            }
        }
        board.setNumBarColor(anyDMGdealt ? NUMBAR_STATE.SUCCESS : NUMBAR_STATE.FAILURE);

        //clear board while calculating damage/heals/poisons sequentially
        foreach (char c in inputNum) {
            StartCoroutine(board.rmvNextOrb());
            switch (c) {
                case 'P':
                    player.addToHealth(-50);
                    break;
                case 'E':
                    // Do nothing.
                    break;
                case '0':
                    if (anyDMGdealt) damageBar.addNextDigit(0);
                    player.addToHealth(50);
                    break;
                default:
                    if (anyDMGdealt) damageBar.addNextDigit((int)char.GetNumericValue(c));
                    break;
            }
            yield return Board.DISAPPEAR_DELTA;
        }
        if (!player.isAlive()) player.setCauseOfDeath("poison");

        //fill the board
        yield return new WaitForSeconds(Board.DISAPPEAR_DURATION);
        yield return StartCoroutine(player.resetDeltaHealth());
        yield return StartCoroutine(board.fillBoard(false));
        yield return StartCoroutine(board.toggleForeground(true));

        //deal damage to enemies
        damageBar.displayText(false);
        if (anyDMGdealt) {
            int damageDealt = damageBar.getCurrDamage();
            for (int i = 0; i < currEnemies.Count; i++) {  //deal damage to the enemy
                Enemy e = currEnemies[i];
                if (actualNum % e.currState.number == 0) {
                    yield return StartCoroutine(e.takeDMG(-damageDealt, player, board));
                    e.toggleFlashingRed(false);  //flashing red animaion end
                    if (!e.isAlive()) {
                        currEnemies.Remove(e);
                        Destroy(e.gameObject);
                        i--;
                        // TO-DO: delay here? or enemy death animation
                        displayEnemies();
                    }
                }
            }
        }
        damageBar.resetValues();
    }
    private IEnumerator EnemyTurn() {
        foreach (Enemy e in currEnemies) {
            yield return StartCoroutine(e.Attack(player, board));
        }
        yield return StartCoroutine(player.resetDeltaHealth());
        if (!player.isAlive()) player.setCauseOfDeath(currEnemies[Random.Range(0, currEnemies.Count)].currState.number.ToString());
    }
    private void displayEnemies() {
        switch (currEnemies.Count) {
            case 1:
                currEnemies[0].setPosition(new UnityEngine.Vector3(0, 100));
                break;
            case 2:
                currEnemies[0].setPosition(new UnityEngine.Vector3(-50f, 100f));
                currEnemies[1].setPosition(new UnityEngine.Vector3(50, 100f));
                break;
            case 3:
                currEnemies[0].setPosition(new UnityEngine.Vector3(-80f, 100f));
                currEnemies[1].setPosition(new UnityEngine.Vector3(0, 200f));
                currEnemies[2].setPosition(new UnityEngine.Vector3(80f, 100f));
                break;
            default:
                break;
        }
    }
    private IEnumerator gameEnd(bool win) {
        sendDataToLeaderboard();
        yield return endAnim.GetComponent<EndGameAnimation>().endGameAnimation(win);
    }

    private void sendDataToLeaderboard() {  // TO-DO: merge with gameEnd?
        PlayerPrefs.SetInt("Floor", GSaUI.currFloor);
        PlayerPrefs.SetString("Time", GSaUI.elapsedTime.ToString("R"));
        PlayerPrefs.SetString("Death", player.getCauseOfDeath());
    }
}

public class EnemySpawner {
    private System.Random rng = new System.Random();
    private int[] enemiesLvl1 = { 3, 4, 5, 6, 8, 9, 10, 12, 20, 25, 50 };
    private int[] enemiesLvl2 = { 5, 6, 7, 8, 9, 10, 14, 15, 20, 22, 24, 25, 30, 32, 40, 50 };
    private int[] enemiesLvl3 = { 7, 10, 11, 12, 14, 15, 16, 18, 21, 22, 26, 27, 30, 32, 35, 40, 45, 60 };
    public List<Enemy> getEnemies(int floor) {
        List<Enemy> enemies = new List<Enemy>();

        if (floor == 0) {
            enemies.Add(TutorialEnemy.Create());
        }
        else if (floor < 15) {
            int len = rng.Next(1, 3);
            for (int i = 0; i < len; i++) {
                int num = enemiesLvl1[rng.Next(enemiesLvl1.Length)];
                int hp = 100 + (floor - 1) * 50;
                int dmg = (60 + (floor - 1) * 3 / len);
                enemies.Add(Enemy.Create("Enemy", num, hp, dmg));
            }
        }
        else if (floor == 15) {
            enemies.Add(Enemy.Create("Enemy", 16, 4000, 64));
            enemies.Add(Enemy.Create("Enemy", 25, 5000, 125));
            enemies.Add(Enemy.Create("Enemy", 36, 6000, 216));
        }
        else if (floor < 30) {
            int len = rng.Next(1, 4);
            for (int i = 0; i < len; i++) {
                int num = enemiesLvl2[rng.Next(enemiesLvl2.Length)];
                int hp = 100 + (floor - 1) * 50;
                int dmg = (60 + (floor - 1) * 3 / len);
                enemies.Add(Enemy.Create("Enemy", num, hp, dmg));
            }
        }
        else if (floor == 30) {
            enemies.Add(Enemy.Create("Enemy", 26, 2600, 130));
            enemies.Add(Enemy.Create("Enemy", 27, 2700, 135));
            enemies.Add(Enemy.Create("Enemy", 28, 2800, 140));
        }
        else if (floor < 45) {
            int len = rng.Next(2, 4);
            for (int i = 0; i < len; i++) {
                int num = enemiesLvl3[rng.Next(enemiesLvl3.Length)];
                int hp = 100 + (floor - 1) * 50;
                int dmg = (60 + (floor - 1) * 3 / len);
                enemies.Add(Enemy.Create("Enemy", num, hp, dmg));
            }
        }
        else if (floor == 45) {
            enemies.Add(Enemy.Create("Enemy", 11, 1500, 400));
            enemies.Add(Enemy.Create("Enemy", 13, 4000, 150));
        }
        else if (floor == 46) {
            enemies.Add(Enemy.Create("Enemy", 17, 3000, 0));
            enemies.Add(Enemy.Create("Enemy", 19, 3000, 0));
        }
        else if (floor == 47) {
            enemies.Add(Enemy.Create("Enemy", 23, 2500, 200));
            enemies.Add(Enemy.Create("Enemy", 29, 2500, 200));
        }
        else if (floor == 48) {
            enemies.Add(Enemy.Create("Enemy", 15, 2000, 115));
            enemies.Add(Enemy.Create("Enemy", 21, 2000, 121));
            enemies.Add(Enemy.Create("Enemy", 35, 2000, 135));
        }
        else if (floor == 49) {
            enemies.Add(Enemy.Create("Enemy", 3, 9000, 99));
            enemies.Add(Enemy.Create("Enemy", 6, 6000, 99));
            enemies.Add(Enemy.Create("Enemy", 9, 3000, 99));
        }
        else if (floor == 50) {
            //TODO
        }
        return enemies;
    }
}