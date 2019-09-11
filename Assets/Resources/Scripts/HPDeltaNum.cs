using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPDeltaNum : MonoBehaviour{
    private const string PREFAB_PATH = "Prefabs/HPDeltaNum";
    public static HPDeltaNum Create(Vector3 spawnPos, int value) {
        HPDeltaNum hpdn = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<HPDeltaNum>();
        return hpdn;
    }
    //animation
}
