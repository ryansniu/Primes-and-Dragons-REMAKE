using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour {
    private int currFloor = 0;
    private EnemySpawner es = new EnemySpawner();
    private List<Enemy> currEnemies;

    public TextMeshPro floorNum;
    public Player player;
    public Board board;
    void Start() {
        StartCoroutine(TurnRoutine());
    }
    private IEnumerator TurnRoutine() {
        do {
            currFloor++;
            initRound();
            do {
                yield return StartCoroutine(PlayerTurn());
                yield return StartCoroutine(EnemyTurn());
            } while (player.isAlive() && currEnemies.Count > 0);
        } while (currFloor < 50 && player.isAlive());
        if (player.isAlive() && currFloor == 50) yield return StartCoroutine(PlayerWins());
        else yield return StartCoroutine(GameOver());
    }
    
    public void initRound(){
        floorNum.text = "Floor: "+currFloor;
        adjustBackground();
        adjustPlayerStats();
        currEnemies = es.getEnemies(currFloor);
        displayEnemies();
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
    private void adjustPlayerStats() {
        int maxHealth = 400;
        if (currFloor > 0) maxHealth += 100;
        if (currFloor > 15) maxHealth += 250;
        if (currFloor > 30) maxHealth += 250;
        if (currFloor > 45) maxHealth += 500;
        if (currFloor == 50) maxHealth += 500;
        player.setMaxHealth(maxHealth);
    }

    private IEnumerator PlayerTurn() {
        yield return StartCoroutine(board.getInput());
        string inputNum = board.getInputNum(false);
        BigInteger actualNum = BigInteger.Parse(board.getInputNum(true));
        yield return StartCoroutine(board.clearBoard());
        
        //damage calculation
        int damageDealt = calculateDamage(actualNum);
        Debug.Log("Damage: " + damageDealt);
        bool anyDMGdealt = false;
        for(int i = 0; i < currEnemies.Count; i++) {
            Enemy e = currEnemies[i];
            if (actualNum % e.number == 0) {
                if (!anyDMGdealt) {
                    board.flashNumBar(true);
                    anyDMGdealt = true;
                }
                Debug.Log("dealt damage to: " + e.number);
                e.addToHealth(-damageDealt); //deal damage to the enemy
                if(!e.isAlive()){
                    Debug.Log("enemy is dead");
                    currEnemies.Remove(e);
                    Destroy(e.gameObject);
                    i--;
                    displayEnemies();
                }
            }
        }
        if (!anyDMGdealt) board.flashNumBar(false);
        
        //heals and poison
        int amtHealed = calculateHeals(inputNum);
        Debug.Log("Heals:");
        player.addToHealth(amtHealed);
        int amtPoisoned = calculatePoison(inputNum);
        Debug.Log("Poison:");
        player.addToHealth(amtPoisoned);
        yield return null;
    }
    private int calculateDamage(BigInteger actualNum) {
        int sum = actualNum.ToString().ToCharArray().Sum(c => c - '0');
        int len = (int)Mathf.Floor((float)BigInteger.Log10(actualNum) + 1);
        return sum * len;
    }
    private int calculateHeals(string num) {
        return num.Count(c => c == '0') * 50;
    }
    private int calculatePoison(string num) {
        return num.Count(c => c == 'P') * -50;
    }

    private IEnumerator EnemyTurn() {
        foreach (Enemy e in currEnemies) yield return StartCoroutine(e.Attack(player, board));
    }
    private void displayEnemies(){
        switch(currEnemies.Count){
            case 1:
                currEnemies[0].setPosition(new UnityEngine.Vector3(0, 0.8f, -1));
                break;
            case 2:
                currEnemies[0].setPosition(new UnityEngine.Vector3(-0.4f, 0.8f, -1));
                currEnemies[1].setPosition(new UnityEngine.Vector3(0.4f, 0.8f, -1));
                break;
            case 3:
                currEnemies[0].setPosition(new UnityEngine.Vector3(-0.5f, 0.8f, -1));
                currEnemies[1].setPosition(new UnityEngine.Vector3(0, 1f, -1));
                currEnemies[2].setPosition(new UnityEngine.Vector3(0.5f, 0.8f, -1));
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
        int len = (int)Random.Range(2f, 3.99f);
        List<Enemy> enemies = new List<Enemy>();
        for (int i = 0; i < len; i++) {
            enemies.Add(Enemy.Create("Enemy", new UnityEngine.Vector3(0, 1, -1), (int)Random.Range(2f, 10f), 100, 60));
        }
        return enemies;
    }
}