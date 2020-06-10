using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboards : MonoBehaviour {
    private const string LEADERBOARD_DATA = "/lData.binary";

    [SerializeField] private LeaderboardInput input = default;
    [SerializeField] private WittyComment comment = default;
    private EntryList data;
    private List<LeaderboardItem> topTenItems;
    private bool fastForward = false;

    void Start() {
        data = GameData.readFile(LEADERBOARD_DATA) as EntryList;
        if (data == null) data = new EntryList();
        StartCoroutine(leaderboardAnim(addNewEntry()));
    }
    private int addNewEntry() {
        if (!PlayerPrefs.HasKey("Floor") || !PlayerPrefs.HasKey("Time")) return -1;
        int floor = PlayerPrefs.GetInt("Floor");
        PlayerPrefs.DeleteKey("Floor");
        double time = double.Parse(PlayerPrefs.GetString("Time"));
        PlayerPrefs.DeleteKey("Time");
        return data.addNewEntry(floor, time);
    }
    private IEnumerator leaderboardAnim(int newEntryIndex) {
        // exit loading screen
        yield return StartCoroutine(LoadingScreen.Instance.HideDelay());

        // get player name if new record
        if (newEntryIndex != -1) {
            yield return StartCoroutine(input.getInput());
            data.topTen[newEntryIndex].name = input.getName();
            GameData.writeFile(LEADERBOARD_DATA, data);
        }
        yield return StartCoroutine(input.exitInput());

        // display the leaderboard
        topTenItems = new List<LeaderboardItem>();
        StartCoroutine(detectPress());
        for (int i = 9; i >= 0 ; i--) {
            topTenItems.Add(LeaderboardItem.Create(i + 1, i < data.topTen.Count ? data.topTen[i] : null, i == newEntryIndex));
            for(float elapsedTime = 0f; elapsedTime <= (fastForward ? 0.05f : i >= data.topTen.Count ? 0.10f : 0.25f); elapsedTime += Time.deltaTime) yield return null;
        }
        StopCoroutine(detectPress());

        // display the witty comment
        yield return StartCoroutine(comment.displayComment());
    }
    private IEnumerator detectPress() { for (fastForward = false; ; fastForward = fastForward || Input.GetMouseButton(0) || Input.touchCount > 0) yield return null; }

    [Serializable]
    private class EntryList {
        private const int CAPACITY = 10;
        public List<LeaderboardEntry> topTen = new List<LeaderboardEntry>();

        public int addNewEntry(int floor, double time) {
            LeaderboardEntry entry = new LeaderboardEntry(floor, time);
            topTen.Add(entry);
            topTen.Sort();
            while (topTen.Count > CAPACITY) topTen.RemoveAt(topTen.Count - 1);
            return topTen.IndexOf(entry);
        }
    }
}


[Serializable] 
public class LeaderboardEntry : IComparable {
    public string name;
    public int floor;
    public double time;
    public LeaderboardEntry(int f, double t, string n = null) { name = n; floor = f; time = t; }
    public int CompareTo(object obj) {
        if (obj == null) return 1;
        LeaderboardEntry other = obj as LeaderboardEntry;
        if (other != null) {
            if (floor == other.floor) {
                if (time == other.time) {
                    if (name == null && other.name == null) return 0;
                    else if (name == null) return -1;
                    else if (other.name == null) return 1;
                    else return 0;
                } else return time.CompareTo(other.time);
            } else return floor.CompareTo(other.floor);
        } else throw new ArgumentException("Object is not a LeaderboardEntry");
    }
}

