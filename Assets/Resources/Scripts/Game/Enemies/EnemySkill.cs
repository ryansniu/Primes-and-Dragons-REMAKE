using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EnemySkillType {
    HEAL, ATTACK,
    REFLECT, MITIGATE, ABSORB,
    MARK, CLEAR, REPLACE, DECREMENT, SHUFFLE,
    TIMER, ORB_SPAWN
}

public class EnemySkill : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/UI/EnemySkillItem";
    protected static float FADE_ANIM_TIME = 0.15f;
    protected static float MIN_SLIDE_TIME = 0.25f;
    protected static float MOVE_ANIM_TIME = 0.15f;
    protected static float DESTROY_ANIM_TIME = 0.3f;
    private static readonly Vector3 SPAWN_POS = new Vector3(0f, -110f, 0f);
    private const float SLIDER_HEIGHT = 70f;

    private CanvasGroup cGroup = default;
    private Slider BG = default;
    private TextMeshProUGUI skillText = default;
    private RectTransform rectTrans;
    private int currPos = 0;

    protected EnemySkillType startSkill, endSkill;
    protected int activatedTurn = -1, turnDur = 0;
    protected float startAnim = 0f, endAnim = -1f;
    private Func<bool> whenToUse;
    protected bool isAnimating;
    private WaitUntil animIsOver;

    protected static GameObject Create(Transform parent) {
        GameObject skillObj = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH), SPAWN_POS, Quaternion.identity));
        skillObj.transform.SetParent(parent, false);
        return skillObj;
    }
    public void initValues(EnemySkillType startST, Func<bool> wtu, int td, float sa, EnemySkillType endST = default, float ea = -1f) {
        startSkill = startST;
        whenToUse = wtu;
        turnDur = td;
        startAnim = sa;
        endSkill = endST;
        endAnim = ea;
        cGroup = GetComponent<CanvasGroup>();
        BG = GetComponentInChildren<Slider>();
        skillText = GetComponentInChildren<TextMeshProUGUI>();
    }
    void Awake() {
        rectTrans = GetComponent<RectTransform>();
        animIsOver = new WaitUntil(() => !isAnimating);
    }

    public float getTurnProgress() => (GameController.Instance.getState().turnCount - activatedTurn) / (float)turnDur;
    public float getNumTurnsLeft() => (activatedTurn + turnDur) - GameController.Instance.getState().turnCount;
    public int compareSliders(EnemySkill other) => getNumTurnsLeft() == other.getNumTurnsLeft() ? other.getTurnProgress().CompareTo(getTurnProgress()) : getNumTurnsLeft().CompareTo(other.getNumTurnsLeft()); 
    public void toggleActivate(bool isActivated) => activatedTurn = isActivated ? GameController.Instance.getState().turnCount : -1;
    public bool isActivated() => activatedTurn != -1;
    public bool useSkillNow() => whenToUse();
    public WaitUntil getAnimIsOver() => animIsOver;
    public bool oneTurnOnly() => turnDur == 0;
    public bool hasEndAnim() => endAnim != -1f;

    public virtual IEnumerator onActivate(Enemy e) { yield return null; }
    public virtual IEnumerator onEnd(Enemy e) { yield return null; }
    public virtual IEnumerator onDestroy(Enemy e) { yield return null; }
    public virtual string getSkillText(bool useFirstSkill) => useFirstSkill ? startSkill.ToString().ToLower() : endSkill.ToString().ToLower();
    
    public IEnumerator fadeInAnim(bool useFirstSkill) {
        isAnimating = true;
        skillText.text = getSkillText(useFirstSkill);
        rectTrans.anchoredPosition = SPAWN_POS;
        currPos = 0;
        BG.value = 0;

        // Fade in.
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            cGroup.alpha = currTime / FADE_ANIM_TIME;
            yield return null;
        }
        cGroup.alpha = 1f;

        StartCoroutine(slideAnim(useFirstSkill ? startAnim : endAnim));
    }
    public IEnumerator slideAnim(float animTime) {
        float dur = Mathf.Max(animTime, MIN_SLIDE_TIME);
        // Slider.
        for (float currTime = 0f; currTime < dur; currTime += Time.deltaTime) {
            BG.value = currTime / dur;  // smooth step?
            yield return null;
        }
        BG.value = 1;
        isAnimating = false;
    }
    public IEnumerator updateSlider() {
        float oldVal = BG.value, newVal = 1f - getTurnProgress();
        if (oldVal == newVal) yield break;
        isAnimating = true;
        for (float currTime = 0f; currTime < MIN_SLIDE_TIME; currTime += Time.deltaTime) {
            BG.value = Mathf.SmoothStep(oldVal, newVal, currTime / MIN_SLIDE_TIME);
            yield return null;
        }
        BG.value = newVal;
        isAnimating = false;
    }
    public IEnumerator movePosTo(int newPos) {
        if (newPos == currPos) yield break;
        isAnimating = true;
        Vector3 oldVectPos = rectTrans.anchoredPosition, newVectPos = new Vector3(oldVectPos.x, oldVectPos.y + SLIDER_HEIGHT * (newPos - currPos), oldVectPos.z);
        for (float currTime = 0f; currTime < MOVE_ANIM_TIME; currTime += Time.deltaTime) {
            rectTrans.anchoredPosition = Vector3.Lerp(oldVectPos, newVectPos, currTime / MOVE_ANIM_TIME);
            yield return null;
        }
        rectTrans.anchoredPosition = newVectPos;
        currPos = newPos;
        isAnimating = false;
    }
    public IEnumerator destroyAnim() {
        isAnimating = true;
        Vector3 oldVectPos = rectTrans.anchoredPosition, newVectPos = new Vector3(oldVectPos.x + 500, oldVectPos.y, oldVectPos.z);
        for (float currTime = 0f; currTime < DESTROY_ANIM_TIME; currTime += Time.deltaTime) {
            cGroup.alpha = 1f - currTime / DESTROY_ANIM_TIME;  // SUS
            rectTrans.anchoredPosition = Vector3.Lerp(oldVectPos, newVectPos, currTime / DESTROY_ANIM_TIME);
            yield return null;
        }
        cGroup.alpha = 0f;
        rectTrans.anchoredPosition = newVectPos;
        isAnimating = false;
    }
}

public class EnemyAttack : EnemySkill {
    private Func<GameObject> getTarget;
    private Func<int> getDmg;
    public static EnemyAttack Create(Func<bool> wtu, bool isHeal, Func<GameObject> getTarget, Func<int> getDmg, Transform parent) {
        EnemyAttack atk = Create(parent).AddComponent<EnemyAttack>();
        atk.initValues(isHeal ? EnemySkillType.HEAL : EnemySkillType.ATTACK, wtu, 0, 0f);
        atk.getDmg = getDmg;
        atk.getTarget = getTarget;
        return atk;
    }

    public override IEnumerator onActivate(Enemy e) {
        GameObject target = getTarget();
        if (target.GetComponent<Enemy>() != null) yield return StartCoroutine(target.GetComponent<Enemy>().takeDMG(getDmg()));
        else if (target.GetComponent<Player>() != null) yield return StartCoroutine(Player.Instance.addToHealth(getDmg()));
    }
}

public class EnemyHPBuff : EnemySkill {
    private EnemyBuffs buff;
    public static EnemyHPBuff Create(Func<bool> wtu, EnemyBuffs buff, int turnDur, Transform parent) {
        EnemyHPBuff HPbuff = Create(parent).AddComponent<EnemyHPBuff>();
        HPbuff.initValues(convertBuffToSkill(buff), wtu, turnDur, 0f);
        HPbuff.buff = buff;
        HPbuff.gameObject.SetActive(false);
        return HPbuff;
    }
    private static EnemySkillType convertBuffToSkill(EnemyBuffs buff) {
        switch (buff) {
            case EnemyBuffs.DMG_REFLECT: return EnemySkillType.REFLECT;
            case EnemyBuffs.DMG_MITI_50: return EnemySkillType.MITIGATE;
            case EnemyBuffs.DMG_ABSORB: return EnemySkillType.ABSORB;
            default: throw new Exception("Invalid Enemy HP Buff!");
        }
    }
    public override IEnumerator onActivate(Enemy e) { e.setBuff(buff); yield return null; }
    public override IEnumerator onDestroy(Enemy e) { e.setBuff(EnemyBuffs.NONE); yield return null; }
}

public class EnemyBoardSkill : EnemySkill {
    public static EnemyBoardSkill Create(Func<bool> wtu, Transform parent) {
        EnemyBoardSkill boardSkill = Create(parent).AddComponent<EnemyBoardSkill>();
        boardSkill.initValues(EnemySkillType.MARK, wtu, 1, 0f);
        return boardSkill;
    }
    public override IEnumerator onActivate(Enemy e) {
        switch (startSkill) {
            case EnemySkillType.MARK:
                break;
            case EnemySkillType.SHUFFLE:
                break;
        }
        yield return null; 
    }
    public override IEnumerator onEnd(Enemy e) {
        switch (endSkill) {
            case EnemySkillType.CLEAR:
                break;
            case EnemySkillType.REPLACE:
                break;
            case EnemySkillType.DECREMENT:
                break;
        }
        yield return null;
    }
}

public class EnemyTimer : EnemySkill {
    private Func<int> getDOT;
    private int currDOT = 0;
    public static EnemyTimer Create(Func<bool> wtu, Func<int> getDOT, Transform parent) {
        EnemyTimer eTimer = Create(parent).AddComponent<EnemyTimer>();
        eTimer.initValues(EnemySkillType.TIMER, wtu, 1, 0f);
        eTimer.getDOT = getDOT;
        eTimer.gameObject.SetActive(false);
        return eTimer;
    }
    public override IEnumerator onActivate(Enemy e) {
        currDOT = getDOT();
        Player.Instance.setDOT(Player.Instance.getDOT() + currDOT); 
        yield return null;
    }
}

public class EnemyOrbSkill : EnemySkill {
    private OrbSpawnRate[] orbSpawnRates;
    public static EnemyOrbSkill Create(Func<bool> wtu, OrbSpawnRate[] orbSpawnRates, int turnDur, Transform parent) {
        EnemyOrbSkill orbSkill = Create(parent).AddComponent<EnemyOrbSkill>();
        orbSkill.initValues(EnemySkillType.ORB_SPAWN, wtu, turnDur, 0f);
        orbSkill.orbSpawnRates = orbSpawnRates;
        orbSkill.gameObject.SetActive(false);
        return orbSkill;
    }
    public override IEnumerator onActivate(Enemy e) { e.getState().orbSpawnRates = orbSpawnRates; yield return null; }
    public override IEnumerator onDestroy(Enemy e) { e.getState().orbSpawnRates = Board.getDefaultOrbSpawnRates(); yield return null; }
}