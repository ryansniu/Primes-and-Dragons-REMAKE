using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour {
    private const int ROWS = 5;
    private const int COLUMNS = 6;
    private const int X_OFFSET = 7;
    private const int Y_OFFSET = 7;
    private const int ORB_LEN = 32;
    private const int ORB_SPACE = 2;

    private Orb[][] orbArray = new Orb[COLUMNS][];
    private Stack<Orb> selectedOrbs = new Stack<Orb>();
    public float[] orbSpawnRates = new float[12];  //must always add up to 1

    void Awake() {
        for (int i = 0; i < COLUMNS; i++) orbArray[i] = new Orb[ROWS];
        for (int i = 0; i < 10; i++) orbSpawnRates[i] = 0.1f;
    }
    void Start() {
        fillBoard();
    }

    public IEnumerator getInput() {
        //getting valid input
        do {
            //reseting the stack and its orbs
            while (selectedOrbs.Count > 0) {  
                selectedOrbs.Peek().isSelected = false;
                selectedOrbs.Pop().updateConnectors();
            }
            //wait for input
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            //getting input
            while (Input.GetMouseButton(0)) {
                Vector2 relativeMousePos = convertScreenToGridPos(Input.mousePosition);
                int c = (int)(relativeMousePos.x);
                int r = (int)(relativeMousePos.y);
                if (0 <= r && r < ROWS && 0 <= c && c < COLUMNS) {
                    Orb chosenOrb = orbArray[c][r];
                    if (selectedOrbs.Count == 0) {
                        selectedOrbs.Push(chosenOrb);
                        chosenOrb.isSelected = true;
                    }
                    else {
                        Orb head = selectedOrbs.Pop();
                        Vector2 prevHeadDir = head.prevOrbDir;
                        head.prevOrbDir = Vector2.zero;
                        head.isSelected = false;
                        if (!(selectedOrbs.Count >= 1 && relativeMousePos.Equals(selectedOrbs.Peek().getGridPos()))) {  //if the player backtracks, then the head orb is removed
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
                yield return null;
            }
        } while (selectedOrbs.Count <= 1);
    }
    public string getInputNum() {
        string input = "";
        Orb[] tempOrbs = selectedOrbs.ToArray();
        do {
            int value = selectedOrbs.Pop().getValue();
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
        } while (selectedOrbs.Count > 0);
        foreach (Orb o in tempOrbs) selectedOrbs.Push(o);
        return input;
    }
    public System.Numerics.BigInteger parseInputNumOnly(string input) {
        return System.Numerics.BigInteger.Parse(new string(input.Where(c => char.IsDigit(c)).ToArray()));
    }

    public IEnumerator clearBoard() {
        Orb[] tempOrbs = selectedOrbs.ToArray();
        foreach(Orb o in tempOrbs) {
            Vector2 rmvPos = o.getGridPos();
            StartCoroutine(orbArray[(int)rmvPos.x][(int)rmvPos.y].disappearAnim());
        }
        yield return new WaitForSeconds(Orb.DISAPPEAR_DURATION);
        do {
            Vector2 rmvPos = selectedOrbs.Pop().getGridPos();
            OrbPool.SharedInstance.ReturnToPool(orbArray[(int)rmvPos.x][(int)rmvPos.y].gameObject);
            orbArray[(int)rmvPos.x][(int)rmvPos.y] = null;
        } while (selectedOrbs.Count > 0);
        fillBoard();
    }

    private void fillBoard() {
        for (int c = 0; c < COLUMNS; c++) {
            int lowestEmptyRow = -1;
            for(int r = 0; r < ROWS; r++) {
                if (orbArray[c][r] == null && lowestEmptyRow == -1) lowestEmptyRow = r;
                else if(orbArray[c][r] != null && lowestEmptyRow != -1) {
                    orbArray[c][lowestEmptyRow] = orbArray[c][r];
                    orbArray[c][lowestEmptyRow].setGridPos(new Vector2(c, lowestEmptyRow));
                    orbArray[c][r] = null;
                    lowestEmptyRow++;
                }
            }
            for(int i = lowestEmptyRow; i < ROWS && lowestEmptyRow != -1; i++) orbArray[c][i] = getRandomOrb(c, i, 5-lowestEmptyRow);
        }
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

    public static Vector2 convertGridToWorldPos(Vector2 gridPos) {  //BRUH MOMENT
        return Camera.main.ScreenToWorldPoint(new Vector2(gridPos.x * (ORB_LEN + ORB_SPACE) + X_OFFSET + ORB_SPACE / 2, gridPos.y * (ORB_LEN + ORB_SPACE) + Y_OFFSET + ORB_SPACE / 2));
    }

    public static Vector2 convertScreenToGridPos(Vector2 screenPos) {  //within a certain range (to allow for diagonals) (ROUND)
        return new Vector2((screenPos.x - X_OFFSET) / (ORB_LEN + ORB_SPACE), (screenPos.y - Y_OFFSET) / (ORB_LEN + ORB_SPACE));
    }

    //calculating damage animations
    //show number bar red or green
    //changing floor animation
}
