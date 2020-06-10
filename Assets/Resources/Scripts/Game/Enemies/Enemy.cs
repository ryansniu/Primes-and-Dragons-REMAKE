using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public enum EnemyPosition {
    CENTER_1,
    LEFT_2, RIGHT_2,
    LEFT_3, CENTER_3, RIGHT_3
}
public enum EnemyBuffs {
    NONE, DMG_REFLECT, DMG_MITI_50, DMG_ABSORB
}

[Serializable]
public class EnemyState {
    public string prefab, spriteName;
    public int number, currHealth, maxHealth, damage;
    public EnemyBuffs buff; 
}

public class Enemy : MonoBehaviour {
    protected readonly static Vector3 spawnPos = new Vector3();
    protected readonly static Vector3 healthbarOffset = new Vector3(0f, 200f, 0f);
    protected readonly static string PREFAB_PATH = "Prefabs/Enemies/";
    protected readonly static string SPRITE_PATH = "Sprites/Enemies/";
    protected readonly static string HPBAR_PATH = "Sprites/Enemies/Enemy UI/";
    protected readonly static float Z_VALUE = -3f;
    protected readonly static float UI_SCALE = 5f;
    protected static System.Random RNG = new System.Random();

    protected EnemyState currState = new EnemyState();
    protected SpriteRenderer spr;
    protected Transform trans;
    protected RectTransform HPtrans, skilltrans;
    [SerializeField] protected TextMeshProUGUI textNum = default;
    [SerializeField] protected HealthBar HPBar = default;
    [SerializeField] protected Image HPBarIMG = default;
    protected Sprite[] enemyHPBars;
    [SerializeField] protected EnemySkillUI eSkillUI = default;

    protected bool isFlashingColor = false;
    protected bool attackThisTurn = true;

    public static Enemy Create(string prefab, int num, int health, int dmg, string sprite){
        Enemy e = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH+prefab), spawnPos, Quaternion.identity)).GetComponent<Enemy>();
        e.setInitValues(prefab, num, health, dmg, sprite);
        return e;
    }
    public void setInitValues(string prefab, int num, int health, int dmg, string sprite) {
        currState.prefab = prefab;
        currState.number = num;
        textNum.text = currState.number.ToString();
        currState.maxHealth = health;
        currState.currHealth = currState.maxHealth;
        currState.damage = dmg;
        currState.spriteName = sprite;
        spr.sprite = Resources.Load<Sprite>(SPRITE_PATH + currState.spriteName);
        loadAllHPBarIMGs();
    }
    protected virtual void loadAllHPBarIMGs() => enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Normal");

    // vv SAVING AND LOADING vv
    public EnemyState getState() => currState;
    public void setState(EnemyState es) => currState = es;
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
    protected IEnumerator addToHealth(int value){
        if (value >= 0) HPBar.setHPNumColor(ColorPalette.getColor(6, 2));
        else if (value == 0) HPBar.setHPNumColor(Color.black);
        else HPBar.setHPNumColor(ColorPalette.getColor(1, 1));

        int resultHealth = Mathf.Clamp(currState.currHealth + value, 0, currState.maxHealth);
        float totalTime = Mathf.Min((float)Mathf.Abs(resultHealth - currState.currHealth) / HealthBar.ANIM_SPEED, HealthBar.MAX_ANIM_TIME);
        for (float currTime = 0f; currTime < totalTime; currTime += Time.deltaTime) {
            int middleHealth = Mathf.Clamp((int)(currState.currHealth + (resultHealth - currState.currHealth) * (currTime / totalTime)), 0, currState.maxHealth);
            HPBar.displayHP(middleHealth, currState.maxHealth);
            yield return null;
        }
        currState.currHealth = resultHealth;

        HPBar.displayHP(currState.currHealth, currState.maxHealth);
        HPBar.setHPNumColor(Color.black);
    }
    protected IEnumerator useSkill(EnemySkillName skill, float skillDuration) {
        yield return StartCoroutine(eSkillUI.displaySkill(skill.ToString(), skillDuration));
    }
    public IEnumerator takeDMG(int dmg) {
        if (dmg > 0) yield return StartCoroutine(addToHealth(dmg));  // Buffs don't affect healing (for now)
        else if (dmg < 0) {
            if (currState.buff == EnemyBuffs.DMG_REFLECT) yield return StartCoroutine(Player.Instance.addToHealth(dmg / 10));
            else {
                if (currState.buff == EnemyBuffs.DMG_MITI_50) dmg /= 2;
                else if (currState.buff == EnemyBuffs.DMG_ABSORB) dmg *= -1;
                yield return StartCoroutine(addToHealth(dmg));
            }
        }
        isFlashingColor = false;
        spr.color = Color.white;
    }
    public virtual IEnumerator Attack(){
        if (attackThisTurn) yield return StartCoroutine(Player.Instance.addToHealth(-currState.damage));
        else attackThisTurn = true;
    }
    public virtual void setPosition(EnemyPosition pos) {
        float spriteX = 0f, spriteY = 95f, spriteZ = -9f;
        float UIx = 0, UIy = 165f, UIz = -14f;
        switch (pos) {
            case EnemyPosition.CENTER_1:
                /* Do nothing */
                break;
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
    public IEnumerator targetedAnimation(bool isHealed){
        isFlashingColor = true;
        float FLASH_ANIM_DIFF = 0.7f;
        float FLASH_ANIM_TIME = 0.5f;
        for(float flashAnimTimer = 0f; isFlashingColor; flashAnimTimer += Time.deltaTime) {
            float currDarken = Mathf.SmoothStep(1f - FLASH_ANIM_DIFF, 1f, 1f - FLASH_ANIM_DIFF * Mathf.PingPong(flashAnimTimer / FLASH_ANIM_TIME, 1f));
            bool flashRed = false, flashGreen = false, flashBlue = false;
            if (isHealed || currState.buff == EnemyBuffs.DMG_MITI_50) flashGreen = true;
            else if (currState.buff == EnemyBuffs.DMG_REFLECT) flashBlue = true;
            else if (currState.buff == EnemyBuffs.DMG_ABSORB) { }  // TO-DO: implement damage absorb pls
            else if (currState.buff == EnemyBuffs.NONE) flashRed = true;
            spr.color = new Color(flashRed ? 1f : currDarken, flashGreen ? 1f : currDarken, flashBlue ? 1f : currDarken, 1f);
            yield return null;
        }
    }
    protected void setBuff(EnemyBuffs buff) {
        currState.buff = buff;
        switch (currState.buff) {  // Change the health bar.
            case EnemyBuffs.DMG_MITI_50: HPBarIMG.sprite = enemyHPBars[1]; break;
            case EnemyBuffs.DMG_REFLECT: HPBarIMG.sprite = enemyHPBars[2]; break;
            case EnemyBuffs.DMG_ABSORB: HPBarIMG.sprite = enemyHPBars[3]; break;   // TO-DO: implement damage absorb pls
            default: HPBarIMG.sprite = enemyHPBars[0]; break;
        }
    }

    public bool isAlive() => currState.currHealth > 0;
}
public enum EnemySkillName {
    shuffle,
    clear,
    replace,
    decrement,
    reflect,
    mitigate,
    mark,
    heal,
    timer,
    attack
}
public class EnemySkill {
    private EnemySkillName skillName;
}