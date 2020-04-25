using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Leaderboards : MonoBehaviour {
    private LeaderboardData data;
    private List<LeaderboardItem> topTenItems;

    void Start() {
        StartCoroutine(LoadingScreen.Instance.HideDelay());
        if (!loadLeaderboardData()) data = new LeaderboardData();
        int newEntry = data.addEntry(recieveDataFromGameController());
        if (newEntry != -1) updateLeaderboardData();
        topTenItems = new List<LeaderboardItem>();
        for (int i = 0; i < 10; i++) topTenItems.Add(LeaderboardItem.Create(i + 1, i < data.topTenEntries.Count ? data.topTenEntries[i] : null, i == newEntry));
    }
    private LeaderboardEntry recieveDataFromGameController() {
        int floor = PlayerPrefs.GetInt("Floor");
        PlayerPrefs.DeleteKey("Floor");
        double time = double.Parse(PlayerPrefs.GetString("Time"));
        PlayerPrefs.DeleteKey("Time");
        return new LeaderboardEntry("PLACEHLD", floor, time);
    }
    private bool loadLeaderboardData() {
        if (!Directory.Exists("Leaderboard Data")) return false;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream dataFile = File.Open("Leaderboard Data/data.binary", FileMode.OpenOrCreate);
        if (dataFile.Length == 0) {
            dataFile.Close();
            return false;
        }

        data = formatter.Deserialize(dataFile) as LeaderboardData;
        dataFile.Close();
        return true;
    }
    private void updateLeaderboardData() {
        if (!Directory.Exists("Leaderboard Data")) Directory.CreateDirectory("Leaderboard Data");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream dataFile = File.Create("Leaderboard Data/data.binary");
        formatter.Serialize(dataFile, data);
        dataFile.Close();
    }
}

[System.Serializable]
public class LeaderboardData {
    public List<LeaderboardEntry> topTenEntries;

    public int addEntry(LeaderboardEntry entry) {
        if (topTenEntries == null) topTenEntries = new List<LeaderboardEntry>();
        topTenEntries.Add(entry);
        topTenEntries.Sort((e1, e2) => -(e1.floor).CompareTo(e2.floor) != 0 ? -(e1.floor).CompareTo(e2.floor) 
                                        : ((e1.time).CompareTo(e2.time) != 0 ? (e1.time).CompareTo(e2.time)
                                        : (e1 == entry ? -1 : 1)));
        while(topTenEntries.Count > 10) topTenEntries.RemoveAt(topTenEntries.Count - 1);
        return topTenEntries.IndexOf(entry);
    }
}

[System.Serializable]
public class LeaderboardEntry {
    public string name;
    public int floor;
    public double time;

    public LeaderboardEntry(string n, int f, double t) {
        name = n;
        floor = f;
        time = t;
    }
}