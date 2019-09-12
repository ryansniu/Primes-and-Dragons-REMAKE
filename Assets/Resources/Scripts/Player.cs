using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    private WaitUntil DELTA_ZERO;
    public HealthBar HPBar;
    public Image HPBarIMG;
    private Sprite[] playerHPBars = new Sprite[3];
    private float currHealth;
    private int maxHealth;
    public volatile float deltaHealth;
    private bool isUpdatingHealth = false;
    private float HPSpeed = 100f;
    private readonly Vector3 HPDelta_POS = new Vector3(0.7f, 0.1f, -4f);
    void Awake() {
       playerHPBars[0] = Resources.Load<Sprite>("Sprites/Player Board/health_bar_fg_25");
       playerHPBars[1] = Resources.Load<Sprite>("Sprites/Player Board/health_bar_fg_50");
       playerHPBars[2] = Resources.Load<Sprite>("Sprites/Player Board/health_bar_fg_100");
       DELTA_ZERO = new WaitUntil(() => deltaHealth == 0f);
    }
    void Update() {
        if(isUpdatingHealth){
            if(deltaHealth == 0f){
                HPBar.setHPNumColor(Color.black);
                isUpdatingHealth = false;
            }
            else{
                HPBar.setHPNumColor(deltaHealth > 0f ? Color.green : Color.red);
                float diff = deltaHealth > 0f ? Mathf.Min(Time.deltaTime * HPSpeed, deltaHealth) : Mathf.Max(Time.deltaTime * -HPSpeed, deltaHealth);
                currHealth += diff;
                deltaHealth -= diff;
            }
            updateHPBar((int)Mathf.Round(currHealth), maxHealth);
        }
    }
    public void addToHealth(int value) {
        deltaHealth += value;
        HPSpeed = Mathf.Max(100f, value > 0 ? deltaHealth : deltaHealth * -1);
        HPDeltaNum.Create(HPDelta_POS, value, 4f/3);
        isUpdatingHealth = true;
    }
    public IEnumerator resetDeltaHealth(){
        yield return DELTA_ZERO;
        deltaHealth = 0f;
        currHealth = (int)Mathf.Round(Mathf.Clamp(currHealth, 0, maxHealth));
    }
    public IEnumerator setMaxHealth(int value) {
        if(maxHealth == value) yield break;
        int oldMaxHealth = maxHealth;
        maxHealth = value;
        addToHealth(maxHealth - oldMaxHealth);
        yield return StartCoroutine(resetDeltaHealth());
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
