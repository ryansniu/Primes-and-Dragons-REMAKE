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

    private int currSum;
    private int currLen;
    private int currDmg;
    public void displayText(bool toDisplay){
        Color textColor = toDisplay ? Color.white : Color.clear;
        SumText.color = textColor;
        LenText.color = textColor;
        DmgText.color = textColor;
        BG.color = textColor;
        SumText.text = toDisplay ? currSum.ToString() : "";
        LenText.text = toDisplay ? currLen.ToString() : "";
        DmgText.text = toDisplay ? currDmg.ToString() : "";
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
