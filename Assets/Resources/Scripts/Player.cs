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

    public IEnumerator addToHealth(int value) {
        if(value >= 0) HPBar.setHPNumColor(Color.green);
        else if(value == 0) HPBar.setHPNumColor(Color.black);
        else HPBar.setHPNumColor(Color.red);

        int resultHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);
        float totalTime = Mathf.Min((float)Mathf.Abs(resultHealth - currHealth)/HealthBar.ANIM_SPEED, HealthBar.MAX_ANIM_TIME);
        float currTime = 0f;
        while(currTime < totalTime){
            currTime += Time.deltaTime;
            int middleHealth = Mathf.Clamp((int)(currHealth + (resultHealth - currHealth) * (currTime/totalTime)), 0, maxHealth);
            HPBar.displayHP(middleHealth, maxHealth);
            yield return null;
        }
        currHealth = resultHealth;

        HPBar.displayHP(currHealth, maxHealth);
        HPBar.setHPNumColor(Color.black);
    }
    public IEnumerator setMaxHealth(int value) {
        int oldMaxHealth = maxHealth;
        maxHealth = value;
        yield return StartCoroutine(addToHealth(maxHealth - oldMaxHealth));
    }
    public bool isAlive() {
        return currHealth > 0;
    }
}
