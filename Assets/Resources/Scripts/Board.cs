using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour {
    private const int ROWS = 5;
    private const int COLUMNS = 6;
    private const int X_OFFSET = 7;
    private const int Y_OFFSET = 7;
    private const int ORB_LEN = 32;
    private const int ORB_SPACE = 2;
    private static float SCALE;
    private const float THRESHOLD = 0.4f;

    public static readonly float DISAPPEAR_DURATION = 0.25f;
    public static readonly Vector3 FALL_SPEED = new Vector3(0f, -5f);

    public TextMeshPro numBar;
    private Orb[][] orbArray = new Orb[COLUMNS][];
    private Stack<Orb> selectedOrbs = new Stack<Orb>();
    public float[] orbSpawnRates = new float[12];  //must always add up to 1

    private WaitUntil waitForInput;
    void Awake() {
        for (int i = 0; i < COLUMNS; i++) orbArray[i] = new Orb[ROWS];
        for (int i = 0; i < 10; i++) orbSpawnRates[i] = 0.1f;

        if(Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.WindowsEditor) waitForInput = new WaitUntil(() => Input.GetMouseButtonDown(0));
        else if(Application.platform == RuntimePlatform.Android) waitForInput = new WaitUntil(() => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        SCALE = Mathf.Min(Screen.width / 216f, Screen.height / 384f);
        
    }
    void Start() {
        StartCoroutine(fillBoard());
    }
    private bool inputReleased(){
        if(Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.WindowsEditor) return !Input.GetMouseButton(0);
        else if(Application.platform == RuntimePlatform.Android) return Input.GetTouch(0).phase == TouchPhase.Ended;
        return true;
    }
    private Vector2 getRelativeInputPos(){
        Vector2 relativeInputPos = Vector2.zero;
        if(Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.WindowsEditor) relativeInputPos = convertScreenToGridPos(Input.mousePosition);
        else if(Application.platform == RuntimePlatform.Android) relativeInputPos = convertScreenToGridPos(Input.GetTouch(0).position);
        return relativeInputPos;
    }
    public IEnumerator getInput() {
        //getting valid input
        do {
            //reseting the stack and its orbs
            while (selectedOrbs.Count > 0) {
                selectedOrbs.Peek().isSelected = false;
                selectedOrbs.Pop().updateConnectors();
            }
            displayNumBar();
            //waiting for && getting input
            yield return waitForInput;
            while (!inputReleased()) {
                Vector2 relativeInputPos = getRelativeInputPos();
                int c = Mathf.RoundToInt(relativeInputPos.x);
                int r = Mathf.RoundToInt(relativeInputPos.y);
                if (0 <= r && r < ROWS && 0 <= c && c < COLUMNS && Mathf.Abs(relativeInputPos.x - c) <= THRESHOLD && Mathf.Abs(relativeInputPos.y - r) <= THRESHOLD) {
                    Orb chosenOrb = orbArray[c][r];
                    Vector2 currGridPosition = new Vector2(c, r);
                    if (selectedOrbs.Count == 0) {
                        selectedOrbs.Push(chosenOrb);
                        chosenOrb.isSelected = true;
                    }
                    else {
                        Orb head = selectedOrbs.Pop();
                        Vector2 prevHeadDir = head.prevOrbDir;
                        head.prevOrbDir = Vector2.zero;
                        head.isSelected = false;
                        if (!(selectedOrbs.Count >= 1 && currGridPosition.Equals(selectedOrbs.Peek().getGridPos()))) {  //if the player backtracks, then the head orb is removed
                            selectedOrbs.Push(head);
                            head.prevOrbDir = prevHeadDir;
                            head.isSelected = true;
                            if (!chosenOrb.Equals(head) && chosenOrb.isAdjacentTo(head) && !chosenOrb.isSelected) {  //if the player moves to a new, adjacent orb, then the new orb is put in
                                head.nextOrbDir = head.directionTo(chosenOrb);
                                selectedOrbs.Push(chosenOrb);
                                chosenOrb.prevOrbDir = chosenOrb.directionTo(head);
                                chosenOrb.isSelected = true;
                            }
                        }
                        else chosenOrb.nextOrbDir = Vector2.zero;
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
        Orb[] tempOrbs = selectedOrbs.ToArray();
        foreach(Orb o in tempOrbs){
            int value = o.getValue();
            string digit;
            switch (value) {
                case 10:
                    digit = "E";
                    break;
                case 11:
                    digit = "P";
                    break;
                default:
                    digit = value.ToString();
                    break;
            }
            input = string.Concat(digit, input);
        }
        return digitsOnly ? new string(input.Where(c => char.IsDigit(c)).ToArray()) : input;
    }

    public IEnumerator clearBoard() {
        //disappear animation
        Orb[] tempOrbs = selectedOrbs.ToArray();
        foreach (Orb o in tempOrbs) {
            Vector2 rmvPos = o.getGridPos();
            orbArray[(int)rmvPos.x][(int)rmvPos.y].removeConnectorSprites();
        }
        float disappearTimer = 0f;
        float disappearOffset = 0.05f;
        while (disappearTimer <= DISAPPEAR_DURATION) {
            foreach (Orb o in tempOrbs) {
                Vector2 rmvPos = o.getGridPos();
                orbArray[(int)rmvPos.x][(int)rmvPos.y].getWhiteRenderer().color = Color.Lerp(Color.clear, Color.white, disappearTimer / (DISAPPEAR_DURATION - disappearOffset));
            }
            disappearTimer += Time.deltaTime;
            yield return null;
        }
        //removing the orbs
        do {
            Vector2 rmvPos = selectedOrbs.Pop().getGridPos();
            OrbPool.SharedInstance.ReturnToPool(orbArray[(int)rmvPos.x][(int)rmvPos.y].gameObject);
            orbArray[(int)rmvPos.x][(int)rmvPos.y] = null;
        } while (selectedOrbs.Count > 0);
        yield return StartCoroutine(fillBoard());
    }

    private IEnumerator fillBoard() {
        //moving and spawning the orbs
        for (int c = 0; c < COLUMNS; c++) {
            int lowestEmptyRow = -1;
            for (int r = 0; r < ROWS; r++) {
                if (orbArray[c][r] == null && lowestEmptyRow == -1) lowestEmptyRow = r;
                else if (orbArray[c][r] != null && lowestEmptyRow != -1) {
                    orbArray[c][lowestEmptyRow] = orbArray[c][r];
                    orbArray[c][lowestEmptyRow].setGridPos(new Vector2(c, lowestEmptyRow));
                    orbArray[c][r] = null;
                    lowestEmptyRow++;
                }
            }
            for (int i = lowestEmptyRow; i < ROWS && lowestEmptyRow != -1; i++) orbArray[c][i] = getRandomOrb(c, i, ROWS - lowestEmptyRow);
        }
        //making the orbs fall
        bool isFalling;
        do{
            isFalling = false;
            for(int c = 0; c < COLUMNS; c++){
                for(int r = 0; r < ROWS; r++){
                    Vector2 currGridPos = orbArray[c][r].getGridPos();
                    Vector2 target = Board.convertGridToWorldPos(currGridPos);
                    Transform trans = orbArray[c][r].getTrans();
                    if(trans.position.y > target.y){
                        trans.position += FALL_SPEED * Time.deltaTime;
                        isFalling = true;
                    }
                    else trans.position = target;
                }
            }
            yield return null;
        } while(isFalling);
    }

    private Orb getRandomOrb(int column, int row, int fallDist) {
        float rand = Random.value;
        ORB_VALUE newOrb = ORB_VALUE.ZERO;
        foreach (float prob in orbSpawnRates) {
            rand -= prob;
            if (rand <= 0) break;
            newOrb++;
        }
        return OrbPool.SharedInstance.GetPooledOrb(new Vector2(column, row + fallDist), fallDist, newOrb).GetComponent<Orb>();
    }

    private void displayNumBar(){  //TO-DO
        numBar.text = getInputNum(true);
    }

    public void flashNumBar(bool playerAttacked){   //TO-DO
        //color = playerAttacked ? green : red
    }


    public static Vector2 convertGridToWorldPos(Vector2 gridPos) {
        Vector2 screenPos = new Vector2((gridPos.x + 0.5f) * (ORB_LEN + ORB_SPACE) + X_OFFSET - ORB_SPACE / 2, (gridPos.y + 0.5f) * (ORB_LEN + ORB_SPACE) + Y_OFFSET - ORB_SPACE / 2) * SCALE;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
    public static Vector2 convertScreenToGridPos(Vector2 screenPos) {
        return new Vector2(screenPos.x / SCALE - X_OFFSET + ORB_SPACE / 2, screenPos.y / SCALE - Y_OFFSET + ORB_SPACE / 2) / (ORB_LEN + ORB_SPACE) - new Vector2(0.1f, 0.1f) * SCALE;  //SUS
    }
    //calculating damage animations
    //show number bar red or green
    //changing floor animation
}
