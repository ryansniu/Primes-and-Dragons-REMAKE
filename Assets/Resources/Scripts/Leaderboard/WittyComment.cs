using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class WittyComment : MonoBehaviour {
    private static readonly string PLAYER_PREF_KEY = "Death";
    [SerializeField] private TextMeshProUGUI header, comment;
    private bool fastForward = false;

    public WittyCommentData getWittyComment() {
        if (!PlayerPrefs.HasKey(PLAYER_PREF_KEY)) return new WittyCommentData(WittyCommentData.DEFAULT_DIR);
        string death = PlayerPrefs.GetString(PLAYER_PREF_KEY);
        PlayerPrefs.DeleteKey(PLAYER_PREF_KEY);
        return new WittyCommentData(death);
    }

    public IEnumerator displayComment(WittyCommentData wcd) {
        if (wcd == null || wcd.size == 0) yield break;
        StartCoroutine(detectPress());
        yield return StartCoroutine(rollingText(header, wcd, 0));
        for(int i = 1; i < wcd.size; i++) yield return StartCoroutine(rollingText(comment, wcd, i));
        StopCoroutine(detectPress());
    }
    private IEnumerator rollingText(TextMeshProUGUI tmp, WittyCommentData wcd, int index) {
        string text = wcd.texts[index];
        string color = wcd.colors[index];
        float speed = wcd.speeds[index];
        float endlag = wcd.endlag[index];

        tmp.text += "<color=#"+ color +">";
        foreach(char c in text) {
            tmp.text += c;
            yield return new WaitForSeconds(fastForward ? speed / 3f : speed);
        }
        tmp.text += "</color>";

        yield return new WaitForSeconds(fastForward ? endlag/3f : endlag);
    }

    private IEnumerator detectPress() {
        for (fastForward = false; ; fastForward = fastForward || Input.GetMouseButton(0) || Input.touchCount > 0) yield return null;
    }
}
 
public class WittyCommentData {
    public static readonly string DEFAULT_DIR = "Default/";
    private static readonly string WITTY_COMMENT_DIR = "Assets/Resources/WittyComments/";
    private static readonly string[] DELIMITER = { "| |" };

    private static readonly string DEFAULT_COLOR = "FFFFFF";
    private static readonly float DEFAULT_SPEED = 0.02f;
    private static readonly float DEFAULT_DELAY = 0.5f;

    public int size = 0;
    
    public List<string> texts = new List<string>();
    public List<string> colors = new List<string>();
    public List<float> speeds = new List<float>();
    public List<float> endlag = new List<float>();

    public WittyCommentData(string file) {
        string dir = Directory.Exists(WITTY_COMMENT_DIR + file) ? WITTY_COMMENT_DIR + file : WITTY_COMMENT_DIR + DEFAULT_DIR;
        string[] files = Directory.GetFiles(dir, "*.txt");
        if (files.Length == 0) return;

        StreamReader reader = new StreamReader(files[Random.Range(0, files.Length)]);
        size = int.Parse(reader.ReadLine());
        for(int i = 0; i < size; i++) {
            string[] words = reader.ReadLine().Split(DELIMITER, System.StringSplitOptions.None);
            texts.Add(words[0]);
            if (words.Length > 1 && words[1] != "default") colors.Add(words[1]);
            else colors.Add(DEFAULT_COLOR);
            if (words.Length > 2 && words[2] != "default") speeds.Add(float.Parse(words[2]));
            else speeds.Add(DEFAULT_SPEED);
            if (words.Length > 3 && words[3] != "default") endlag.Add(float.Parse(words[3]));
            else endlag.Add(DEFAULT_DELAY);
        }
    }
}
