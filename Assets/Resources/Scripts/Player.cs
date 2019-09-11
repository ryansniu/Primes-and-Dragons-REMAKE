using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    private volatile int currHealth;
    private int maxHealth;
    public volatile HealthBar HPBar;
    public volatile Image HPBarIMG;
    private Sprite[] playerHPBars = new Sprite[3];
    void Awake() {
       playerHPBars[0] = Resources.Load<Sprite>("Sprites/Player Board/health_bar_fg_25");
       playerHPBars[1] = Resources.Load<Sprite>("Sprites/Player Board/health_bar_fg_50");
       playerHPBars[2] = Resources.Load<Sprite>("Sprites/Player Board/health_bar_fg_100");
    }
    void Start() {
        updateHPBar(currHealth, maxHealth);
    }

    public IEnumerator addToHealth(int value) {
        if(value >= 0) HPBar.setHPNumColor(Color.green);
        else if(value == 0) HPBar.setHPNumColor(Color.black);
        else HPBar.setHPNumColor(Color.red);

        int resultHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);  //SUS
        float totalTime = Mathf.Min((float)Mathf.Abs(resultHealth - currHealth)/HealthBar.ANIM_SPEED, HealthBar.MAX_ANIM_TIME);
        float currTime = 0f;
        while(currTime < totalTime){
            currTime += Time.deltaTime;
            int middleHealth = Mathf.Clamp((int)(currHealth + (resultHealth - currHealth) * (currTime/totalTime)), 0, maxHealth);
            updateHPBar(middleHealth, maxHealth);
            yield return null;
        }
        currHealth = resultHealth;

        updateHPBar(currHealth, maxHealth);
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

    public void updateHPBar(int currHealth, int maxHealth){
        HPBar.displayHP(currHealth, maxHealth);
        float ratio = (float)currHealth/maxHealth;
        int HPIndex = 0;
        if(ratio > 0.25f) HPIndex++;
        if(ratio > 0.50f) HPIndex++;
        HPBarIMG.sprite = playerHPBars[HPIndex];
    }
}
