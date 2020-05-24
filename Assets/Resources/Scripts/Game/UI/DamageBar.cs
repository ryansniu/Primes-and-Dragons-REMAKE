using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageBar : MonoBehaviour {
    public Image BG;
    public TextMeshProUGUI SumText;
    public TextMeshProUGUI LenText;
    public TextMeshProUGUI DmgText;

    private const float FADE_ANIM_TIME = 0.25f;
    private int currSum;
    private int currLen;
    private int currDmg;

    private bool isDisplayed = false;
    public void displayText(bool toDisplay){
        SumText.text = toDisplay ? currSum.ToString() : "";
        LenText.text = toDisplay ? currLen.ToString() : "";
        DmgText.text = toDisplay ? currDmg.ToString() : "";

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
            SumText.color = c;
            LenText.color = c;
            DmgText.color = c;
            BG.color = c;
            yield return null;
        }
        SumText.color = endColor;
        LenText.color = endColor;
        DmgText.color = endColor;
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
