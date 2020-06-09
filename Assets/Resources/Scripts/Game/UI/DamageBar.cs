using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageBar : MonoBehaviour {
    [SerializeField] private CanvasGroup cGroup;
    [SerializeField] private Slider BG;
    [SerializeField] private TextMeshProUGUI sumText, lenText, dmgText;

    private const float FADE_ANIM_TIME = 0.25f;
    private int currSum, currLen, currDmg;

    private bool isDisplayed = false;
    public void displayText(bool toDisplay){
        sumText.text = toDisplay ? currSum.ToString() : "";
        lenText.text = toDisplay ? currLen.ToString() : "";
        dmgText.text = toDisplay ? currDmg.ToString() : "";
        BG.value = 0.05f * Mathf.Pow(currDmg, 1/3f);
        if (isDisplayed != toDisplay) {
            isDisplayed = toDisplay;
            StartCoroutine(fadeAnimation(isDisplayed = toDisplay));
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
        currDmg = currSum * currLen;
        displayText(true);
    }
    public void resetValues(){
        BG.value = currSum = currLen = currDmg = 0;
        displayText(false);
    }
    public int getCurrDamage(){
        return currDmg;
    }
}