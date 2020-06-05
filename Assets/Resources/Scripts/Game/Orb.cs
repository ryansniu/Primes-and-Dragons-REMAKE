﻿using System;
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
    POISON = 10,
    EMPTY = 11,
    NULLIFY = 12,
    STOP = 13
}

public class Orb : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/Orb";
    private const string ORB_PATH = "Sprites/Orbs";
    private const string CONNECTOR_PATH = "Sprites/Connectors";

    public static bool BOARD_IS_NULLIFIED = false;

    private SpriteRenderer spr;
    private SpriteRenderer sprWhite;
    private SpriteRenderer sprMarker;  // TO-DO: fix this sprite
    public Color sprWhiteColor;
    private Sprite[] orbSprites;
    private Sprite[] connectorSprites;

    private ORB_VALUE value;
    private Vector2 currGridPos;
    private Transform trans;

    public bool isSelected = false;
    public Vector2 prevOrbDir = Vector2.zero;
    private SpriteRenderer prevConnector;
    public Vector2 nextOrbDir = Vector2.zero;
    private SpriteRenderer nextConnector;
    private Vector2[] orbDirs = { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1) };

    public static Orb Create(Vector3 spawnGridPos, int fallDist, ORB_VALUE val) {
        Orb orb = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnGridPos, Quaternion.identity, OrbPool.SharedInstance.transform) as GameObject).GetComponent<Orb>();
        orb.setInitValues(spawnGridPos, fallDist, val);
        return orb;
    }
    public void setInitValues(Vector3 spawnGridPos, int fallDist, ORB_VALUE val) {
        currGridPos = spawnGridPos - new Vector3(0, fallDist);
        trans.position = Board.convertGridToWorldPos(spawnGridPos);  // TO-DO: SUS

        isSelected = false;
        prevOrbDir = Vector2.zero;
        nextOrbDir = Vector2.zero;
        sprWhite.color = Color.clear;
        sprMarker.color = Color.clear;

        changeValue(val);
        updateConnectors();
    }
    public void incrementValue(int offset) {
        int newVal = (int)value < 10 ? Mathf.Clamp((int)value + offset, 0 , 9) : (int)value;
        if(newVal != (int)value) {
            Vector3 deltaNumSpawn = new Vector3(trans.position.x*5 - 55, trans.position.y*5, 2);
            HPDeltaNum.Create(deltaNumSpawn, offset);
            changeValue((ORB_VALUE)newVal);
        }
    }
    public void changeValue(ORB_VALUE val) {
        if (value == val) return;
        value = val;
        spr.sprite = orbSprites[(int)value * 2 + 1];
        name = value.ToString();

        if(value == ORB_VALUE.ZERO) sprWhiteColor = ColorPalette.getColor(12, 1);
        else if(value == ORB_VALUE.POISON) sprWhiteColor = ColorPalette.getColor(1, 1);
        else if(value == ORB_VALUE.EMPTY) sprWhiteColor = ColorPalette.getColor(2, 2);
        else if (value == ORB_VALUE.NULLIFY) sprWhiteColor = ColorPalette.getColor(4, 1);
        else if (value == ORB_VALUE.STOP) sprWhiteColor = ColorPalette.getColor(0, 2);
        else sprWhiteColor = Color.white;
    }
    void Awake() {
        trans = transform;

        spr = GetComponent<SpriteRenderer>();
        sprWhite = trans.Find("White").GetComponent<SpriteRenderer>();
        sprMarker = trans.Find("Marker").GetComponent<SpriteRenderer>();
        prevConnector = trans.Find("Connector-Prev").GetComponent<SpriteRenderer>();
        nextConnector = trans.Find("Connector-Next").GetComponent<SpriteRenderer>();

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
    }
    public Vector2 getGridPos() {
        return currGridPos;
    }
    public Transform getTrans(){
        return trans;
    }
    public void updateConnectors() {
        if (isSelected && prevOrbDir.Equals(Vector2.zero) && nextOrbDir.Equals(Vector2.zero)) {
            prevConnector.sprite = connectorSprites[0];
            nextConnector.sprite = null;
        }
        else {
            if (prevOrbDir.Equals(Vector2.zero)) prevConnector.sprite = null;
            else {
                int prevConnectorIndex = (nextOrbDir.Equals(Vector2.zero) ? 0 : 1) + (BOARD_IS_NULLIFIED ? 6 : 0);
                for (int i = 0; i < orbDirs.Length; i++) {
                    if (prevOrbDir.Equals(orbDirs[i])) {
                        prevConnectorIndex += 2 * ((i % 2) + 1);
                        prevConnector.sprite = connectorSprites[prevConnectorIndex];
                        prevConnector.transform.rotation = Quaternion.Euler(0, 0, 90 * (i / 2));
                    }
                }
            }
            if (nextOrbDir.Equals(Vector2.zero)) nextConnector.sprite = null;
            else {
                int nextConnectorIndex = (prevOrbDir.Equals(Vector2.zero) ? 0 : 1) + (BOARD_IS_NULLIFIED ? 6 : 0);
                for (int i = 0; i < orbDirs.Length; i++) {
                    if (nextOrbDir.Equals(orbDirs[i])) {
                        nextConnectorIndex += 2 * ((i % 2) + 1);
                        nextConnector.sprite = connectorSprites[nextConnectorIndex];
                        nextConnector.transform.rotation = Quaternion.Euler(0, 0, 90 * (i / 2));
                    }
                }
            }
        }
        spr.sprite = orbSprites[(int)value * 2 + (isSelected ? 0 : 1)];
    }
    public void removeConnectorSprites(){
        prevConnector.sprite = null;
        nextConnector.sprite = null;
    }
    public SpriteRenderer getWhiteRenderer(){
        return sprWhite;
    }
    public void toggleOrbMarker(bool markerOn) {
        sprMarker.color = markerOn ? Color.white : Color.clear;
    }
}
