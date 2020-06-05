using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public enum EnemyPosition {
    CENTER_1,
    LEFT_2,
    RIGHT_2,
    LEFT_3,
    CENTER_3,
    RIGHT_3
}

public enum EnemyStatus {
    NONE,
    ATTACKED,
    HEAL,
    DMG_REFLECT,
    DMG_MITI_50
}

[Serializable]
public class EnemyState {
    public string prefab;
    public int number;
    public int currHealth;
    public int maxHealth;
    public int damage;
    public int turnCount = 0;
    public string sprite;
    public EnemyStatus status; 
}
public class Enemy : MonoBehaviour {
    protected static System.Random RNG = new System.Random(DateTime.Now.Millisecond);
    protected const string PREFAB_PATH = "Prefabs/Enemies/";
    protected const string SPRITE_PATH = "Sprites/Enemies/";
    protected const float Z_VALUE = -3f;
    protected const float UI_SCALE = 5f;
    protected readonly static Vector3 spawnPos = new Vector3();
    protected readonly static Vector3 healthbarOffset = new Vector3(0f, 200f, 0f);
    public TextMeshProUGUI textNum;
    public HealthBar HPBar;
    public EnemySkill eSkill;

    public EnemyState currState = new EnemyState();

    protected SpriteRenderer spr;
    protected Transform trans;
    protected RectTransform HPtrans;
    protected RectTransform skilltrans;

    private bool isFlashingColor = false;
    private int flashingColorType = 0;
    private float flashAnimTimer = 0f;
    private float FLASH_ANIM_DIFF = 0.7f;
    private float FLASH_ANIM_TIME = 0.5f;

    protected bool attackThisTurn = true;
    public static Enemy Create(string prefab, int num, int health, int dmg){
        Enemy e = (Instantiate((GameObject)Resources.Load(PREFAB_PATH+prefab), spawnPos, Quaternion.identity)).GetComponent<Enemy>();
        e.setInitValues(prefab, num, health, dmg);
        return e;
    }
    public void setInitValues(string prefab, int num, int health, int dmg) {
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
        HPtrans = trans.Find("Enemy UI").Find("Health Bar UI").GetComponent<RectTransform>();
        skilltrans = trans.Find("Enemy UI").Find("Skill Display").GetComponent<RectTransform>();
        trans.SetParent(GameObject.Find("Enemies").transform);

        spr = GetComponent<SpriteRenderer>();
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
    void Start() {
        HPBar.displayHP(currState.currHealth, currState.maxHealth);
    }
    void Update() {
        if (isFlashingColor) {
            float currDarken = Mathf.SmoothStep(1f - FLASH_ANIM_DIFF, 1f, 1f - FLASH_ANIM_DIFF * Mathf.PingPong(flashAnimTimer / FLASH_ANIM_TIME, 1f));
            spr.color = new Color(flashingColorType == 1 ? 1f : currDarken, flashingColorType == 2 ? 1f : currDarken, flashingColorType == 3 ? 1f : currDarken, 1f);
            flashAnimTimer += Time.deltaTime;
        }
    }
    protected IEnumerator addToHealth(int value){
        if (value >= 0) HPBar.setHPNumColor(ColorPalette.getColor(6, 2));
        else if (value == 0) HPBar.setHPNumColor(Color.black);
        else {
            HPBar.setHPNumColor(ColorPalette.getColor(1, 1));
            if (currState.status == EnemyStatus.DMG_MITI_50) value /= 2;  // TO-DO: apply buffs somewhere else
        }

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
    public IEnumerator useSkill(string skillName, float skillDuration) {
        yield return StartCoroutine(eSkill.displaySkill(skillName, skillDuration));
    }
    public IEnumerator takeDMG(int dmg, Player p, Board b) {
        if (currState.status == EnemyStatus.DMG_REFLECT) yield return StartCoroutine(p.addToHealth(dmg / 10));
        else yield return StartCoroutine(addToHealth(currState.status == EnemyStatus.DMG_MITI_50 ? dmg / 2 : dmg));
    }
    public virtual IEnumerator Attack(Player p, Board b){
        if (attackThisTurn) {
            yield return StartCoroutine(p.addToHealth(-currState.damage));
            currState.turnCount++;
        }
        else attackThisTurn = true;
    }
    public void setSprite(string sprite) {
        currState.sprite = sprite;
        spr.sprite = Resources.Load<Sprite>(SPRITE_PATH + currState.sprite);
    }
    public virtual void setPosition(EnemyPosition pos) {
        float spriteX = 0f, spriteY = 95f, spriteZ = -9f;
        float UIx = 0, UIy = 165f, UIz = -14f;
        switch (pos) {
            case EnemyPosition.CENTER_1: /* Do nothing */ break;
            case EnemyPosition.LEFT_2:
                spriteX = -48f;
                UIx = -240f;
                break;
            case EnemyPosition.RIGHT_2:
                spriteX = 48f;
                UIx = 240f;
                break;
            case EnemyPosition.LEFT_3:
                spriteX = -72f;
                UIx = -300f;
                break;
            case EnemyPosition.CENTER_3:
                UIy = 785f;
                break;
            case EnemyPosition.RIGHT_3:
                spriteX = 72f;
                UIx = 300;
                break;
            default: break;
        }
        trans.position = new Vector3(spriteX, spriteY, spriteZ);
        skilltrans.anchoredPosition = new Vector3(spriteX * 5, 335f, UIz);
        HPtrans.anchoredPosition = new Vector3(UIx, UIy, UIz);
    }
    private void toggleFlashingColor(bool isFlashing, int colorType = 0){  // red = 1, green = 2, blue = 3; once a color starts flashing
        isFlashingColor = isFlashing;
        if (isFlashingColor && colorType != 0) flashingColorType = colorType;
        else {
            spr.color = Color.white;
            flashingColorType = 0;
        }
    }
    public void toggleStatus(EnemyStatus status, bool activated) {  // a status must always be manually deactivated before a new one can be activated (besides heal), buffs cant stack TO-DO: make buffs stack later
        if (!activated && currState.status == status) {
            currState.status = EnemyStatus.NONE;
            toggleFlashingColor(false);
        }
        else if (activated && currState.status == EnemyStatus.NONE) {
            currState.status = status;
            switch (currState.status) {
                case EnemyStatus.ATTACKED: toggleFlashingColor(true, 1); break;
                case EnemyStatus.HEAL: case EnemyStatus.DMG_MITI_50: toggleFlashingColor(true, 2); break;
                case EnemyStatus.DMG_REFLECT: toggleFlashingColor(true, 3); break;
            }
        }
    }

    // TO-DO: overhaul buffs system

    public bool isAlive(){
        return currState.currHealth > 0;
    }
}