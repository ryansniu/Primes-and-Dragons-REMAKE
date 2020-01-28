using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/LeaderboardItem";
    public TextMeshProUGUI numText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI timeText;

    private Transform trans;
    private bool newRecord;
    public static LeaderboardItem Create(Vector3 spawnPos, int num, string name, int floor, string time, bool isNewRecord) {
        LeaderboardItem li = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<LeaderboardItem>();
        li.initValues(num, name, floor, time, isNewRecord);
        return li;
    }
    void Awake() {
        trans = transform;
    }

    private void initValues(int num, string name, int floor, string time, bool isNewRecord) {
        newRecord = isNewRecord;
        Color textColor = Color.white;
        if (newRecord) textColor = Color.red;
        else if (num == 1) textColor = Color.yellow;
        else if (num == 2) textColor = Color.gray;
        else if (num == 3) textColor = Color.blue; //bronze
        numText.text = num + ".";
        nameText.text = name.ToLower();
        floorText.text = floor.ToString();
        timeText.text = time;
    }
}
