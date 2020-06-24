using System;
using System.Collections;
using System.Collections.Generic;
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
        skillObj.SetActive(false);
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
    public int getTurnStart() => activatedTurn;
    public void setStartTurn(int turnStart) {
        activatedTurn = turnStart;
        BG.value = 1f - (GameController.Instance.getState().turnCount - activatedTurn - 1) / (float)turnDur;
        cGroup.alpha = 1f;
        skillText.text = getSkillText(true);
    }
    public void setPos(int pos) {
        currPos = pos;
        rectTrans.anchoredPosition = new Vector3(SPAWN_POS.x, SPAWN_POS.y + SLIDER_HEIGHT * currPos, SPAWN_POS.z);
    }
    public float getTurnProgress() => (GameController.Instance.getState().turnCount - activatedTurn) / (float)turnDur;
    public float getNumTurnsLeft() => (activatedTurn + turnDur) - GameController.Instance.getState().turnCount;
    public int compareSliders(EnemySkill other) => getNumTurnsLeft() == other.getNumTurnsLeft() ? other.getTurnProgress().CompareTo(getTurnProgress()) : getNumTurnsLeft().CompareTo(other.getNumTurnsLeft()); 
    public void toggleActivate(bool isActivated) => activatedTurn = isActivated ? GameController.Instance.getState().turnCount : -1;
    public bool isActivated() => activatedTurn != -1;
    public bool useSkillNow() => whenToUse();
    public WaitUntil getAnimIsOver() => animIsOver;
    public bool oneTurnOnly() => turnDur == 0;
    public bool hasEndAnim() { return getEndAnim() != -1f; }

    public virtual float getStartAnim() => startAnim;
    public virtual float getEndAnim() => endAnim;
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

        if (useFirstSkill) {
            for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
                cGroup.alpha = currTime / FADE_ANIM_TIME;
                yield return null;
            }
            cGroup.alpha = 1f;
        }

        StartCoroutine(slideAnim(useFirstSkill ? getStartAnim() : getEndAnim()));
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
        Vector3 oldVectPos = rectTrans.anchoredPosition, newVectPos = new Vector3(SPAWN_POS.x, SPAWN_POS.y + SLIDER_HEIGHT * newPos, SPAWN_POS.z);
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
    private string enemyID = null;
    private float delay1 = 0f, delay2 = -1f;
    private List<Vector2Int> markOrder = null;
    private Func<Orb, bool> markCondition = null;
    private Func<Orb, ORB_VALUE> setCondition = null;
    private Func<Orb, int> incCondition = null;
    private int numShuffles = 0;

    public static EnemyBoardSkill ShuffleSkill(Func<bool> wtu, int numShuffles, float delay, Transform parent) {
        EnemyBoardSkill boardSkill = Create(parent).AddComponent<EnemyBoardSkill>();
        boardSkill.initValues(EnemySkillType.SHUFFLE, wtu, 0, 0);
        boardSkill.numShuffles = numShuffles;
        boardSkill.delay1 = delay;
        return boardSkill;
    }
    public static EnemyBoardSkill MarkOrderSkill(Func<bool> wtu, string enemyID, List<Vector2Int> markOrder, float delay1, Transform parent, int turnDur = 0, float delay2 = 0) {
        EnemyBoardSkill boardSkill = Create(parent).AddComponent<EnemyBoardSkill>();
        boardSkill.initValues(EnemySkillType.MARK, wtu, turnDur, 0);
        boardSkill.skillHelper1(enemyID, delay1);
        if (turnDur > 0) boardSkill.skillHelper2(EnemySkillType.MARK, delay2);
        boardSkill.markOrder = markOrder;
        return boardSkill;
    }
    public static EnemyBoardSkill MarkIfSkill(Func<bool> wtu, string enemyID, Func<Orb, bool> markCondition, float delay1, Transform parent, int turnDur = 0, float delay2 = 0) {
        EnemyBoardSkill boardSkill = Create(parent).AddComponent<EnemyBoardSkill>();
        boardSkill.initValues(EnemySkillType.MARK, wtu, turnDur, 0);
        boardSkill.skillHelper1(enemyID, delay1);
        if (turnDur > 0) boardSkill.skillHelper2(EnemySkillType.MARK, delay2);
        boardSkill.markCondition = markCondition;
        return boardSkill;
    }
    private void skillHelper1(string enemyID, float delay1) { this.enemyID = enemyID; this.delay1 = delay1; }

    public void addSetSkill(float delay, Func<Orb, ORB_VALUE> setCondition) {
        skillHelper2(EnemySkillType.REPLACE, delay);
        this.setCondition = setCondition;
    }

    public void addIncSkill(float delay, Func<Orb, int> incCondition) {
        skillHelper2(EnemySkillType.DECREMENT, delay);
        this.incCondition = incCondition;
    }

    public void addRmvSkill(float delay) {
        skillHelper2(EnemySkillType.CLEAR, delay);
    }

    private void skillHelper2(EnemySkillType endSkill, float delay2) {
        this.endSkill = endSkill;
        this.delay2 = delay2;
    }

    public override float getStartAnim() {
        if (numShuffles > 0) return delay1 * numShuffles;
        if (markOrder != null) return delay1 * markOrder.Count;
        return delay1 * Board.Instance.getNumValidOrbs(markCondition);
    }
    public override float getEndAnim() => delay2 == -1 ? -1 : delay2 * Board.Instance.getAllMarkedOrbsBy(enemyID, null).Count;
    public override IEnumerator onActivate(Enemy e) {
        switch (startSkill) {
            case EnemySkillType.MARK:
                if (markOrder != null) yield return StartCoroutine(Board.Instance.markOrbsInOrder(enemyID, markOrder, delay1));
                else if (markCondition != null)  yield return StartCoroutine(Board.Instance.markAllOrbsIf(enemyID, markCondition, delay1));
                break;
            case EnemySkillType.SHUFFLE: yield return StartCoroutine(Board.Instance.shuffleBoard(numShuffles, delay1)); break;
        }
    }
    public override IEnumerator onEnd(Enemy e) {
        if (turnDur != 0) markOrder = null;
        switch (endSkill) {
            case EnemySkillType.CLEAR: yield return StartCoroutine(Board.Instance.removeAllMarkedOrbsBy(enemyID, delay2, markOrder)); break;
            case EnemySkillType.REPLACE: yield return StartCoroutine((Board.Instance.setAllMarkedOrbsBy(enemyID, setCondition, delay2, markOrder))); break;
            case EnemySkillType.DECREMENT: yield return StartCoroutine((Board.Instance.incrementAllMarkedOrbsBy(enemyID, incCondition, delay2, markOrder))); break;
            default: Board.Instance.unmarkAllOrbsBy(enemyID); break;  // so this doesnt run LOL
        }
    }
}
public class EnemyTimer : EnemySkill {
    private Func<int> getDOT;
    private int currDOT = 0;
    public static EnemyTimer Create(Func<bool> wtu, Func<int> getDOT, Transform parent) {
        EnemyTimer eTimer = Create(parent).AddComponent<EnemyTimer>();
        eTimer.initValues(EnemySkillType.TIMER, wtu, 1, 0f);
        eTimer.getDOT = getDOT;
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
        orbSkill.initValues(EnemySkillType.ORB_SPAWN, wtu, turnDur, 0f);  // TO-DO: how tf do i show the name...
        orbSkill.orbSpawnRates = orbSpawnRates;
        return orbSkill;
    }
    public override IEnumerator onActivate(Enemy e) { e.getState().orbSpawnRates = orbSpawnRates; yield return null; }
    public override IEnumerator onDestroy(Enemy e) { e.getState().orbSpawnRates = Board.getDefaultOrbSpawnRates(); yield return null; }
}