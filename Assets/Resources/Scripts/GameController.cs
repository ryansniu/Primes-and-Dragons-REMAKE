using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameController : MonoBehaviour {
    private int currFloor = 0;
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
            spawnEnemies();
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
        BigInteger inputNum = board.parseInput();
        Debug.Log(inputNum);
        int damageDealt = 0; //damage calculation
        bool anyDMGdealt = false;
        foreach (Enemy e in currEnemies) {
            if(inputNum % e.number == 0) {
                if (!anyDMGdealt) {
                    //some damage was dealt, show green bar (board.showGreen)
                    anyDMGdealt = true;
                }
                e.addToHealth(-damageDealt); //deal damage to the enemy
            }
        }
        if (!anyDMGdealt) {
            //no damage was dealt, show red bar (board.showRed)
        }
        int amtHealed = 0;
        player.addToHealth(amtHealed);
        int amtPoisoned = 0;
        player.addToHealth(amtPoisoned);
        yield return null;
    }
    private IEnumerator EnemyTurn() {
        foreach(Enemy e in currEnemies) yield return StartCoroutine(e.Attack(player, board));
    }

    private IEnumerator PlayerWins() {
        yield return null;
    }
    private IEnumerator GameOver() {
        yield return null;
    }

    private void spawnEnemies() {
        //currFloor = 0;
        currEnemies = new Enemy[1];
        currEnemies[0] = new Enemy();
        currEnemies[0].number = 1;
    }
    private void adjustOrbRates() {
        //currFloor = 0;  && board rates
    }
    private void adjustPlayerStats() {
        //currFloor = 0; switch case
    }

    private bool livingEnemyExists() {
        // for(int e = 0; e < currEnemies.Length; e++) if (currEnemies[e].isAlive()) return true;
        // return false;
        return true;
    }
}
