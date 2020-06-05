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
    private AudioClip[] musBGs;

    public Player player;
    public Board board;
    public DamageBar damageBar;
    public GameObject endAnim;

    void Awake() {
        enemyBGs = Resources.LoadAll<Sprite>("Sprites/Main Screen/Enemy Board");
        musBGs = Resources.LoadAll<AudioClip>("Audio/Music");
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
        adjustMusic();
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
    private void adjustMusic() {
        int bgMus = 0;
        if (GSaUI.currFloor > 0) bgMus++;
        if (GSaUI.currFloor > 15) bgMus++;
        if (GSaUI.currFloor > 30) bgMus++;
        if (GSaUI.currFloor > 45) bgMus++;
        if (GSaUI.currFloor == 50) bgMus++;
        if (AudioController.Instance.musicSource.clip.name != musBGs[bgMus].name) {
            AudioController.Instance.musicSource.clip = musBGs[bgMus];
            AudioController.Instance.musicSource.Play();
        }
    }
    private void adjustOrbRates() {
        //currFloor = 0;  board.orbSpawnRates
    }
    private IEnumerator adjustPlayerStats() {
        int maxHealth = 400;
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
        if (player.getDOT() != 0) GSaUI.pauseButton.interactable = false;  // disable pause button if taking damage over time
        yield return StartCoroutine(board.getInput());
        if (GSaUI.currFloor != 50) player.setDOT(0);  // ends player DOT once their turn ends TO-DO: if activated by final boss, only the boss can stop it
        GSaUI.toggle(false);
        string inputNum = board.getInputNum(false);
        bool isNulified = board.numberIsNullified();  //nullifies the whole string (TO-DO: should this change to just end of string?)
        BigInteger actualNum = board.getInputNum(true).Equals("") ? new BigInteger(1) : BigInteger.Parse(board.getInputNum(true));

        //checking if the input is divisible by any enemy
        bool anyDMGdealt = false;
        if (!isNulified) {
            foreach (Enemy e in currEnemies) {
                bool dealDMG = actualNum % e.currState.number == 0;
                anyDMGdealt = anyDMGdealt || dealDMG;
                e.toggleStatus(EnemyStatus.ATTACKED, true);  //flashing red animation start
            }
        }
        board.setNumBarColor(anyDMGdealt ? NUMBAR_STATE.SUCCESS : NUMBAR_STATE.FAILURE);

        //clear board while calculating damage/heals/poisons sequentially
        foreach (char c in inputNum) {
            StartCoroutine(board.rmvNextOrb());
            if (!isNulified) {
                switch (c) {
                    case 'P':
                        StartCoroutine(player.addToHealth(-50));
                        break;
                    case 'E': case 'S': case 'N':
                        // Do nothing.
                        break;
                    case '0':
                        if (anyDMGdealt) damageBar.addNextDigit(0);
                        StartCoroutine(player.addToHealth(50));
                        break;
                    default:
                        if (anyDMGdealt) damageBar.addNextDigit((int)char.GetNumericValue(c));
                        break;
                }
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
                    e.toggleStatus(EnemyStatus.ATTACKED, false);  //flashing red animation end
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
        foreach (Enemy e in currEnemies) yield return StartCoroutine(e.Attack(player, board));
        yield return StartCoroutine(player.resetDeltaHealth());
        if (!player.isAlive()) player.setCauseOfDeath(currEnemies[Random.Range(0, currEnemies.Count)].currState.number.ToString());
    }
    private void displayEnemies() {
        switch (currEnemies.Count) {
            case 1:
                currEnemies[0].setPosition(EnemyPosition.CENTER_1);
                break;
            case 2:
                currEnemies[0].setPosition(EnemyPosition.LEFT_2);
                currEnemies[1].setPosition(EnemyPosition.RIGHT_2);
                break;
            case 3:
                currEnemies[0].setPosition(EnemyPosition.LEFT_3);
                currEnemies[1].setPosition(EnemyPosition.CENTER_3);
                currEnemies[2].setPosition(EnemyPosition.RIGHT_3);
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