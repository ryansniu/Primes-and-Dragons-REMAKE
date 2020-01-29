using System.Collections;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour {
    protected const string PREFAB_PATH = "Prefabs/Enemies/";
    protected readonly static Vector3 spawnPos = new Vector3(0, 1, -1);
    public TextMeshProUGUI textNum;
    public HealthBar HPBar;

    private Vector3 sliderOffset = new Vector3(-0.0667f, -0.475f, 0);
    private Vector3 hpNumOffset = new Vector3(-0.084f, -0.48f, 0);
    private Vector3 numOffset = new Vector3(0.188f, -0.48f, 0);
    public int number;
    protected int currHealth;
    protected int maxHealth;
    protected int damage;
    protected int turnCount = 0;

    protected SpriteRenderer spr;
    protected Transform trans;

    private bool isFlashingRed = false;
    private float redAnimTimer = 0f;
    private float RED_ANIM_DIFF = 0.7f;
    private float RED_ANIM_TIME = 0.5f;
    public static Enemy Create(string prefab, int num, int health, int dmg){
        Enemy e = (Instantiate((GameObject)Resources.Load(PREFAB_PATH+prefab), spawnPos, Quaternion.identity) as GameObject).GetComponent<Enemy>();
        e.setPosition(spawnPos);
        e.setInitValues(num, health, dmg);
        return e;
    }
    public void setInitValues(int num, int health, int dmg){
        number = num;
        textNum.text = number.ToString();
        maxHealth = health;
        currHealth = maxHealth;
        damage = dmg;
    }

    void Awake(){
        trans = transform;
        spr = GetComponent<SpriteRenderer>();
    }
    void Start(){
        HPBar.displayHP(currHealth, maxHealth);
    }
    void Update(){
        if (isFlashingRed){
            float currRedness = Mathf.SmoothStep(1f - RED_ANIM_DIFF, 1f, 1f - RED_ANIM_DIFF * Mathf.PingPong(redAnimTimer / RED_ANIM_TIME, 1f));
            spr.color = new Color(1f, currRedness, currRedness, 1f);
            redAnimTimer += Time.deltaTime;
        }
    }
    private IEnumerator addToHealth(int value){
        if (value >= 0) HPBar.setHPNumColor(Color.green);
        else if (value == 0) HPBar.setHPNumColor(Color.black);
        else HPBar.setHPNumColor(Color.red);

        int resultHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);
        float totalTime = Mathf.Min((float)Mathf.Abs(resultHealth - currHealth) / HealthBar.ANIM_SPEED, HealthBar.MAX_ANIM_TIME);
        float currTime = 0f;
        while (currTime < totalTime){
            currTime += Time.deltaTime;
            int middleHealth = Mathf.Clamp((int)(currHealth + (resultHealth - currHealth) * (currTime / totalTime)), 0, maxHealth);
            HPBar.displayHP(middleHealth, maxHealth);
            yield return null;
        }
        currHealth = resultHealth;

        HPBar.displayHP(currHealth, maxHealth);
        HPBar.setHPNumColor(Color.black);
    }
    public virtual IEnumerator takeDMG(int dmg, Player p, Board b) {
        yield return StartCoroutine(addToHealth(dmg));
    }
    public virtual IEnumerator Attack(Player p, Board b){
        p.addToHealth(-damage);
        yield return null;
        turnCount++;
    }

    public void setPosition(Vector3 newPos){
        trans.position = newPos;
        HPBar.HPBar.transform.position = Camera.main.WorldToScreenPoint(newPos + sliderOffset);
        HPBar.HPNum.transform.position = Camera.main.WorldToScreenPoint(newPos + hpNumOffset);
        textNum.transform.position = Camera.main.WorldToScreenPoint(newPos + numOffset);
    }
    public void toggleFlashingRed(bool isFlashing){
        if (!isFlashing){
            redAnimTimer = 0f;
            spr.color = Color.white;
        }
        isFlashingRed = isFlashing;
    }
    public bool isAlive(){
        return currHealth > 0;
    }
}