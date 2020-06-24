using System;
using System.Collections.Generic;
using UnityEngine;

public enum ORB_VALUE {
    ZERO, ONE, TWO, THREE, FOUR,
    FIVE, SIX, SEVEN, EIGHT, NINE,
    POISON, EMPTY, NULLIFY, STOP
}

[Serializable]
public class OrbState {
    public ORB_VALUE value;
    public List<string> isMarkedBy = new List<string>();
}

public class Orb : MonoBehaviour {
    public static bool BOARD_IS_NULLIFIED = false;
    private const string PREFAB_PATH = "Prefabs/Orb";
    private const string ORB_PATH = "Sprites/Orbs";
    private const string CONNECTOR_PATH = "Sprites/Connectors";

    [SerializeField] private SpriteRenderer spr = default, sprMarker = default, sprWhite = default;
    [HideInInspector] public Color sprWhiteColor;
    private Sprite[] orbSprites, connectorSprites;

    private OrbState currState = new OrbState();
    private Vector2Int currGridPos;
    private Transform trans;

    private bool isHighlighted = false;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public Vector2Int prevOrbDir, nextOrbDir;
    [SerializeField] private SpriteRenderer prevConnector = default, nextConnector = default;
    private Vector2Int[] orbDirs = { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) };

    public OrbState getState() => currState;
    public void setState(OrbState os) {
        currState = os;
        updateMarkerSprite();
    }

    public static Orb Create(Vector3 spawnGridPos, int fallDist, ORB_VALUE val) {
        Orb orb = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH), spawnGridPos, Quaternion.identity, OrbPool.Instance.transform)).GetComponent<Orb>();
        orb.setInitValues(spawnGridPos, fallDist, val);
        return orb;
    }
    public void setInitValues(Vector3 spawnGridPos, int fallDist, ORB_VALUE val) {
        Vector2Int prevGridPos = new Vector2Int((int)spawnGridPos.x, (int)spawnGridPos.y);
        currGridPos = new Vector2Int(prevGridPos.x, prevGridPos.y - fallDist);
        trans.position = Board.convertGridToWorldPos(prevGridPos);  // TO-DO: SUS when changing resolutions

        isSelected = false;
        isHighlighted = false;
        prevOrbDir = nextOrbDir = Vector2Int.zero;
        sprWhite.color = Color.clear;

        changeValue(val);
        currState.isMarkedBy = new List<string>();
        updateMarkerSprite();
        updateConnectors();
    }
    void Awake() {
        trans = transform;

        orbSprites = Resources.LoadAll<Sprite>(ORB_PATH);
        connectorSprites = Resources.LoadAll<Sprite>(CONNECTOR_PATH);
    }

    public ORB_VALUE getOrbValue() => currState.value;
    public int getIntValue() => (int)(currState.value);
    public bool isDigit() => currState.value < ORB_VALUE.POISON;
    public bool isEven() => isDigit() && getIntValue() % 2 == 0;
    public bool isOdd() => isDigit() && getIntValue() % 2 == 1;
    public void changeValue(ORB_VALUE val) {
        if (currState.value == val) return;
        currState.value = val;
        spr.sprite = orbSprites[getIntValue() * 2 + 1];
        name = currState.value.ToString();
        switch (currState.value) {
            case ORB_VALUE.ZERO: sprWhiteColor = ColorPalette.getColor(12, 1); break;
            case ORB_VALUE.POISON: sprWhiteColor = ColorPalette.getColor(1, 1); break;
            case ORB_VALUE.EMPTY: sprWhiteColor = ColorPalette.getColor(2, 2); break;
            case ORB_VALUE.NULLIFY: sprWhiteColor = ColorPalette.getColor(3, 1); break;
            case ORB_VALUE.STOP: sprWhiteColor = ColorPalette.getColor(0, 2); break;
            default: sprWhiteColor = Color.white; break;
        }
    }
    public void incrementValue(int offset) {
        ORB_VALUE newVal = isDigit() ? (ORB_VALUE)Mathf.Clamp(getIntValue() + offset, 0 , 9) : currState.value;
        if(newVal != currState.value) {
            Vector3 deltaNumSpawn = new Vector3(trans.position.x * 5 - 55f, trans.position.y * 5, 2f);
            HPDeltaNum.Create(deltaNumSpawn, offset);
            changeValue(newVal);
        }
    }

    public Transform getTrans() => trans;
    public Vector2Int getGridPos() => currGridPos;
    public void setGridPos(Vector2Int newGridPos) => currGridPos = newGridPos;
    public Vector2Int directionTo(Orb other) => other.currGridPos - currGridPos;
    public bool isAdjacentTo(Orb other) {
        Vector2Int dirTo = directionTo(other);
        return Mathf.Abs(dirTo.x) <= 1 && Mathf.Abs(dirTo.y) <= 1;
    }

    public void updateConnectors() {
        if (isSelected && prevOrbDir.Equals(Vector2Int.zero) && nextOrbDir.Equals(Vector2Int.zero)) {
            prevConnector.sprite = connectorSprites[0];
            nextConnector.sprite = null;
        }
        else {
            updateConnectorHelper(true, prevOrbDir, prevConnector, nextOrbDir == Vector2Int.zero);
            updateConnectorHelper(false, nextOrbDir, nextConnector, prevOrbDir == Vector2Int.zero);
        }
        spr.sprite = orbSprites[getIntValue() * 2 + (isSelected ? 0 : 1)];
    }
    private void updateConnectorHelper(bool intoOrb, Vector2Int orbDir, SpriteRenderer connectorSpr, bool isEnd) {
        if (orbDir.Equals(Vector2Int.zero)) connectorSpr.sprite = null;
        else {
            int connectorIndex = (intoOrb ? 2 : 0) + (isEnd ? 0 : 1) + (BOARD_IS_NULLIFIED ? 10 : 0);
            for (int i = 0; i < orbDirs.Length; i++) {
                if (orbDir.Equals(orbDirs[i])) {
                    connectorIndex += 2 + 4 * (i % 2);
                    connectorSpr.sprite = connectorSprites[connectorIndex];
                    connectorSpr.transform.rotation = Quaternion.Euler(0, 0, 90 * (i / 2));
                }
            }
        }
    }
    public void removeConnectorSprites(){ prevConnector.sprite = nextConnector.sprite = null; }

    public SpriteRenderer getWhiteRenderer() => sprWhite;
    public bool getIsHighlighted() => isHighlighted;
    public void toggleOrbHighlight(bool highlightOn) {
        isHighlighted = highlightOn;
        updateMarkerSprite();
    }
    public bool getIsMarkedBy(string enemyID) => currState.isMarkedBy.Contains(enemyID);
    public void toggleOrbMarker(string enemyID, bool markerOn) {
        if (markerOn && !currState.isMarkedBy.Contains(enemyID)) currState.isMarkedBy.Add(enemyID);
        else currState.isMarkedBy.Remove(enemyID);
        updateMarkerSprite();
    }
    private void updateMarkerSprite() {
        if (isHighlighted) sprMarker.sprite = orbSprites[30];
        else if (currState.isMarkedBy.Count > 0) sprMarker.sprite = orbSprites[29];
        else sprMarker.sprite = null;
    }

}
