using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[Serializable]
public class GameState {
    public int floor = 0, turnCount = 0;
    public double elapsedTime = 0;
}

public class GameController : MonoBehaviour {
    public static GameController Instance;
    private const double MAX_TIME = 3600 * 99 + 60 * 99 + 99;
    private GameState currState = new GameState();
    [SerializeField] private GameStatsUI gsUI = default;
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool waitingForInput = false;

    private EnemySpawner es = new EnemySpawner();
    private List<Enemy> currEnemies;
    [SerializeField] private SpriteRenderer currEnemyBG = default;
    private Sprite[] enemyBGs;
    private AudioClip[] musBGs;

    [SerializeField] private DamageBar damageBar = default;
    [SerializeField] private GameObject endAnim = default;

    void Awake() {
        Instance = this;
        enemyBGs = Resources.LoadAll<Sprite>("Sprites/Main Screen/Board/Enemy Board");
        musBGs = Resources.LoadAll<AudioClip>("Audio/Music");
    }
    void Start() {
        StartCoroutine(LoadingScreen.Instance.HideDelay());
        isPaused = false;
        currEnemies = new List<Enemy>();
        if (isLoadingData()) SaveStateController.Instance.loadDataIntoGame();
        StartCoroutine(TurnRoutine());
    }
    void Update() {
        if (waitingForInput && !isPaused) {
            currState.elapsedTime = Math.Min(currState.elapsedTime + Time.deltaTime, MAX_TIME);
            gsUI.updateText(currState);
        }
    }
    private bool isLoadingData() => PlayerPrefs.HasKey("LoadFromSaveFile") && PlayerPrefs.GetInt("LoadFromSaveFile") == 1;
    public GameState getState() => currState;
    public void setState(GameState gs) => currState = gs;
    public List<Enemy> getCurrEnemies() => currEnemies;
    public void loadEnemy(Enemy e) => currEnemies.Add(e);
    public void saveGame() => SaveStateController.Instance.saveCurrData();

    private IEnumerator TurnRoutine() {
        do {
            yield return StartCoroutine(initRound());
            do {
                yield return StartCoroutine(PlayerTurn());
                yield return StartCoroutine(EnemyTurn());
                currState.turnCount++;
            } while (Player.Instance.isAlive() && currEnemies.Count > 0);
            currState.floor++;
        } while (currState.floor <= 50 && Player.Instance.isAlive());
        yield return StartCoroutine(gameEnd(Player.Instance.isAlive() && currState.floor == 50));
    }

    private IEnumerator initRound() {
        gsUI.updateText(currState);
        adjustBackground();
        adjustMusic();
        if (!isLoadingData()) currEnemies = es.getEnemies(currState.floor);
        else PlayerPrefs.SetInt("LoadFromSaveFile", 0);
        displayEnemies();
        adjustOrbRates();
        yield return StartCoroutine(adjustPlayerStats());
        saveGame();
    }
    private void adjustBackground() {
        int currEnemyBGIndex = 0;
        if (currState.floor > 0) currEnemyBGIndex++;
        if (currState.floor > 15) currEnemyBGIndex++;
        if (currState.floor > 30) currEnemyBGIndex++;
        if (currState.floor > 45) currEnemyBGIndex++;
        if (currState.floor == 50) currEnemyBGIndex++;
        currEnemyBG.sprite = enemyBGs[currEnemyBGIndex];
    }
    private void adjustMusic() {
        int bgMus = 0;
        if (currState.floor > 0) bgMus++;
        if (currState.floor > 15) bgMus++;
        if (currState.floor > 30) bgMus++;
        if (currState.floor > 45) bgMus++;
        if (currState.floor == 50) bgMus++;
        AudioClip clip = AudioController.Instance.musicSource.clip;
        if (clip == null || clip.name != musBGs[bgMus].name) {
            AudioController.Instance.musicSource.clip = musBGs[bgMus];
            AudioController.Instance.musicSource.Play();
        }
    }
    private IEnumerator adjustPlayerStats() {
        int maxHealth = 400;
        if (currState.floor > 0) maxHealth += 100;
        if (currState.floor > 15) maxHealth += 250;
        if (currState.floor > 30) maxHealth += 250;
        if (currState.floor > 45) maxHealth += 500;
        if (currState.floor == 50) maxHealth += 500;
        yield return StartCoroutine(Player.Instance.setMaxHealth(maxHealth));
    }
    private void adjustOrbRates() {
        Board.Instance.resetOrbSpawnRates();
        foreach (Enemy e in currEnemies) {
            // TO-DO: do something
        }
    }
    private void setWaitingForInput(bool getInput) {
        waitingForInput = getInput;
        gsUI.toggleAll(waitingForInput);
    }

    private IEnumerator PlayerTurn() {
        //getting input
        yield return StartCoroutine(Board.Instance.toggleForeground(false));
        setWaitingForInput(true);
        if (Player.Instance.getDOT() != 0) {
            gsUI.togglePauseButton(false);  // disable pause button if taking damage over time
            StartCoroutine(Player.Instance.takeDOT());
        }
        yield return StartCoroutine(Board.Instance.getInput());
        if (currState.floor != 50) Player.Instance.setDOT(0);  // ends player DOT once their turn ends TO-DO: if activated by final boss, only the boss can stop it
        setWaitingForInput(false);
        string inputNum = Board.Instance.getInputNum(false);
        bool isNulified = Board.Instance.numberIsNullified();
        BigInteger actualNum = Board.Instance.getInputNum(true).Equals("") ? new BigInteger(1) : BigInteger.Parse(Board.Instance.getInputNum(true));

        //checking if the input is divisible by any enemy
        bool anyDMGdealt = false;
        if (!isNulified) {
            foreach (Enemy e in currEnemies) {
                bool dealDMG = actualNum % e.getState().number == 0;
                if (dealDMG) StartCoroutine(e.targetedAnimation(false));  //flashing red animation start
                anyDMGdealt = anyDMGdealt || dealDMG;
            }
        }
        Board.Instance.setNumBarColor(anyDMGdealt ? NUMBAR_STATE.SUCCESS : NUMBAR_STATE.FAILURE);

        //clear board while calculating damage/heals/poisons sequentially
        foreach (char c in inputNum) {
            StartCoroutine(Board.Instance.rmvNextOrb());
            if (!isNulified) {
                switch (c) {
                    case 'P':
                        StartCoroutine(Player.Instance.addToHealth(-50, ColorPalette.getColor(14, 2)));
                        break;
                    case 'E': case 'S': case 'N':
                        // Do nothing.
                        break;
                    case '0':
                        if (anyDMGdealt) damageBar.addNextDigit(0);
                        StartCoroutine(Player.Instance.addToHealth(50));
                        break;
                    default:
                        if (anyDMGdealt) damageBar.addNextDigit((int)char.GetNumericValue(c));
                        break;
                }
            }
            yield return Board.Instance.DISAPPEAR_DELTA;
        }
        if (!Player.Instance.isAlive()) Player.Instance.setCauseOfDeath("poison");

        //fill the board
        yield return new WaitForSeconds(Board.Instance.DISAPPEAR_DURATION);
        yield return StartCoroutine(Board.Instance.fillBoard(false));
        yield return StartCoroutine(Board.Instance.toggleForeground(true));

        //deal damage to enemies
        damageBar.displayText(false);
        if (anyDMGdealt) {
            int damageDealt = damageBar.getCurrDamage();
            for (int i = 0; i < currEnemies.Count; i++) {  //deal damage to the enemy
                Enemy e = currEnemies[i];
                if (actualNum % e.getState().number == 0) {
                    yield return StartCoroutine(e.takeDMG(-damageDealt));
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
        yield return StartCoroutine(Player.Instance.resetDeltaHealth());
    }
    private IEnumerator EnemyTurn() {
        adjustOrbRates();
        foreach (Enemy e in currEnemies) {
            yield return StartCoroutine(e.Attack());
            adjustOrbRates();
            if (!Player.Instance.isAlive()) {
                Player.Instance.setCauseOfDeath(e.getState().number.ToString());
                break;
            }
        }
        yield return StartCoroutine(Player.Instance.resetDeltaHealth());
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
        // Sending data to the leaderboard.
        PlayerPrefs.SetInt("Floor", currState.floor);
        PlayerPrefs.SetString("Time", currState.elapsedTime.ToString("R"));
        PlayerPrefs.SetString("Death", Player.Instance.getCauseOfDeath());

        // Ending animation.
        yield return endAnim.GetComponent<EndGameAnimation>().endGameAnimation(win);
    }
}