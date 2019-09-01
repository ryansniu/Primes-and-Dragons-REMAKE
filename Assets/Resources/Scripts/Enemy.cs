using System.Collections;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/Enemy";
    public TextMeshProUGUI textNum;
    public HealthBar HPBar;

    private Vector3 sliderOffset = new Vector3(-0.0667f, -0.475f, 0);
    private Vector3 hpNumOffset = new Vector3(-0.084f, -0.48f, 0);
    private Vector3 numOffset = new Vector3(0.188f, -0.48f, 0);
    public int number;
    private int currHealth;
    private int maxHealth;
    private int damage;

    private SpriteRenderer spr;
    private Transform trans;

    public static Enemy Create(string name, Vector3 spawnPos, int num, int health, int dmg) {
        Enemy e = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<Enemy>();
        e.setPosition(spawnPos);
        e.setInitValues(num, health, dmg);
        return e;
    }
    public void setInitValues(int num, int health, int dmg) {
        number = num;
        textNum.text = number.ToString();
        maxHealth = health;
        currHealth = maxHealth;
        damage = dmg;
    }

    void Awake() {
        trans = transform;
        spr = GetComponent<SpriteRenderer>();
    }
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
            HPBar.displayHP((int)(currHealth + (resultHealth - currHealth) * (currTime/totalTime)), maxHealth);
            yield return null;
        }
        currHealth = resultHealth;

        HPBar.displayHP(currHealth, maxHealth);
        HPBar.setHPNumColor(Color.black);
    }

    public virtual IEnumerator Attack(Player p, Board b) {
        yield return StartCoroutine(p.addToHealth(-damage));
        //board.orbSpawnRates
        //remove or change orbs
    }

    public void setPosition(Vector3 newPos){
        trans.position = newPos;
        HPBar.HPBar.transform.position = Camera.main.WorldToScreenPoint(newPos + sliderOffset);
        HPBar.HPNum.transform.position = Camera.main.WorldToScreenPoint(newPos + hpNumOffset);
        textNum.transform.position = Camera.main.WorldToScreenPoint(newPos + numOffset);
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
