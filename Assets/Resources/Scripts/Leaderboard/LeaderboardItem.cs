using System;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour {
    private static readonly Vector3 SPAWN_POS = new Vector3(-462.5f, 673f, 0);
    private const string PREFAB_PATH = "Prefabs/UI/LeaderboardItem";
    public TextMeshProUGUI numText, nameText, floorText, timeText;

    private Transform trans;
    private bool newRecord;
    public static LeaderboardItem Create(int num, LeaderboardEntry entry, bool isNewRecord) {
        Vector3 spawnPos = new Vector3(SPAWN_POS.x, SPAWN_POS.y - 90 * (num - 1), SPAWN_POS.z);
        LeaderboardItem li = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH), spawnPos, Quaternion.identity)).GetComponent<LeaderboardItem>();
        li.initValues(num, entry, isNewRecord);
        return li;
    }
    void Awake() {
        trans = transform;
        trans.SetParent(GameObject.Find("Leaderboard Items").transform, false);
    }

    private void initValues(int num, LeaderboardEntry entry, bool isNewRecord) {
        newRecord = isNewRecord;
        Color textColor = Color.white;
        if (newRecord) textColor = ColorPalette.getColor(4, 1);
        else if (num == 1) textColor = ColorPalette.getColor(3, 1);
        else if (num == 2) textColor = ColorPalette.getColor(2, 2);
        else if (num == 3) textColor = ColorPalette.getColor(12, 3);
        numText.color = nameText.color = floorText.color = timeText.color =textColor;
        numText.text = num + ".";

        if (entry == null) return;
        nameText.text = entry.name.ToLower();
        floorText.text = entry.floor.ToString();
        timeText.text = TimeSpan.FromSeconds(entry.time).ToString(@"hh\:mm\:ss");
    }
}
