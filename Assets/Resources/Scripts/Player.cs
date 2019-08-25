using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private int currHealth;
    private int maxHealth;
    //health bar obj

    void Awake() {

    }

    void Update() {

    }

    public void addToHealth(int value) {
        if(value >= 0){
            //flash green
        }
        else if(value == 0) {
            //flash black wtf
        }
        else{
            //flash red
        }
        Debug.Log("Damage: " + value);
        currHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);   //adjust health bar bit by bit
        Debug.Log("HP: " + currHealth + "/" + maxHealth);
        //return health num to black
    }
    public void setMaxHealth(int value) {
        int oldMaxHealth = maxHealth;
        maxHealth = value;
        addToHealth(maxHealth - oldMaxHealth);
    }
    public bool isAlive() {
        return currHealth > 0;
    }
}
