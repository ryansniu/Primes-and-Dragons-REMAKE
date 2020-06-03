using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;

public class DamageBar : MonoBehaviour {
    public CanvasGroup cGroup;
    public Slider BG;
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
        BG.value = 0.05f * Mathf.Pow(currDmg, 1/3f);
        if (isDisplayed != toDisplay) {
            isDisplayed = toDisplay;
            StartCoroutine(fadeAnimation(toDisplay));
        }
    }
    private IEnumerator fadeAnimation(bool fadeIn) {
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            cGroup.alpha = fadeIn ? currTime / FADE_ANIM_TIME : 1f - currTime / FADE_ANIM_TIME;
            yield return null;
        }
        cGroup.alpha = fadeIn ? 1f : 0f;
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
        BG.value = 0;
        displayText(false);
    }
    public int getCurrDamage(){
        return currDmg;
    }
}