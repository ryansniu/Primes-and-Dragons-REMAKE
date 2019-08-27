using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public TextMeshPro HPNum;
    public SpriteRenderer HPBar;

    public static readonly float MAX_ANIM_TIME = 2f;
    public static readonly int ANIM_SPEED = 200;
    public void setHPNumColor(Color c){
        HPNum.color = c;
    }
    public void displayHP(int currHealth, int maxHealth){
        HPNum.text = currHealth+"/"+maxHealth; 
        //HPBar.scale = (float)currHealth.maxHealth * scale
        //HPBar.position.x = something
    }
}
