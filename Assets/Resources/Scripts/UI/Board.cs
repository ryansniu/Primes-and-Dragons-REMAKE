﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public static class NUMBAR_STATE
{
    public static readonly Color DEFAULT = new Color(0.8f, 0.8f, 0.8f);
    public static readonly Color SUCCESS = Color.green;
    public static readonly Color FAILURE = Color.red;
}
public class Board : MonoBehaviour
{
    private const int ROWS = 5;
    private const int COLUMNS = 6;
    private const int X_OFFSET = 7;
    private const int Y_OFFSET = 7;
    private const int ORB_LEN = 32;
    private const int ORB_SPACE = 2;
    private static float SCALE;
    private const float THRESHOLD = 0.4f;

    public static readonly float DISAPPEAR_DURATION = 0.25f;
    public static readonly WaitForSeconds DISAPPEAR_DELTA = new WaitForSeconds(0.05f);
    public static readonly Vector3 FALL_SPEED = new Vector3(0f, -4f);
    private bool isDarkened = true;
    public SpriteRenderer orbGridFG;

    public TextMeshPro numBar;
    public SpriteRenderer numBarBG;
    private Orb[][] orbArray = new Orb[COLUMNS][];
    private List<Orb> selectedOrbs = new List<Orb>();
    private float[] orbSpawnRates = new float[12];  //must always add up to 1

    private WaitUntil waitForInput;
    void Awake() {
        for (int i = 0; i < COLUMNS; i++) orbArray[i] = new Orb[ROWS];
        setDefaultOrbSpawnRates();

        if (Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.WindowsEditor
        || Application.platform == RuntimePlatform.OSXEditor
        || Application.platform == RuntimePlatform.OSXPlayer) waitForInput = new WaitUntil(() => Input.GetMouseButtonDown(0));
        else if (Application.platform == RuntimePlatform.Android) waitForInput = new WaitUntil(() => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        SCALE = Mathf.Min(Screen.width / 216f, Screen.height / 384f);

    }
    void Start() {
        setNumBarColor(NUMBAR_STATE.DEFAULT);
        StartCoroutine(fillBoard());
    }
    private bool inputReleased() {
        if (Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.WindowsEditor
        || Application.platform == RuntimePlatform.OSXEditor
        || Application.platform == RuntimePlatform.OSXPlayer) return !Input.GetMouseButton(0);
        else if (Application.platform == RuntimePlatform.Android) return Input.GetTouch(0).phase == TouchPhase.Ended;
        return true;
    }
    private Vector2 getRelativeInputPos() {
        Vector2 relativeInputPos = Vector2.zero;
        if (Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.WindowsEditor
        || Application.platform == RuntimePlatform.OSXEditor
        || Application.platform == RuntimePlatform.OSXPlayer) relativeInputPos = convertScreenToGridPos(Input.mousePosition);
        else if (Application.platform == RuntimePlatform.Android) relativeInputPos = convertScreenToGridPos(Input.GetTouch(0).position);
        return relativeInputPos;
    }
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
            while (!inputReleased() && !GameController.isPaused) {
                Vector2 relativeInputPos = getRelativeInputPos();
                int c = Mathf.RoundToInt(relativeInputPos.x);
                int r = Mathf.RoundToInt(relativeInputPos.y);
                if (0 <= r && r < ROWS && 0 <= c && c < COLUMNS && Mathf.Abs(relativeInputPos.x - c) <= THRESHOLD && Mathf.Abs(relativeInputPos.y - r) <= THRESHOLD) {
                    Orb chosenOrb = orbArray[c][r];
                    Vector2 currGridPosition = new Vector2(c, r);
                    if (selectedOrbs.Count == 0) {
                        selectedOrbs.Add(chosenOrb);
                        chosenOrb.isSelected = true;
                    }
                    else {
                        Orb head = selectedOrbs.Last();
                        selectedOrbs.Remove(selectedOrbs.Last());
                        Vector2 prevHeadDir = head.prevOrbDir;
                        head.prevOrbDir = Vector2.zero;
                        head.isSelected = false;
                        if (!(selectedOrbs.Count >= 1 && currGridPosition.Equals(selectedOrbs.Last().getGridPos()))) {  //if the player backtracks, then the head orb is removed
                            selectedOrbs.Add(head);
                            head.prevOrbDir = prevHeadDir;
                            head.isSelected = true;
                            if (!chosenOrb.Equals(head) && chosenOrb.isAdjacentTo(head) && !chosenOrb.isSelected) {  //if the player moves to a new, adjacent orb, then the new orb is put in
                                head.nextOrbDir = head.directionTo(chosenOrb);
                                selectedOrbs.Add(chosenOrb);
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
        for (int i = selectedOrbs.Count - 1; i >= 0; i--) {
            int value = selectedOrbs[i].getValue();
            string digit;
            switch (value) {
                case 10:
                    digit = "P";
                    break;
                case 11:
                    digit = "E";
                    break;
                default:
                    digit = value.ToString();
                    break;
            }
            input = string.Concat(digit, input);
        }
        return digitsOnly ? new string(input.Where(c => char.IsDigit(c)).ToArray()) : input;
    }
    public IEnumerator rmvAllOrbs(Vector2[] orbPos) {
        selectedOrbs.Clear();
        for (int i = 0; i < orbPos.Count(); i++) selectedOrbs.Add(orbArray[(int)orbPos[i].x][(int)orbPos[i].y]);
        while (selectedOrbs.Count != 0) {
            StartCoroutine(rmvNextOrb());
            yield return DISAPPEAR_DELTA;
        }
        yield return new WaitForSeconds(DISAPPEAR_DURATION);
        yield return StartCoroutine(fillBoard());
    }

    public IEnumerator rmvNextOrb() {
        Orb rmvOrb = selectedOrbs.First();
        selectedOrbs.Remove(selectedOrbs.First());
        Vector2 rmvPos = rmvOrb.getGridPos();
        orbArray[(int)rmvPos.x][(int)rmvPos.y].removeConnectorSprites();
        //disappear animation
        float disappearTimer = 0f;
        while (disappearTimer <= DISAPPEAR_DURATION) {
            orbArray[(int)rmvPos.x][(int)rmvPos.y].getWhiteRenderer().color = Color.Lerp(Color.clear, rmvOrb.sprWhiteColor, Mathf.SmoothStep(0f, 1f, disappearTimer / DISAPPEAR_DURATION));
            disappearTimer += Time.deltaTime;
            yield return null;
        }
        //remove the orb
        OrbPool.SharedInstance.ReturnToPool(orbArray[(int)rmvPos.x][(int)rmvPos.y].gameObject);
        orbArray[(int)rmvPos.x][(int)rmvPos.y] = null;
    }

    public IEnumerator fillBoard() {
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
        do {
            isFalling = false;
            for (int c = 0; c < COLUMNS; c++) {
                for (int r = 0; r < ROWS; r++) {
                    Vector2 currGridPos = orbArray[c][r].getGridPos();
                    Vector2 target = Board.convertGridToWorldPos(currGridPos);
                    Transform trans = orbArray[c][r].getTrans();
                    if (trans.position.y > target.y) {
                        trans.position += FALL_SPEED * Time.deltaTime;
                        isFalling = true;
                    }
                    else trans.position = target;
                }
            }
            yield return null;
        } while (isFalling);
    }
    public IEnumerator toggleForeground(bool darken) {
        if (isDarkened != darken) {
            float animationTime = 0.25f;
            float currTime = 0f;
            Color darkFG = new Color(0f, 0f, 0f, 0.5f);
            while (currTime < animationTime) {
                if (darken) orbGridFG.color = Color.Lerp(Color.clear, darkFG, currTime / animationTime);
                else orbGridFG.color = Color.Lerp(darkFG, Color.clear, currTime / animationTime);
                currTime += Time.deltaTime;
                yield return null;
            }
            if (darken) orbGridFG.color = darkFG;
            else orbGridFG.color = Color.clear;
            isDarkened = darken;
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

    private void displayNumBar() {  //TO-DO
        numBar.text = getInputNum(true);
    }

    public void setNumBarColor(Color c) {   //TO-DO
        numBarBG.color = c;
    }

    public void setOrb(int c, int r, ORB_VALUE val) {
        orbArray[c][r].changeValue(val);
    }
    public Orb getOrb(int c, int r) {
        return orbArray[c][r];
    }

    public void setDefaultOrbSpawnRates() {
        float[] defaultOrbSparnRates = {0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0, 0};
        setOrbSpawnRates(defaultOrbSparnRates);
    }
    public void setOrbSpawnRates(float[] newOrbSpawnRates) {
        if(newOrbSpawnRates.Count() == orbSpawnRates.Count()) orbSpawnRates = newOrbSpawnRates;
    }

    public static Vector2 convertGridToWorldPos(Vector2 gridPos) {
        Vector2 screenPos = new Vector2((gridPos.x + 0.5f) * (ORB_LEN + ORB_SPACE) + X_OFFSET - ORB_SPACE / 2, (gridPos.y + 0.5f) * (ORB_LEN + ORB_SPACE) + Y_OFFSET - ORB_SPACE / 2) * SCALE;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
    public static Vector2 convertScreenToGridPos(Vector2 screenPos) {
        return new Vector2(screenPos.x / SCALE - X_OFFSET + ORB_SPACE / 2, screenPos.y / SCALE - Y_OFFSET + ORB_SPACE / 2) / (ORB_LEN + ORB_SPACE) - new Vector2(0.1f, 0.1f) * SCALE;  //SUS
    }
}