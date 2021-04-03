using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public enum EnemyPosition {
    CENTER_1,
    LEFT_2, RIGHT_2,
    LEFT_3, CENTER_3, RIGHT_3
}
public enum EnemyBuffs { NONE, DMG_REFLECT, DMG_MITI_50, DMG_ABSORB }

[Serializable]
public class EnemyState {
    public string prefab, spriteName;
    public int number, currHealth, maxHealth, damage;
    public EnemyBuffs buff = EnemyBuffs.NONE;
    public OrbSpawnRate[] orbSpawnRates = Board.getDefaultOrbSpawnRates();

    public bool alwaysShowSkills = true;
    public List<int> activatedTurn, lastActivatedTurn;
    public int enemyID;

    // for normal enemy use only
    public List<int> easySkills, medSkills, hardSkills;
}

public class Enemy : MonoBehaviour {
    protected readonly static Vector3 spawnPos = new Vector3();
    protected readonly static Vector3 healthbarOffset = new Vector3(0f, 200f, 0f);
    protected readonly static string PREFAB_PATH = "Prefabs/Enemies/";
    protected readonly static string SPRITE_PATH = "Sprites/Enemies/";
    protected readonly static string HPBAR_PATH = "Sprites/Enemies/Enemy UI/";
    protected readonly static float Z_VALUE = -3f;
    protected readonly static float UI_SCALE = 5f;
    protected readonly static float SKILL_DELAY = 0.03f;
    protected readonly float FADE_ANIM_TIME = 0.25f;
    protected readonly static WaitForSeconds SKILL_WAIT = new WaitForSeconds(SKILL_DELAY);
    protected static System.Random RNG = new System.Random();

    protected EnemyState currState = new EnemyState();
    protected SpriteRenderer spr;
    protected Transform trans;
    protected RectTransform HPtrans, skillRectTrans;
    [SerializeField] protected TextMeshProUGUI textNum = default;
    [SerializeField] protected HealthBar HPBar = default;
    [SerializeField] protected Image HPBarIMG = default, toggleSkillImg = default;
    [SerializeField] protected Toggle skillToggle = default;
    protected Sprite[] enemyHPBars;
    protected bool isFlashingColor = false;

    [SerializeField] private CanvasGroup skillGroup = default;
    protected Transform skillTrans;
    protected List<EnemySkill> skillList = new List<EnemySkill>();
    private List<EnemySkill> activeSkills = new List<EnemySkill>();
    protected bool skillsAreShown = true;
    public static Enemy Create(string prefab, int num, int health, int dmg, string sprite) {
        Enemy e = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH + prefab), spawnPos, Quaternion.identity)).GetComponent<Enemy>();
        e.setInitValues(prefab, num, health, dmg, sprite);
        return e;
    }
    protected void setInitValues(string prefab, int num, int health, int dmg, string sprite) {
        currState.prefab = prefab;
        currState.enemyID = RNG.Next(99);
        currState.number = num;
        textNum.text = currState.number.ToString();
        currState.maxHealth = (int)(health * PlayerPrefs.GetFloat("EnemyHP", 1f));
        currState.currHealth = currState.maxHealth;
        currState.damage = dmg;
        currState.spriteName = sprite;
        spr.sprite = Resources.Load<Sprite>(SPRITE_PATH + currState.spriteName);
        loadAllHPBarIMGs();
        addAllSkills();
    }
    protected virtual void loadAllHPBarIMGs() => enemyHPBars = Resources.LoadAll<Sprite>(HPBAR_PATH + "Normal");
    protected virtual void addAllSkills() => skillList.Add(EnemyAttack.Create(() => true, false, () => Player.Instance.gameObject, () => -currState.damage, skillTrans));
    public int getEnemyID() => currState.enemyID;
    protected int getRandomSeedByFloor() => getEnemyID() + GameController.Instance.getFloor();
    protected int getRandomSeedByTurn(EnemySkill es) => skillList.IndexOf(es) + getEnemyID() + GameController.Instance.getCurrTurn();
    public string getSkillID(EnemySkill es, int activatedTurn) => getEnemyID() + ":" + skillList.IndexOf(es) + "," + activatedTurn;

    // vv SAVING AND LOADING vv
    public EnemyState getState() {
        currState.activatedTurn = new List<int>();
        currState.lastActivatedTurn = new List<int>();
        foreach (EnemySkill es in skillList) {
            currState.activatedTurn.Add(es.getTurnStart());
            currState.lastActivatedTurn.Add(es.getLastTurnStart());
        }
        return currState;
    }
    public void setState(EnemyState es) {
        currState = es;
        setBuff(currState.buff);
        skillList = new List<EnemySkill>();
        addAllSkills();
        loadAllSkills();
    }
    // ^^ SAVING AND LOADING ^^

    void Awake() {
        trans = transform;
        skillTrans = skillGroup.transform;
        HPtrans = trans.Find("Enemy UI").Find("Health Bar UI").GetComponent<RectTransform>();
        skillRectTrans = trans.Find("Enemy UI").Find("Skill Display").GetComponent<RectTransform>();
        trans.SetParent(GameObject.Find("Enemies").transform);

        spr = GetComponent<SpriteRenderer>();
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
    void Start() {
        HPBar.displayHP(currState.currHealth, currState.maxHealth);
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
        skillRectTrans.anchoredPosition = new Vector3(spriteX * 5, 335f, UIz);
        HPtrans.anchoredPosition = new Vector3(UIx, UIy, UIz);
    }
    public bool isAlive() => currState.currHealth > 0;
    protected IEnumerator addToHealth(int value) {
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
    public IEnumerator takeDMG(int dmg) {
        if (dmg > 0) yield return StartCoroutine(addToHealth(dmg));  // Buffs don't affect healing (for now)
        else if (dmg < 0) {
            if (currState.buff == EnemyBuffs.DMG_REFLECT) yield return StartCoroutine(Player.Instance.addToHealth(dmg / 10));
            else {
                if (currState.buff == EnemyBuffs.DMG_MITI_50) dmg /= 2;
                else if (currState.buff == EnemyBuffs.DMG_ABSORB) dmg /= -10;
                yield return StartCoroutine(addToHealth(dmg));
            }
        }
        isFlashingColor = false;
        spr.color = Color.white;
    }
    public IEnumerator Attack() {
        yield return StartCoroutine(showAllSkills(true));
        foreach (EnemySkill es in skillList) {
            if (!es.isActivated() && es.useSkillNow()) {
                yield return StartCoroutine(activateSkill(es, true));
                yield return StartCoroutine(es.fadeInAnim(true));
                yield return StartCoroutine(es.onActivate(this));
                yield return es.getAnimIsOver();
            }
            if (es.isActivated()) {
                if (es.oneTurnOnly()) {
                    if (es.hasEndAnim()) {
                        yield return StartCoroutine(es.updateSlider());
                        yield return StartCoroutine(useEndSkill(es));
                    }
                }
                else {
                    float progress = es.getTurnProgress();
                    yield return StartCoroutine(es.updateSlider());
                    if (progress == 1f && es.hasEndAnim()) {
                        yield return StartCoroutine(useEndSkill(es));
                    }
                }
            }
        }
        yield return StartCoroutine(updateAndRmvAllSkills(false));
    }
    public IEnumerator activateSkill(EnemySkill es, bool firstSkill) {
        es.gameObject.SetActive(true);
        activeSkills.Sort((es1, es2) => es1.compareSliders(es2));
        if (!firstSkill) activeSkills.Remove(es);
        activeSkills.Insert(0, es);
        for (int i = 0; i < activeSkills.Count; i++) {
            StartCoroutine(activeSkills[i].movePosTo(i));
            //yield return SKILL_WAIT;
        }
        foreach (EnemySkill aes in activeSkills) yield return aes.getAnimIsOver();
    }
    private IEnumerator useEndSkill(EnemySkill es) {
        yield return StartCoroutine(activateSkill(es, false));
        yield return StartCoroutine(es.fadeInAnim(false));
        yield return StartCoroutine(es.onEnd(this));
        yield return es.getAnimIsOver();
    }
    public IEnumerator updateAndRmvAllSkills(bool updateSkills) {
        yield return StartCoroutine(showAllSkills(true));
        if (updateSkills) {
            foreach (EnemySkill es in activeSkills) {
                StartCoroutine(es.updateSlider());
                //yield return SKILL_WAIT;
            }
            foreach (EnemySkill es in activeSkills) yield return es.getAnimIsOver();
        }
        List<EnemySkill> rmvSkillsNow = new List<EnemySkill>();
        List<EnemySkill> rmvSkillsLater = new List<EnemySkill>();
        foreach (EnemySkill es in activeSkills) {
            if (es.getNumTurnsLeft() <= 0) {
                if (es.hasEndAnim() && !es.oneTurnOnly()) rmvSkillsLater.Add(es);
                else rmvSkillsNow.Add(es);
            }
        }
        foreach(EnemySkill es in rmvSkillsNow) {
            activeSkills.Remove(es);
            StartCoroutine(es.destroyAnim());
            //yield return SKILL_WAIT;
        }
        foreach (EnemySkill es in activeSkills) StartCoroutine(es.movePosTo(activeSkills.IndexOf(es)));
        foreach (EnemySkill es in rmvSkillsNow) {
            yield return es.getAnimIsOver();
            es.onDestroy(this);
            es.gameObject.SetActive(false);
        }
        foreach (EnemySkill es in activeSkills) yield return es.getAnimIsOver();

        foreach (EnemySkill es in rmvSkillsLater) {
            yield return StartCoroutine(useEndSkill(es));
            activeSkills.Remove(es);
            StartCoroutine(es.destroyAnim());
            foreach (EnemySkill aes in activeSkills) StartCoroutine(aes.movePosTo(activeSkills.IndexOf(aes)));
            yield return es.getAnimIsOver();
            es.onDestroy(this);
            es.gameObject.SetActive(false);
            foreach (EnemySkill aes in activeSkills) yield return aes.getAnimIsOver();
        }
        if (!currState.alwaysShowSkills) yield return StartCoroutine(showAllSkills(false));
    }
    public void loadAllSkills() {
        for(int i = 0; i < skillList.Count; i++) {
            EnemySkill es = skillList[i];
            es.setLastStartTurn(currState.lastActivatedTurn[i]);
            int startTurn = currState.activatedTurn[i];
            if (startTurn != -1) {
                es.gameObject.SetActive(true);
                es.setStartTurn(startTurn);
                activeSkills.Add(es);
            }
        }
        activeSkills.Sort((es1, es2) => es1.compareSliders(es2));
        for (int i = 0; i < activeSkills.Count; i++) activeSkills[i].setPos(i);
        skillToggle.isOn = currState.alwaysShowSkills;
        toggleSkillDisplay(currState.alwaysShowSkills);
    }
    public void endAllSkills() {
        foreach(EnemySkill es in activeSkills) {
            es.onDestroy(this);
            es.gameObject.SetActive(false);
        }
    }
    public void toggleSkillDisplay(bool toShow) {
        currState.alwaysShowSkills = toShow;
        toggleSkillImg.sprite = currState.alwaysShowSkills ? enemyHPBars[6] : enemyHPBars[5];
        StartCoroutine(showAllSkills(toShow));
    }
    public void enableSkillToggle(bool turnOn) => skillToggle.interactable = turnOn;
    public IEnumerator showAllSkills(bool toShow) {
        if (toShow == skillsAreShown) yield break;
        skillsAreShown = toShow;
        if (activeSkills.Count == 0) yield break;
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            skillGroup.alpha = skillsAreShown ? currTime / FADE_ANIM_TIME : 1f - currTime / FADE_ANIM_TIME;
            yield return null;
        }
        skillGroup.alpha = skillsAreShown ? 1f : 0f;
    }
    public bool toggleAllTimerSkills(bool toActivate) {
        bool anyTimerActivated = false;
        foreach(EnemySkill es in activeSkills) {
            if(es is EnemyTimer) {
                EnemyTimer et = es as EnemyTimer;
                et.toggleSkill(toActivate);
                anyTimerActivated = toActivate;
            }
        }
        return anyTimerActivated;
    }
    public IEnumerator clearAllMarkedTimerOrbs() {
        foreach (EnemySkill es in activeSkills) {
            if (es is EnemyTimer) {
                EnemyTimer et = es as EnemyTimer;
                yield return StartCoroutine(et.clearAllMarkedOrbs());
            }
        }
    }

    public IEnumerator targetedAnimation(bool isHealed) {
        isFlashingColor = true;
        float FLASH_ANIM_DIFF = 0.7f;
        float FLASH_ANIM_TIME = 0.5f;
        for (float flashAnimTimer = 0f; isFlashingColor; flashAnimTimer += Time.deltaTime) {
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
    public bool setBuff(EnemyBuffs buff) {
        if (buff != currState.buff && buff != EnemyBuffs.NONE && currState.buff != EnemyBuffs.NONE) return false;
        currState.buff = buff;
        switch (currState.buff) {  // Change the health bar.
            case EnemyBuffs.DMG_MITI_50: HPBarIMG.sprite = enemyHPBars[1]; break;
            case EnemyBuffs.DMG_REFLECT: HPBarIMG.sprite = enemyHPBars[2]; break;
            case EnemyBuffs.DMG_ABSORB: HPBarIMG.sprite = enemyHPBars[3]; break;
            default: HPBarIMG.sprite = enemyHPBars[0]; break;
        }
        return true;
    }
}
