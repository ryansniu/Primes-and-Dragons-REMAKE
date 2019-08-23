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
        Debug.Log("Damage: "+value);
        currHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);
        Debug.Log("HP: "+currHealth+"/"+maxHealth);
        //take damage && heal animation
    }
    public void setMaxHealth(int value) {
        int oldMaxHealth = maxHealth;
        maxHealth = value;
        //reduce && increase max health animation
        addToHealth(maxHealth - oldMaxHealth);
    }
    public bool isAlive() {
        return currHealth > 0;
    }

    //health bar animations
}
