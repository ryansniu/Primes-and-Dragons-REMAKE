using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

[Serializable]
public class EnemyState {
    public string prefab;
    public int number;
    public int currHealth;
    public int maxHealth;
    public int damage;
    public int turnCount = 0;
}
public class Enemy : MonoBehaviour {
    protected const string PREFAB_PATH = "Prefabs/Enemies/";
    protected const float Z_VALUE = -3f;
    protected const float UI_SCALE = 5f;
    protected readonly static Vector3 spawnPos = new Vector3(0f, 0f, 0f);
    public TextMeshProUGUI textNum;
    public HealthBar HPBar;

    protected Transform sliderTrans;
    protected Transform numberTrans;
    protected Transform backGTrans;
    protected Vector3? silderOGPos;
    protected Vector3? numberOGPos;
    protected Vector3? backGOGPos;

    public EnemyState currState = new EnemyState();

    protected SpriteRenderer spr;
    protected Transform trans;

    private bool isFlashingRed = false;
    private float redAnimTimer = 0f;
    private float RED_ANIM_DIFF = 0.7f;
    private float RED_ANIM_TIME = 0.5f;
    public static Enemy Create(string prefab, int num, int health, int dmg){
        Enemy e = (Instantiate((GameObject)Resources.Load(PREFAB_PATH+prefab), spawnPos, Quaternion.identity) as GameObject).GetComponent<Enemy>();
        //e.setPosition(spawnPos);
        e.setInitValues(prefab, num, health, dmg);
        return e;
    }
    public void setInitValues(string prefab, int num, int health, int dmg){
        currState.prefab = prefab;
        currState.number = num;
        textNum.text = currState.number.ToString();
        currState.maxHealth = health;
        currState.currHealth = currState.maxHealth;
        currState.damage = dmg;
    }

    // vv SAVING AND LOADING vv
    public EnemyState getState() { return currState; }
    public void setState(EnemyState es) { currState = es; }
    // ^^ SAVING AND LOADING ^^

    void Awake() {
        trans = transform;
        sliderTrans = HPBar.HPBar.transform;
        numberTrans = textNum.transform;
        backGTrans = trans.Find("Enemy UI").Find("Background");
        trans.SetParent(GameObject.Find("Enemies").transform);

        spr = GetComponent<SpriteRenderer>();
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
    void Start() {
        HPBar.displayHP(currState.currHealth, currState.maxHealth);
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

        int resultHealth = Mathf.Clamp(currState.currHealth + value, 0, currState.maxHealth);
        float totalTime = Mathf.Min((float)Mathf.Abs(resultHealth - currState.currHealth) / HealthBar.ANIM_SPEED, HealthBar.MAX_ANIM_TIME);
        float currTime = 0f;
        while (currTime < totalTime){
            currTime += Time.deltaTime;
            int middleHealth = Mathf.Clamp((int)(currState.currHealth + (resultHealth - currState.currHealth) * (currTime / totalTime)), 0, currState.maxHealth);
            HPBar.displayHP(middleHealth, currState.maxHealth);
            yield return null;
        }
        currState.currHealth = resultHealth;

        HPBar.displayHP(currState.currHealth, currState.maxHealth);
        HPBar.setHPNumColor(Color.black);
    }
    public virtual IEnumerator takeDMG(int dmg, Player p, Board b) {
        yield return StartCoroutine(addToHealth(dmg));
    }
    public virtual IEnumerator Attack(Player p, Board b){
        p.addToHealth(-currState.damage);
        yield return null;
        currState.turnCount++;
    }

    public void setPosition(Vector3 newPos) {  // Plz don't touch this.
        newPos.Set(newPos.x, newPos.y, 0);
        if (silderOGPos == null) silderOGPos = sliderTrans.position;
        if (numberOGPos == null) numberOGPos = numberTrans.position;
        if (backGOGPos == null) backGOGPos = backGTrans.position;
        sliderTrans.position = (Vector3)silderOGPos + newPos;
        numberTrans.position = (Vector3)numberOGPos + newPos;
        backGTrans.position = (Vector3)backGOGPos + newPos;
        newPos.Set(newPos.x, newPos.y, Z_VALUE);
        trans.position = newPos;
    }
    public void toggleFlashingRed(bool isFlashing){
        if (!isFlashing){
            redAnimTimer = 0f;
            spr.color = Color.white;
        }
        isFlashingRed = isFlashing;
    }
    public bool isAlive(){
        return currState.currHealth > 0;
    }
}