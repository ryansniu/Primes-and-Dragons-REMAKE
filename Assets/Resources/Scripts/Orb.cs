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
    private const string SPRITE_PATH = "Sprites/Orbs/orb";
    private SpriteRenderer spr;
    private ORB_VALUE value;
    private Vector2 currGridPos;

    public bool isSelected = false;
    public Vector2 prevOrbDir = Vector2.zero;
    public Vector2 nextOrbDir = Vector2.zero;

    public static Orb Create(Vector3 pos, ORB_VALUE val) {  //gotta insert column or something
        Orb orb = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), pos, Quaternion.identity, OrbPool.SharedInstance.transform) as GameObject).GetComponent<Orb>();
        orb.setInitValues(pos, val);
        return orb;
    }
    public void setInitValues(Vector3 pos, ORB_VALUE val) { 
        currGridPos = pos;
        transform.position = Board.convertGridToRealPos(currGridPos);
        isSelected = false;
        prevOrbDir = Vector2.zero;
        nextOrbDir = Vector2.zero;
        changeValue(val);
    }

    public void changeValue(ORB_VALUE val) {
        value = val;
        string orbName = Convert.ToString((int)value * 2 + 1).PadLeft(2, '0');
        spr.sprite = Resources.Load<Sprite>(string.Concat(SPRITE_PATH, orbName));
        name = value.ToString();
    }
    void Awake() {
        spr = GetComponent<SpriteRenderer>();
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
        //ADD CONNECTOR SPRTES AND CHANGE THE LIGHT TO DARK
        
        int spriteNum = (int)value * 2 + (isSelected ? 0 : 1);
        string orbName = Convert.ToString(spriteNum).PadLeft(2, '0');
        spr.sprite = Resources.Load<Sprite>(string.Concat(SPRITE_PATH, orbName));
    }
    //disappear animation
    //fall animation
    //land animation
}
