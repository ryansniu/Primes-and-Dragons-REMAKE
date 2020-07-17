using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.ObjectModel;

public enum BoardPattern { ROW, COLUMN, PLUS, CROSS, BOX, SPIRAL, RANDOM }
public enum OrbSpawnRate { NONE, DECREASED, NORMAL, MAX, INCREASED }

public static class NUMBAR_STATE {
    public static readonly Color DEFAULT = ColorPalette.getColor(2, 2);
    public static readonly Color SUCCESS = ColorPalette.getColor(6, 2);
    public static readonly Color FAILURE = ColorPalette.getColor(1, 1);
}

[Serializable]
public class BoardState { public OrbState[][] orbStates; }

public class Board : MonoBehaviour {
    public static Board Instance;
    public const int ROWS = 5, COLUMNS = 6;
    private const int X_OFFSET = 7, Y_OFFSET = 7;
    private const int ORB_LEN = 32, ORB_SPACE = 2;
    private static float SCALE;
    private const float THRESHOLD = 0.4f;
    private System.Random RNG = new System.Random();

    private BoardState currState = new BoardState();
    private bool loadFromState = false;
    private Orb[][] orbArray = new Orb[COLUMNS][];
    private List<Orb> selectedOrbs = new List<Orb>();
    private static OrbSpawnRate[] defaultOSR = { OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NORMAL, OrbSpawnRate.NONE, OrbSpawnRate.NONE, OrbSpawnRate.NONE, OrbSpawnRate.NONE};  // This line can't get any longer can it? Oh wait-
    public static ReadOnlyCollection<OrbSpawnRate> defaultOrbSpawnRates = new ReadOnlyCollection<OrbSpawnRate>(defaultOSR);
    private OrbSpawnRate[] orbSpawnRates = new OrbSpawnRate[Enum.GetValues(typeof(ORB_VALUE)).Length];  //must always add up to 1

    public static readonly float DISAPPEAR_DURATION = 0.25f;
    public static readonly float DISAPPEAR_DELTA = 0.05f;
    public readonly Vector3 FALL_SPEED = new Vector3(0f, -480f);
    [SerializeField] private SpriteRenderer orbGridFG = default;
    private bool isDarkened = true;

    [SerializeField] private TextMeshProUGUI numBar = default;
    [SerializeField] private Image numBarBG = default;

    private bool useTouchInput = false;
    private WaitUntil waitForInput;
    
    // vv SAVING AND LOADING vv
    public BoardState getState() {
        currState.orbStates = new OrbState[COLUMNS][];
        for (int i = 0; i < orbArray.Length; i++) {
            currState.orbStates[i] = new OrbState[ROWS];
            for (int j = 0; j < orbArray[i].Length; j++) currState.orbStates[i][j] = orbArray[i][j].getState();
        }
        return currState;
    }
    public void setState(BoardState bs) {
        currState = bs;
        fillBoardData(true);
        loadFromState = true;
    }
    // ^^ SAVING AND LOADING ^^

    void Awake() {
        Instance = this;
        for (int i = 0; i < COLUMNS; i++) orbArray[i] = new Orb[ROWS];
        resetOrbSpawnRates();
        useTouchInput = SystemInfo.deviceType == DeviceType.Handheld;
        waitForInput = useTouchInput ? new WaitUntil(() => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) : new WaitUntil(() => Input.GetMouseButtonDown(0));
        SCALE = Mathf.Min(Screen.width / 216f, Screen.height / 384f);
    }
    void Start() {
        setNumBarColor(NUMBAR_STATE.DEFAULT);
        if (!loadFromState) fillBoardData(false);
        StartCoroutine(makeOrbsFall());
        loadFromState = false;
    }
    private bool inputReleased() => useTouchInput ? Input.GetTouch(0).phase == TouchPhase.Ended : !Input.GetMouseButton(0);
    private Vector2 getRelativeInputPos() => useTouchInput ? convertScreenToGridPos(Input.GetTouch(0).position) : convertScreenToGridPos(Input.mousePosition);
    public IEnumerator getInput() {
        //getting valid input
        do {
            //reseting the stack and its orbs
            while (selectedOrbs.Count > 0) {
                selectedOrbs.Last().isSelected = false;
                selectedOrbs.Last().updateConnectors();
                selectedOrbs.Remove(selectedOrbs.Last());
            }
            displayNumBar();
            setNumBarColor(NUMBAR_STATE.DEFAULT);
            //waiting for && getting input
            yield return waitForInput;
            while (!inputReleased() && !GameController.Instance.isPaused) {
                Vector2 relativeInputPos = getRelativeInputPos();
                int c = Mathf.RoundToInt(relativeInputPos.x);
                int r = Mathf.RoundToInt(relativeInputPos.y);
                if (0 <= r && r < ROWS && 0 <= c && c < COLUMNS && Mathf.Abs(relativeInputPos.x - c) <= THRESHOLD && Mathf.Abs(relativeInputPos.y - r) <= THRESHOLD) {
                    Orb chosenOrb = orbArray[c][r];
                    Vector2Int currGridPosition = new Vector2Int(c, r);
                    if (selectedOrbs.Count == 0) {
                        selectedOrbs.Add(chosenOrb);
                        chosenOrb.isSelected = true;
                    }
                    else {
                        Orb head = selectedOrbs.Last();
                        selectedOrbs.Remove(selectedOrbs.Last());
                        Vector2Int prevHeadDir = head.prevOrbDir;
                        head.prevOrbDir = Vector2Int.zero;
                        head.isSelected = false;
                        if (!(selectedOrbs.Count >= 1 && currGridPosition.Equals(selectedOrbs.Last().getGridPos()))) {  //if the player backtracks, then the head orb is removed
                            selectedOrbs.Add(head);
                            head.prevOrbDir = prevHeadDir;
                            head.isSelected = true;
                            if (!chosenOrb.Equals(head) && chosenOrb.isAdjacentTo(head) && !chosenOrb.isSelected //if the player moves to a new adjacent orb, then the new orb is put in
                                && (numberIsNullified() || head.getOrbValue() != ORB_VALUE.STOP)) {  // can't move if the current orb is a non-nullified stop orb
                                head.nextOrbDir = head.directionTo(chosenOrb);
                                selectedOrbs.Add(chosenOrb);
                                chosenOrb.prevOrbDir = chosenOrb.directionTo(head);
                                chosenOrb.isSelected = true;
                            }
                        }
                        else chosenOrb.nextOrbDir = Vector2Int.zero;
                        head.updateConnectors();
                        chosenOrb.updateConnectors();
                    }
                }
                displayNumBar();
                yield return null;
            }
        } while (selectedOrbs.Count <= 1);
    }
    public string getInputNum(bool digitsOnly) {
        string input = "";
        for (int i = selectedOrbs.Count - 1; i >= 0; i--) {
            int value = selectedOrbs[i].getIntValue();
            string digit;
            switch (value) {
                case 10: digit = "P"; break;
                case 11: digit = "E"; break;
                case 12: digit = "N"; break;
                case 13: digit = "S"; break;
                default: digit = value.ToString(); break;
            }
            input = string.Concat(digit, input);
        }
        return digitsOnly ? new string(input.Where(c => char.IsDigit(c)).ToArray()) : input;
    }
    public IEnumerator fillBoard() {
        fillBoardData(false);
        yield return StartCoroutine(makeOrbsFall());
    }
    private void fillBoardData(bool isLoadFromState) {
        //moving and spawning the orbs
        for (int c = 0; c < COLUMNS; c++) {
            int lowestEmptyRow = -1;
            for (int r = 0; r < ROWS; r++) {
                if (orbArray[c][r] == null && lowestEmptyRow == -1) lowestEmptyRow = r;
                else if (orbArray[c][r] != null && lowestEmptyRow != -1) {
                    orbArray[c][lowestEmptyRow] = orbArray[c][r];
                    orbArray[c][lowestEmptyRow].setGridPos(new Vector2Int(c, lowestEmptyRow));
                    orbArray[c][r] = null;
                    lowestEmptyRow++;
                }
            }
            for (int i = lowestEmptyRow; i < ROWS && lowestEmptyRow != -1; i++) {
                if (isLoadFromState) {
                    orbArray[c][i] = spawnOrb(currState.orbStates[c][i].value, c, i, ROWS - lowestEmptyRow);
                    orbArray[c][i].setState(currState.orbStates[c][i]);
                }
                else orbArray[c][i] = spawnRandomOrb(c, i, ROWS - lowestEmptyRow);
            }
        }
    }
    private IEnumerator makeOrbsFall() {
        bool isFalling;
        Transform[][] orbTrans = new Transform[COLUMNS][];
        for (int c = 0; c < COLUMNS; c++) orbTrans[c] = new Transform[ROWS];
        do {
            isFalling = false;
            for (int c = 0; c < COLUMNS; c++) {
                for (int r = 0; r < ROWS; r++) {
                    Vector2 target = convertGridToWorldPos(orbArray[c][r].getGridPos());
                    if (orbTrans[c][r] == null) orbTrans[c][r] = orbArray[c][r].getTrans();
                    if (orbTrans[c][r].position.y > target.y) {
                        orbTrans[c][r].position += FALL_SPEED * Time.deltaTime;
                        isFalling = true;
                    }
                    else orbTrans[c][r].position = target;
                }
            }
            yield return null;
        } while (isFalling);
    }

    public IEnumerator toggleForeground(bool darken) {
        if (isDarkened != darken) {
            float animationTime = 0.25f;
            Color darkFG = new Color(0f, 0f, 0f, 0.5f);
            for (float currTime = 0f; currTime < animationTime; currTime += Time.deltaTime) {
                orbGridFG.color = Color.Lerp(Color.clear, darkFG, darken ? currTime / animationTime : 1f - currTime / animationTime);
                yield return null;
            }
            if (darken) orbGridFG.color = darken ? darkFG : Color.clear;
            isDarkened = darken;
        }
    }
    private Orb spawnRandomOrb(int column, int row, int fallDist) {
        int total = 0;
        foreach (OrbSpawnRate osr in orbSpawnRates) total += (int)osr;
        float rand = RNG.Next(total);
        ORB_VALUE newOrb = ORB_VALUE.ZERO;
        foreach (OrbSpawnRate osr in orbSpawnRates) {
            rand -= (int)osr;
            if (rand < 0) break;
            newOrb++;
        }
        return spawnOrb(newOrb, column, row, fallDist);
    }
    private Orb spawnOrb(ORB_VALUE value, int column, int row, int fallDist) => OrbPool.Instance.GetPooledOrb(new Vector2(column, row + fallDist), fallDist, value).GetComponent<Orb>();

    private void displayNumBar() => numBar.text = numberIsNullified() ? "null" : getInputNum(true);
    public void setNumBarColor(Color c) => numBarBG.color = c;
    public bool numberIsNullified() {
        bool isNullified = getInputNum(false).Contains("N");
        if (Orb.BOARD_IS_NULLIFIED != isNullified) {
            Orb.BOARD_IS_NULLIFIED = isNullified;
            foreach(Orb o in selectedOrbs) o.updateConnectors();
        }
        return isNullified;
    }

    // Marking orbs
    public IEnumerator markAllOrbsIf(string enemyID, Func<Orb, bool> condition, float delay) {
        for (int r = ROWS - 1; r >= 0; r--) for (int c = 0; c < COLUMNS; c++) if(condition(orbArray[c][r])) yield return StartCoroutine(markOrbAt(c, r, enemyID, delay));
    }
    public IEnumerator markOrbsInOrder(string enemyID, List<Vector2Int> order, float delay) {
        foreach(Vector2Int orbPos in order) yield return StartCoroutine(markOrbAt(orbPos.x, orbPos.y, enemyID, delay));
    }
    private IEnumerator markOrbAt(int c, int r, string enemyID, float delay) {
        orbArray[c][r].toggleOrbHighlight(true);
        orbArray[c][r].toggleOrbMarker(enemyID, true);
        if (delay != 0) yield return new WaitForSeconds(delay);
        orbArray[c][r].toggleOrbHighlight(false);
    }
    public void unmarkAllOrbsBy(string enemyID) {
        for (int r = ROWS - 1; r >= 0; r--) for (int c = 0; c < COLUMNS; c++) if (orbArray[c][r].getIsMarkedBy(enemyID)) orbArray[c][r].toggleOrbMarker(enemyID, false);
    }
    // Getting orbs
    public Orb getOrb(int c, int r) => orbArray[c][r];
    public int getNumValidOrbs(Func<Orb, bool> condition) {
        int total = 0;
        for (int r = ROWS - 1; r >= 0; r--) for (int c = 0; c < COLUMNS; c++) if (condition(orbArray[c][r])) total++;
        return total;
    }
    public List<Orb> getAllMarkedOrbsBy(string enemyID, List<Vector2Int> order) {
        List<Orb> results = new List<Orb>();
        if (order == null) {
            for (int r = ROWS - 1; r >= 0; r--) for (int c = 0; c < COLUMNS; c++) if (orbArray[c][r].getIsMarkedBy(enemyID)) results.Add(orbArray[c][r]);
        }
        else foreach (Vector2Int orbPos in order) if (orbArray[orbPos.x][orbPos.y].getIsMarkedBy(enemyID)) results.Add(orbArray[orbPos.x][orbPos.y]);
        return results;
    }
    // Setting orbs
    public IEnumerator setAllMarkedOrbsBy(string enemyID, Func<Orb, ORB_VALUE> getVal, float delay, List<Vector2Int> order) {
        List<Orb> toSet = getAllMarkedOrbsBy(enemyID, order);
        foreach (Orb orb in toSet) {
            orb.toggleOrbHighlight(false);
            orb.toggleOrbMarker(enemyID, false);
            ORB_VALUE newVal = getVal(orb);
            if (orb.getOrbValue() != newVal) {
                orb.changeValue(newVal);
                orb.toggleOrbHighlight(true);
                if (delay != 0) yield return new WaitForSeconds(delay);
                orb.toggleOrbHighlight(false);
            }
        }
    }
    // Incrementing orbs
    public IEnumerator incrementAllMarkedOrbsBy(string enemyID, Func<Orb, int> getInc, float delay, List<Vector2Int> order) {
        List<Orb> toIncrement = getAllMarkedOrbsBy(enemyID, order);
        foreach (Orb orb in toIncrement) {
            orb.toggleOrbHighlight(false);
            orb.toggleOrbMarker(enemyID, false);
            int incVal = getInc(orb);
            if (incVal != 0) {
                orb.incrementValue(incVal);
                orb.toggleOrbHighlight(true);
                if (delay != 0) yield return new WaitForSeconds(delay);
                orb.toggleOrbHighlight(false);
            }
        }
    }
    // Removing orbs
    public IEnumerator removeAllMarkedOrbsBy(string enemyID, float delay, List<Vector2Int> order = null) {
        selectedOrbs = getAllMarkedOrbsBy(enemyID, order);
        int numOrbs = selectedOrbs.Count;
        for (int i = 0; i < numOrbs; i++) {
            selectedOrbs.First().toggleOrbHighlight(true);
            yield return StartCoroutine(rmvNextOrb(delay));
        }
        yield return new WaitForSeconds(DISAPPEAR_DURATION);
        yield return StartCoroutine(fillBoard());
    }
    public IEnumerator rmvNextOrb(float delay = 0) {
        StartCoroutine(rmvOrb());
        if (delay != 0) yield return new WaitForSeconds(delay);
    }
    private IEnumerator rmvOrb() {
        Orb rmvOrb = selectedOrbs.First();
        selectedOrbs.Remove(selectedOrbs.First());
        Vector2Int rmvPos = rmvOrb.getGridPos();
        orbArray[rmvPos.x][rmvPos.y].removeConnectorSprites();
        for (float disappearTimer = 0f; disappearTimer <= DISAPPEAR_DURATION; disappearTimer += Time.deltaTime) {
            orbArray[rmvPos.x][rmvPos.y].getWhiteRenderer().color = Color.Lerp(Color.clear, rmvOrb.sprWhiteColor, Mathf.SmoothStep(0f, 1f, disappearTimer / DISAPPEAR_DURATION));
            yield return null;
        }
        OrbPool.Instance.ReturnToPool(orbArray[rmvPos.x][rmvPos.y].gameObject);
        orbArray[rmvPos.x][rmvPos.y] = null;
    }
    // Shuffling orbs
    public IEnumerator shuffleBoard(int numShuffles = 1, float delay = 0f) {  // sum ugly ass code
        for(int s = 0; s < numShuffles; s++) {
            for (int i = 0; i < ROWS * COLUMNS - 1; i++) {
                int j = RNG.Next(i, ROWS * COLUMNS);
                int row_i = i / COLUMNS;
                int col_i = i % COLUMNS;
                int row_j = j / COLUMNS;
                int col_j = j % COLUMNS;
                Orb orb1 = orbArray[col_i][row_i];
                Orb orb2 = orbArray[col_j][row_j];
                Vector2Int temp = orb1.getGridPos();
                orb1.setGridPos(orb2.getGridPos());
                orb2.setGridPos(temp);
                orbArray[col_i][row_i] = orb2;
                orbArray[col_j][row_j] = orb1;
                orb1.getTrans().position = convertGridToWorldPos(orb1.getGridPos());
                orb2.getTrans().position = convertGridToWorldPos(orb2.getGridPos());
            }
            if (delay != 0) yield return new WaitForSeconds(delay);
        }
    }

    // Setting and resetting orb spawn rates
    public static OrbSpawnRate[] getDefaultOrbSpawnRates() => defaultOrbSpawnRates.ToArray();
    public void resetOrbSpawnRates() => setOrbSpawnRates(getDefaultOrbSpawnRates());
    public void setOrbSpawnRates(OrbSpawnRate[] newOrbSpawnRates) { if(newOrbSpawnRates.Count() == orbSpawnRates.Count()) orbSpawnRates = newOrbSpawnRates; }

    // Converting between unity scales and perspectives
    public static Vector2 convertGridToWorldPos(Vector2Int gridPos) {
        Vector2 screenPos = new Vector2((gridPos.x + 0.5f) * (ORB_LEN + ORB_SPACE) + X_OFFSET - ORB_SPACE / 2, (gridPos.y + 0.5f) * (ORB_LEN + ORB_SPACE) + Y_OFFSET - ORB_SPACE / 2) * SCALE;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
    public static Vector2 convertScreenToGridPos(Vector2 screenPos) {
        return new Vector2(screenPos.x / SCALE - X_OFFSET + ORB_SPACE / 2, screenPos.y / SCALE - Y_OFFSET + ORB_SPACE / 2) / (ORB_LEN + ORB_SPACE) - new Vector2(0.1f, 0.1f) * SCALE;  // TO-DO: SUS
    }
}