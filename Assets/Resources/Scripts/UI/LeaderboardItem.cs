using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour {
    private static readonly Vector3 SPAWN_POS = new Vector3(70.2f, 1653f, 0);
    private const string PREFAB_PATH = "Prefabs/LeaderboardItem";
    public TextMeshProUGUI numText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI timeText;

    private Transform trans;
    private bool newRecord;
    public static LeaderboardItem Create(int num, LeaderboardEntry entry, bool isNewRecord) {
        Vector3 spawnPos = new Vector3(SPAWN_POS.x, SPAWN_POS.y - 90 * (num - 1), SPAWN_POS.z);
        LeaderboardItem li = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<LeaderboardItem>();
        li.initValues(num, entry, isNewRecord);
        return li;
    }
    void Awake() {
        trans = transform;
        trans.SetParent(GameObject.Find("Leaderboard Canvas").GetComponent<Canvas>().transform);
    }

    private void initValues(int num, LeaderboardEntry entry, bool isNewRecord) {
        newRecord = isNewRecord;
        Color textColor = Color.white;
        if (newRecord) textColor = Color.red;
        else if (num == 1) textColor = Color.yellow;
        else if (num == 2) textColor = Color.gray;
        else if (num == 3) textColor = Color.blue; //bronze
        numText.color = textColor;
        nameText.color = textColor;
        floorText.color = textColor;
        timeText.color = textColor;

        numText.text = num + ".";

        if (entry == null) return;
        nameText.text = entry.name.ToLower();
        floorText.text = entry.floor.ToString();
        timeText.text = TimeSpan.FromSeconds(entry.time).ToString(@"hh\:mm\:ss");
    }
}
