using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ORB_VALUE {
    ZERO = 0,
    ONE = 1,
    TWO = 2,
    THREE = 3,
    FOUR = 4,
    FIVE = 5,
    SIX = 6,
    SEVEN = 7,
    EIGHT = 8,
    NINE = 9,
    EMPTY = 10,
    POISON = 11
}

public class Orb : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/Orb";
    private const string ORB_PATH = "Sprites/Orbs";
    private const string CONNECTOR_PATH = "Sprites/Connectors";

    private SpriteRenderer spr;
    private Sprite[] orbSprites;
    private Sprite[] connectorSprites;

    private ORB_VALUE value;
    private Vector2 currGridPos;

    public bool isSelected = false;
    public Vector2 prevOrbDir = Vector2.zero;
    private SpriteRenderer prevConnector;
    public Vector2 nextOrbDir = Vector2.zero;
    private SpriteRenderer nextConnector;
    private Vector2[] orbDirs = {new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1)};

    public static Orb Create(Vector3 pos, ORB_VALUE val) {  //gotta insert column or something
        Orb orb = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), pos, Quaternion.identity, OrbPool.SharedInstance.transform) as GameObject).GetComponent<Orb>();
        orb.setInitValues(pos, val);
        return orb;
    }
    public void setInitValues(Vector3 pos, ORB_VALUE val) { 
        currGridPos = pos;
        transform.position = Board.convertGridToRealPos(currGridPos);  //sus

        isSelected = false;
        prevOrbDir = Vector2.zero;
        nextOrbDir = Vector2.zero;

        changeValue(val);
        updateConnectors();
    }

    public void changeValue(ORB_VALUE val) {
        value = val;
        spr.sprite = orbSprites[(int)value * 2 + 1];
        name = value.ToString();
    }
    void Awake() {
        spr = GetComponent<SpriteRenderer>();
        prevConnector = transform.Find("Connector-Prev").GetComponent<SpriteRenderer>();
        nextConnector = transform.Find("Connector-Next").GetComponent<SpriteRenderer>();

        orbSprites = Resources.LoadAll<Sprite>(ORB_PATH);
        connectorSprites = Resources.LoadAll<Sprite>(CONNECTOR_PATH);
    }

    public Vector2 directionTo(Orb other) {
        return other.currGridPos - this.currGridPos;
    }
    public bool isAdjacentTo(Orb other) {
        Vector2 dirTo = directionTo(other);
        return Mathf.Abs(dirTo.x) <= 1 && Mathf.Abs(dirTo.y) <= 1;
    }

    public int getValue() {
        return (int)value;
    }
    public void setGridPos(Vector2 newGridPos) {
        currGridPos = newGridPos;
        transform.position = Board.convertGridToRealPos(currGridPos);
    }
    public Vector2 getGridPos() {
        return currGridPos;
    }
    
    public void updateConnectors() {
        if(isSelected && prevOrbDir.Equals(Vector2.zero) && nextOrbDir.Equals(Vector2.zero)) prevConnector.sprite = connectorSprites[0];
        else {
            if (prevOrbDir.Equals(Vector2.zero)) prevConnector.sprite = null;
            else {
                int prevConnectorIndex = nextOrbDir.Equals(Vector2.zero) ? 0 : 1;
                for (int i = 0; i < orbDirs.Length; i++) if (prevOrbDir.Equals(orbDirs[i])) prevConnectorIndex += 2 * (i + 1);
                prevConnector.sprite = connectorSprites[prevConnectorIndex];
            }
            if (nextOrbDir.Equals(Vector2.zero)) nextConnector.sprite = null;
            else {
                int nextConnectorIndex = prevOrbDir.Equals(Vector2.zero) ? 0 : 1;
                for (int i = 0; i < orbDirs.Length; i++) if (nextOrbDir.Equals(orbDirs[i])) nextConnectorIndex += 2 * (i + 1);
                nextConnector.sprite = connectorSprites[nextConnectorIndex];
            }
        }
        spr.sprite = orbSprites[(int)value*2 + (isSelected ? 0 : 1)];
    }
    //disappear animation
    //fall animation
    //land animation
}
