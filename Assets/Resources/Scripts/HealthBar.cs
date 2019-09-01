using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public TextMeshProUGUI HPNum;
    public Slider HPBar;

    public static readonly float MAX_ANIM_TIME = 1f;
    public static readonly int ANIM_SPEED = 100;
    public void setHPNumColor(Color c){
        HPNum.color = c;
    }
    public void displayHP(int currHealth, int maxHealth){
        HPNum.text = currHealth+"/"+maxHealth; 
        HPBar.value = (float)currHealth/maxHealth;
    }
}
