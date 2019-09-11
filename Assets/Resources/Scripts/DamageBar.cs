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
    private bool isOnScreen = false;
    public void displayText(bool defaultText){
        SumText.text = defaultText ? "sum" : currSum.ToString();
        LenText.text = defaultText ? "len" : currLen.ToString();
        DmgText.text = defaultText ? "dmg" : currDmg.ToString();
    }
    public IEnumerator toggleBarPosition(bool onScreen){  
        if(isOnScreen != onScreen){
            displayText(!onScreen);
            if(onScreen){
                //TO-DO: animation
                yield return null;
            }
            else{
                //TO-DO: animation
                yield return null;
            }
        }
    }
    public void addNextDigit(int digit){
        currLen++;
        currSum += digit;
        currDmg = currSum*currLen;
        displayText(false);
    }

    /*    
    private void nothing(){
        int resultHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);
        float totalTime = Mathf.Min((float)Mathf.Abs(resultHealth - currHealth)/HealthBar.ANIM_SPEED, HealthBar.MAX_ANIM_TIME);
        float currTime = 0f;
        while(currTime < totalTime){
            currTime += Time.deltaTime;
            int middleHealth = Mathf.Clamp((int)(currHealth + (resultHealth - currHealth) * (currTime/totalTime)), 0, maxHealth);
            updateHPBar(middleHealth, maxHealth);
            yield return null;
        }
        currHealth = resultHealth;
    }
    */ 
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
