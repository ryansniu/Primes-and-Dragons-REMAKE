using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour {
    public static bool isPaused = false;
    private int currFloor = 13;
    private EnemySpawner es = new EnemySpawner();
    private List<Enemy> currEnemies;

    public TextMeshPro floorNum;
    public Player player;
    public Board board;
    public DamageBar damageBar;
    public Button pauseButton;

    public GameOverScreen gameOver;
    void Start() {
        StartCoroutine(TurnRoutine());
    }
    private IEnumerator TurnRoutine() {
        do {
            currFloor++;
            yield return StartCoroutine(initRound());
            do {
                yield return StartCoroutine(PlayerTurn());
                yield return StartCoroutine(EnemyTurn());
            } while (player.isAlive() && currEnemies.Count > 0);
        } while (currFloor < 50 && player.isAlive());
        if (player.isAlive() && currFloor == 50) yield return StartCoroutine(PlayerWins());
        else yield return StartCoroutine(GameOver());
    }
    
    public IEnumerator initRound(){
        floorNum.text = string.Concat("floor: ", currFloor.ToString().PadLeft(2, '0'));
        adjustBackground();
        currEnemies = es.getEnemies(currFloor);
        displayEnemies();
        yield return StartCoroutine(adjustPlayerStats());
        adjustOrbRates();
    }
    private void adjustBackground(){
        /*
        int maxHealth = 400;
        if (currFloor > 0) maxHealth += 100;
        if (currFloor > 15) maxHealth += 250;
        if (currFloor > 30) maxHealth += 250;
        if (currFloor > 45) maxHealth += 500;
        if (currFloor == 50) maxHealth += 500;
        player.setMaxHealth(maxHealth);
        */
    }
    private void adjustOrbRates() {
        //currFloor = 0;  board.orbSpawnRates
    }
    private IEnumerator adjustPlayerStats() {
        int maxHealth = 400;
        if (currFloor > 0) maxHealth += 100;
        if (currFloor > 15) maxHealth += 250;
        if (currFloor > 30) maxHealth += 250;
        if (currFloor > 45) maxHealth += 500;
        if (currFloor == 50) maxHealth += 500;
        //maxHealth = 1;
        yield return StartCoroutine(player.setMaxHealth(maxHealth));
    }

    private IEnumerator PlayerTurn() {
        //getting input
        yield return StartCoroutine(board.toggleForeground(false));
        pauseButton.interactable = true; //enable pause button
        yield return StartCoroutine(board.getInput());
        pauseButton.interactable = false; //diable pause button
        string inputNum = board.getInputNum(false);
        BigInteger actualNum = board.getInputNum(true).Equals("") ? new BigInteger(1) : BigInteger.Parse(board.getInputNum(true));

        //checking if the input is divisible by any enemy
        bool anyDMGdealt = false;
        foreach(Enemy e in currEnemies){
            if(actualNum % e.number == 0){
                anyDMGdealt = true;
                e.toggleFlashingRed(true);  //flashing red animaion start
            }
        }
        board.setNumBarColor(anyDMGdealt ? NUMBAR_STATE.SUCCESS : NUMBAR_STATE.FAILURE);

        //clear board while calculating damage/heals/poisons sequentially
        foreach(char c in inputNum){
            StartCoroutine(board.rmvNextOrb());
            switch (c) {
                case 'P':
                    player.addToHealth(-50);
                    break;
                case 'E':
                    //do nothing
                    break;
                case '0':
                    if(anyDMGdealt) damageBar.addNextDigit(0);
                    player.addToHealth(50);
                    break;
                default:
                    if(anyDMGdealt) damageBar.addNextDigit((int)char.GetNumericValue(c));
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }

        //fill the board
        yield return new WaitForSeconds(Board.DISAPPEAR_DURATION);
        yield return StartCoroutine(player.resetDeltaHealth());
        yield return StartCoroutine(board.fillBoard());
        yield return StartCoroutine(board.toggleForeground(true));

        //deal damage to enemies
        if(anyDMGdealt){
            int damageDealt = damageBar.getCurrDamage();
            for(int i = 0; i < currEnemies.Count; i++) {  //deal damage to the enemy
                Enemy e = currEnemies[i];
                if (actualNum % e.number == 0) {
                    yield return StartCoroutine(e.takeDMG(-damageDealt, player, board));
                    e.toggleFlashingRed(false);  //flashing red animaion end
                    if(!e.isAlive()){
                        currEnemies.Remove(e);
                        Destroy(e.gameObject);
                        i--;
                        displayEnemies();
                    }
                }
            }
            damageBar.resetValues();
        }
    }
    private IEnumerator EnemyTurn() {
        foreach (Enemy e in currEnemies) yield return StartCoroutine(e.Attack(player, board));
        yield return StartCoroutine(player.resetDeltaHealth());
    }
    private void displayEnemies(){
        switch(currEnemies.Count){
            case 1:
                currEnemies[0].setPosition(new UnityEngine.Vector3(0, 0.9f, -2f));
                break;
            case 2:
                currEnemies[0].setPosition(new UnityEngine.Vector3(-0.4f, 0.9f, -2f));
                currEnemies[1].setPosition(new UnityEngine.Vector3(0.4f, 0.9f, -2f));
                break;
            case 3:
                currEnemies[0].setPosition(new UnityEngine.Vector3(-0.585f, 0.9f, -2f));
                currEnemies[1].setPosition(new UnityEngine.Vector3(0, 1.05f, -2f));
                currEnemies[2].setPosition(new UnityEngine.Vector3(0.585f, 0.9f, -2f));
                break;
            default:
                break;
        }
    }
    private IEnumerator PlayerWins() {
        yield return null;
    }
    private IEnumerator GameOver() {
        yield return StartCoroutine(gameOver.gameOverAnimation());
    }
}

public class EnemySpawner{
    private System.Random rng = new System.Random();
    private int[] enemiesLvl1 = {3,4,5,6,8,9,10,12,20,25,50};
    private int[] enemiesLvl2 = {5,6,7,8,9,10,14,15,20,22,24,25,30,32,40,50};
    private int[] enemiesLvl3 = {7,10,11,12,14,15,16,18,21,22,26,27,30,32,35,40,45,60};
    public List<Enemy> getEnemies(int floor) {
        List<Enemy> enemies = new List<Enemy>();
        UnityEngine.Vector3 enemySpawnPos = new UnityEngine.Vector3(0, 1, -1);

        if(floor == 0) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 2, 500, 20));
        }
        else if (floor < 15) {
            int len = rng.Next(1, 3);
            for (int i = 0; i < len; i++) {
                int num = enemiesLvl1[rng.Next(enemiesLvl1.Length)];
                int hp = 100 + (floor - 1) * 50;
                int dmg = (57 + floor * 3 / len);
                enemies.Add(Enemy.Create("Enemy", enemySpawnPos, num, hp, dmg));
            }
        }
        else if (floor == 15) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 16, 4000, 64));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 25, 5000, 125));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 36, 6000, 216));
        }
        else if (floor < 30) {
            int len = rng.Next(1, 4);
            for (int i = 0; i < len; i++) {
                int num = enemiesLvl2[rng.Next(enemiesLvl2.Length)];
                int hp = 100 + (floor - 1) * 50;
                int dmg = (57 + floor * 3 / len);
                enemies.Add(Enemy.Create("Enemy", enemySpawnPos, num, hp, dmg));
            }
        }
        else if (floor == 30) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 26, 2600, 130));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 27, 2700, 135));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 28, 2800, 140));
        }
        else if (floor < 45) {
            int len = rng.Next(2, 4);
            for (int i = 0; i < len; i++) {
                int num = enemiesLvl3[rng.Next(enemiesLvl3.Length)];
                int hp = 100 + (floor - 1) * 50;
                int dmg = (57 + floor * 3 / len);
                enemies.Add(Enemy.Create("Enemy", enemySpawnPos, num, hp, dmg));
            }
        }
        else if (floor == 45) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 11, 1500, 400));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 13, 4000, 150));
        }
        else if (floor == 46) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 17, 3000, 0));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 19, 3000, 0));
        }
        else if (floor == 47) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 23, 2500, 200));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 29, 2500, 200));
        }
        else if (floor == 48) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 15, 2000, 115));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 21, 2000, 121));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 35, 2000, 135));
        }
        else if (floor == 49) {
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 3, 9000, 99));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 6, 6000, 99));
            enemies.Add(Enemy.Create("Enemy", enemySpawnPos, 9, 3000, 99));
        }
        else if (floor == 50) {
            //TODO
        }
        return enemies;
    }
}