using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public int number;
    private int currHealth;
    private int maxHealth;
    //health bar obj

    void Start() {
        
    }

    void Update() {
        
    }
    public void addToHealth(int value) {
        currHealth += value;
        //take damage && heal animation
    }

    public virtual IEnumerator Attack(Player p, Board b) {
        //either deal damage or use skills
        yield return null;
    }

    public bool isAlive() {
        return currHealth > 0;
    }

    //spawn animation
    //attack animation
    //hurt animation
    //special animation
    //heal animation
}
