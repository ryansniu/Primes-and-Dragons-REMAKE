using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WittyComment : MonoBehaviour {
    public TextMeshProUGUI header;
    public TextMeshProUGUI comment;

    public WittyCommentData getWittyComment() {
        // player prefs
        return new WittyCommentData("");
    }

    public IEnumerator displayComment(WittyCommentData wcd) {
        yield return StartCoroutine(rollingText(header, wcd, 0));
        for(int i = 1; i < wcd.size; i++) yield return StartCoroutine(rollingText(comment, wcd, i));
    }
    private IEnumerator rollingText(TextMeshProUGUI tmp, WittyCommentData wcd, int index) {
        yield return null;
    }
}

public class WittyCommentData {
    public int size;
    public List<string> texts;
    public List<Color> colors;
    public List<float> speeds;
    public List<float> endlag;

    public WittyCommentData(string file) {
        // organize text file into the lists
    }
}
