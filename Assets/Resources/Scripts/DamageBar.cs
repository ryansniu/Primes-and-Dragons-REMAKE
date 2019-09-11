using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageBar : MonoBehaviour
{
    public TextMeshPro SumText;
    public TextMeshPro LenText;
    public TextMeshPro DmgText;

    private int currSum;
    private int currLen;
    private int currDmg;
    public void displayText(bool defaultText){
        Color textColor = defaultText ? Color.gray : Color.black;
        SumText.color = textColor;
        LenText.color = textColor;
        DmgText.color = textColor;
        SumText.text = defaultText ? "sum" : currSum.ToString();
        LenText.text = defaultText ? "len" : currLen.ToString();
        DmgText.text = defaultText ? "dmg" : currDmg.ToString();
    }
    public void addNextDigit(int digit){
        currLen++;
        currSum += digit;
        currDmg = currSum*currLen;
        displayText(false);
    }
    public void resetValues(){
        currSum = 0;
        currLen = 0;
        currDmg = 0;
        displayText(true);
    }

    public int getCurrDamage(){
        return currDmg;
    }
}
