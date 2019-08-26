using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public TextMeshPro HPNum;
    public SpriteRenderer HPBar;

    public void setHPNumColor(Color c){
        HPNum.color = c;
    }
    public void displayHP(int currHealth, int maxHealth){
        HPNum.text = currHealth+"/"+maxHealth; 
        //HPBar.scale = (float)currHealth.maxHealth * scale
        //HPBar.position.x = something
    }
}
