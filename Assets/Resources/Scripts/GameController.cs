using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class GameController : MonoBehaviour {
    private int currFloor = 0;
    private EnemySpawner es = new EnemySpawner();
    private Enemy[] currEnemies;

    public Player player;
    public Board board;
    void Start() {
        StartCoroutine(TurnRoutine());
    }
    private IEnumerator TurnRoutine() {
        do {
            currFloor++;
            adjustPlayerStats();
            currEnemies = es.getEnemies(currFloor);
            adjustOrbRates();
            do {
                yield return StartCoroutine(PlayerTurn());
                yield return StartCoroutine(EnemyTurn());
            } while (player.isAlive() && livingEnemyExists());
        } while (currFloor < 50 && player.isAlive());

        if (player.isAlive() && currFloor == 50) yield return StartCoroutine(PlayerWins());
        else yield return StartCoroutine(GameOver());
    }

    private IEnumerator PlayerTurn() {
        yield return StartCoroutine(board.getInput());
        string inputNum = board.getInputNum();
        Debug.Log(inputNum);
        yield return StartCoroutine(board.clearBoard());
        //damage calculation
        BigInteger actualNum = board.parseInputNumOnly(inputNum);
        int damageDealt = calculateDamage(actualNum);
        Debug.Log("Damage: " + damageDealt);
        bool anyDMGdealt = false;
        foreach (Enemy e in currEnemies) {
            if (actualNum % e.number == 0) {
                if (!anyDMGdealt) {
                    //some damage was dealt, show green bar (board.showGreen)
                    anyDMGdealt = true;
                }
                Debug.Log("dealt damage to: " + e.number);
                e.addToHealth(-damageDealt); //deal damage to the enemy
            }
            else {
                Debug.Log("dealt NO damage to: " + e.number);  //CAN REMOVE
            }
        }
        if (!anyDMGdealt) {
            //no damage was dealt, show red bar (board.showRed)
        }
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

    private IEnumerator PlayerWins() {
        yield return null;
    }
    private IEnumerator GameOver() {
        yield return null;
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
    private bool livingEnemyExists() {
        foreach (Enemy e in currEnemies) if (e.isAlive()) return true;
        return false;
    }
}

public class EnemySpawner{
    public Enemy[] getEnemies(int floor) {
        int len = (int)Random.Range(2f, 3.99f);
        Enemy[] enemies = new Enemy[len];
        for (int i = 0; i < len; i++) {
            enemies[i] = Enemy.Create("Enemy", new UnityEngine.Vector3(0, 1, -1), (int)Random.Range(2f, 10f), 200, 100);
        }
        return enemies;
    }
}