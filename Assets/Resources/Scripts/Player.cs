using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private int currHealth;
    private int maxHealth;
    public HealthBar HPBar;
    void Start() {
        HPBar.displayHP(currHealth, maxHealth);
    }

    public void addToHealth(int value) {
        if(value >= 0) HPBar.setHPNumColor(Color.green);
        else if(value == 0) HPBar.setHPNumColor(Color.black);
        else HPBar.setHPNumColor(Color.red);
        Debug.Log("Damage: " + value);
        currHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);   //adjust health bar bit by bit
        HPBar.displayHP(currHealth, maxHealth);
        HPBar.setHPNumColor(Color.black);
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
