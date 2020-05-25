using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageBar : MonoBehaviour {
    public Image BG;
    public TextMeshProUGUI sumText;
    public TextMeshProUGUI lenText;
    public TextMeshProUGUI dmgText;

    private const float FADE_ANIM_TIME = 0.25f;
    private int currSum;
    private int currLen;
    private int currDmg;

    private bool isDisplayed = false;
    public void displayText(bool toDisplay){
        sumText.text = toDisplay ? currSum.ToString() : "";
        lenText.text = toDisplay ? currLen.ToString() : "";
        dmgText.text = toDisplay ? currDmg.ToString() : "";

        if (isDisplayed != toDisplay) {
            isDisplayed = toDisplay;
            StartCoroutine(fadeAnimation(toDisplay));
        }
    }
    private IEnumerator fadeAnimation(bool fadeIn) {
        Color endColor = fadeIn ? Color.white : Color.clear;
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            float timeRatio = fadeIn ? currTime / FADE_ANIM_TIME : 1f - currTime / FADE_ANIM_TIME;
            Color c = Color.Lerp(Color.clear, Color.white, Mathf.SmoothStep(0f, 1f, timeRatio));
            sumText.color = c;
            lenText.color = c;
            dmgText.color = c;
            BG.color = c;
            yield return null;
        }
        sumText.color = endColor;
        lenText.color = endColor;
        dmgText.color = endColor;
        BG.color = endColor;
        yield return null;
    }
    public void addNextDigit(int digit){
        currLen++;
        currSum += digit;
        currDmg = currSum*currLen;
        displayText(true);
    }
    public void resetValues(){
        currSum = 0;
        currLen = 0;
        currDmg = 0;
        displayText(false);
    }
    public int getCurrDamage(){
        return currDmg;
    }
}
