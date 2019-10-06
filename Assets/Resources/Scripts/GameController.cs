using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour {
    public static bool isPaused = false;
    private int currFloor = 0;
    private EnemySpawner es = new EnemySpawner();
    private List<Enemy> currEnemies;

    public TextMeshPro floorNum;
    public Player player;
    public Board board;
    public DamageBar damageBar;
    public Button pauseButton;
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
                    yield return StartCoroutine(e.addToHealth(-damageDealt));
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
        yield return null;
    }
}

public class EnemySpawner{
    public List<Enemy> getEnemies(int floor) {
        int len = (int)Random.Range(1f, 3.99f);
        List<Enemy> enemies = new List<Enemy>();
        for (int i = 0; i < len; i++) {
            enemies.Add(Enemy.Create("Enemy", new UnityEngine.Vector3(0, 1, -1), (int)Random.Range(1f, 10f) + floor, 100 + (floor - 1) * 50, (57 + floor * 3 / len)));
        }
        return enemies;
    }
}