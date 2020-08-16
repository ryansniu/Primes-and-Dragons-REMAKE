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
    protected int activatedTurn = -1, lastActivatedTurn = -1, turnDur = 0; // if turnDur = -1, that means the skill lasts forever
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
    public int getLastTurnStart() => lastActivatedTurn;
    public void setStartTurn(int turnStart) {
        activatedTurn = turnStart;
        endAnim = getEndAnim();
        updateTextColor(true);
        BG.value = 1f - getTurnProgress();
        cGroup.alpha = 1f;
        skillText.text = getSkillText(true);
    }
    public void setLastStartTurn(int lastTurnStart) => lastActivatedTurn = lastTurnStart;
    public void setPos(int pos) {
        currPos = pos;
        rectTrans.anchoredPosition = new Vector3(SPAWN_POS.x, SPAWN_POS.y + SLIDER_HEIGHT * currPos, SPAWN_POS.z);
    }
    public float getTurnProgress() {
        if (infiniteTurns()) return 0f;
        if (oneTurnOnly()) return 1f;
        return (GameController.Instance.getCurrTurn() - activatedTurn) / (float)turnDur;
    }
    public int getNumTurnsLeft() => infiniteTurns() ? 99 : (activatedTurn + turnDur) - GameController.Instance.getCurrTurn();
    public int compareSliders(EnemySkill other) {
        if (getNumTurnsLeft() == other.getNumTurnsLeft()) {
            int compareEndAnims = (hasEndAnim() ? -1 : 0) - (other.hasEndAnim() ? -1 : 0);
            return compareEndAnims == 0 ? other.getTurnProgress().CompareTo(getTurnProgress()) : compareEndAnims;
        }
        return getNumTurnsLeft().CompareTo(other.getNumTurnsLeft());
    }
    public void toggleActivate(bool isActivated) => activatedTurn = isActivated ? GameController.Instance.getCurrTurn() : -1;
    public bool isActivated() => activatedTurn != -1;
    public int getActivatedTurn() => activatedTurn;
    public int getLastActivatedTurn() => lastActivatedTurn;
    public bool useSkillNow() => whenToUse();
    public WaitUntil getAnimIsOver() => animIsOver;
    public bool oneTurnOnly() => turnDur == 0;
    public bool infiniteTurns() => turnDur == -1;
    public bool hasEndAnim() => endAnim != -1f;

    public virtual float getStartAnim() => startAnim;
    public virtual float getEndAnim() => endAnim;
    public virtual IEnumerator onActivate(Enemy e) { activatedTurn = GameController.Instance.getCurrTurn(); lastActivatedTurn = activatedTurn; yield return null; }
    public virtual IEnumerator onEnd(Enemy e) { yield return null; }
    public virtual void onDestroy(Enemy e) => activatedTurn = -1;
    public virtual string getSkillText(bool useFirstSkill) => useFirstSkill ? startSkill.ToString().ToLower() : endSkill.ToString().ToLower();
    
    public IEnumerator fadeInAnim(bool useFirstSkill) {
        startAnim = getStartAnim();
        endAnim = getEndAnim();

        isAnimating = true;
        skillText.text = getSkillText(useFirstSkill);
        skillText.color = oneTurnOnly() && hasEndAnim() && useFirstSkill ? ColorPalette.getColor(5, 3) : Color.white;
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
        
        StartCoroutine(slideAnim(useFirstSkill));
    }
    public IEnumerator slideAnim(bool useFirstSkill) {
        float dur = Mathf.Max(useFirstSkill ? startAnim : endAnim, MIN_SLIDE_TIME);
        for (float currTime = 0f; currTime < dur; currTime += Time.deltaTime) {
            BG.value = currTime / dur;  // smooth step?
            yield return null;
        }
        BG.value = 1;
        updateTextColor(useFirstSkill);
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
        if (BG.value == 0 && hasEndAnim()) {
            skillText.text = getSkillText(false);
            skillText.color = Color.white;
        }
        else updateTextColor(true);
        isAnimating = false;
    }
    private void updateTextColor(bool useFirstSkill) {
        Color textColor = Color.white;
        switch (getNumTurnsLeft()) {
            case 99: textColor = ColorPalette.getColor(2, 1); break;
            case 2: textColor = ColorPalette.getColor(4, 1); break;
            case 1: textColor = hasEndAnim() ? ColorPalette.getColor(1, 1) : ColorPalette.getColor(6, 2); break;
            case 0: textColor = hasEndAnim() && useFirstSkill ? skillText.color : ColorPalette.getColor(2, 2); break;
        }
        skillText.color = textColor;
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
            cGroup.alpha = 1f - currTime / DESTROY_ANIM_TIME;
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
        yield return StartCoroutine(base.onActivate(e));
        GameObject target = getTarget();
        if (target == null) yield break;
        if (target.GetComponent<Enemy>() != null) yield return StartCoroutine(target.GetComponent<Enemy>().takeDMG(getDmg()));
        else if (target.GetComponent<Player>() != null) yield return StartCoroutine(Player.Instance.addToHealth(getDmg()));
    }
}
public class EnemyHPBuff : EnemySkill {
    private Func<Enemy> getTarget;
    private Enemy currTarget;
    private EnemyBuffs buff;
    private bool isRandom;
    public static EnemyHPBuff Create(Func<bool> wtu, EnemyBuffs buff, Func<Enemy> getTarget, int turnDur, Transform parent) {
        EnemyHPBuff HPbuff = Create(parent).AddComponent<EnemyHPBuff>();
        HPbuff.initValues(convertBuffToSkill(buff), wtu, turnDur, 0f);
        HPbuff.buff = buff;
        HPbuff.getTarget = getTarget;
        return HPbuff;
    }
    private static EnemySkillType convertBuffToSkill(EnemyBuffs buff) {
        switch (buff) {
            case EnemyBuffs.DMG_REFLECT: return EnemySkillType.REFLECT;
            case EnemyBuffs.DMG_MITI_50: return EnemySkillType.MITIGATE;
            case EnemyBuffs.DMG_ABSORB: return EnemySkillType.ABSORB;
            default: return EnemySkillType.REFLECT;
        }
    }
    public override string getSkillText(bool useFirstSkill) => convertBuffToSkill(buff).ToString().ToLower();
    public void toggleIsRandom(bool isRandom) {
        this.isRandom = isRandom;
        buff = (EnemyBuffs)UnityEngine.Random.Range(1, 4);
    }
    public override IEnumerator onActivate(Enemy e) {
        yield return StartCoroutine(base.onActivate(e));
        currTarget = getTarget();
        if (currTarget != null && currTarget.isAlive()) currTarget.setBuff(buff);
    }
    public override void onDestroy(Enemy e) {
        if (currTarget != null && currTarget.isAlive()) currTarget.setBuff(EnemyBuffs.NONE);
        if (isRandom) buff = (EnemyBuffs)UnityEngine.Random.Range(1, 4);
        base.onDestroy(e);
    }
}
public class EnemyBoardSkill : EnemySkill {
    private string skillID = null;
    private float delay1 = 0f, delay2 = -1f;
    private Func<List<Vector2Int>> markOrder = null;
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
    public static EnemyBoardSkill MarkOrderSkill(Func<bool> wtu, Func<List<Vector2Int>> markOrder, float delay1, Transform parent, int turnDur = 0) {
        EnemyBoardSkill boardSkill = Create(parent).AddComponent<EnemyBoardSkill>();
        boardSkill.skillHelper1(wtu, turnDur, delay1);
        boardSkill.markOrder = markOrder;
        return boardSkill;
    }
    public static EnemyBoardSkill MarkIfSkill(Func<bool> wtu, Func<Orb, bool> markCondition, float delay1, Transform parent, int turnDur = 0) {
        EnemyBoardSkill boardSkill = Create(parent).AddComponent<EnemyBoardSkill>();
        boardSkill.skillHelper1(wtu, turnDur, delay1);
        boardSkill.markCondition = markCondition;
        return boardSkill;
    }
    private void skillHelper1(Func<bool> wtu, int td, float delay1) {
        initValues(EnemySkillType.MARK, wtu, td, 0);
        this.delay1 = delay1;
        if (turnDur > 0) skillHelper2(EnemySkillType.MARK, delay2);
    }

    public void addSetSkill(float delay, Func<Orb, ORB_VALUE> setCondition) {
        skillHelper2(EnemySkillType.REPLACE, delay);
        this.setCondition = setCondition;
    }

    public void addIncSkill(float delay, Func<Orb, int> incCondition) {
        skillHelper2(EnemySkillType.DECREMENT, delay);
        this.incCondition = incCondition;
    }

    public void addRmvSkill(float delay) => skillHelper2(EnemySkillType.CLEAR, delay);
    public void addMarkRemainSkill() => skillHelper2(EnemySkillType.MARK, -1f);

    private void skillHelper2(EnemySkillType endSkill, float delay2) {
        this.endSkill = endSkill;
        this.delay2 = delay2;
    }

    public override float getStartAnim() {
        if (numShuffles > 0) return delay1 * numShuffles;
        if (markOrder != null) return delay1 * markOrder().Count;
        return delay1 * Board.Instance.getNumValidOrbs(markCondition);
    }
    public override float getEndAnim() {
        if (delay2 == -1f) return -1f;
        int numOrbsChanged = Board.Instance.getAllMarkedOrbsBy(skillID, null).Count;
        return delay2 * numOrbsChanged + (endSkill == EnemySkillType.CLEAR ? delay2 * Board.DISAPPEAR_DELTA + Board.DISAPPEAR_DURATION : 0);
    }
    public override IEnumerator onActivate(Enemy e) {
        yield return StartCoroutine(base.onActivate(e));
        skillID = e.getSkillID(this, activatedTurn);
        switch (startSkill) {
            case EnemySkillType.MARK:
                if (markOrder != null) yield return StartCoroutine(Board.Instance.markOrbsInOrder(skillID, markOrder(), delay1));
                else if (markCondition != null)  yield return StartCoroutine(Board.Instance.markAllOrbsIf(skillID, markCondition, delay1));
                break;
            case EnemySkillType.SHUFFLE: yield return StartCoroutine(Board.Instance.shuffleBoard(numShuffles, delay1)); break;
        }
    }
    public override IEnumerator onEnd(Enemy e) {
        skillID = e.getSkillID(this, activatedTurn);
        lastActivatedTurn = activatedTurn;
        List<Vector2Int> tempOrder = oneTurnOnly() && markOrder != null ? markOrder() : null;
        switch (endSkill) {
            case EnemySkillType.CLEAR: yield return StartCoroutine(Board.Instance.removeAllMarkedOrbsBy(skillID, delay2, tempOrder)); break;
            case EnemySkillType.REPLACE: yield return StartCoroutine((Board.Instance.setAllMarkedOrbsBy(skillID, setCondition, delay2, tempOrder))); break;
            case EnemySkillType.DECREMENT: yield return StartCoroutine((Board.Instance.incrementAllMarkedOrbsBy(skillID, incCondition, delay2, tempOrder))); break;
            case EnemySkillType.MARK: break;
            // default: Board.Instance.unmarkAllOrbsBy(skillID); break;
        }
    }
    public override void onDestroy(Enemy e) {
        if (endSkill != EnemySkillType.MARK) Board.Instance.unmarkAllOrbsBy(skillID);
        base.onDestroy(e);
    }
}
public class EnemyTimer : EnemySkill {
    private string skillID = null;
    private Func<Orb, bool> rmvCondition = null;
    private Func<Orb, ORB_VALUE> setCondition = null;
    private Func<Orb, int> incCondition = null;
    private Func<int> getDOT;
    private float skillRate = 0f, internalTimer = 0f;
    private WaitForSeconds timeDelay;
    private bool isRunning = false;
    public static EnemyTimer Create(Func<bool> wtu, float skillRate, int turnDur, Transform parent) {
        EnemyTimer eTimer = Create(parent).AddComponent<EnemyTimer>();
        eTimer.initValues(EnemySkillType.TIMER, wtu, turnDur, 0f);
        eTimer.skillRate = skillRate;
        return eTimer;
    }

    public void addSetSkill(Func<Orb, ORB_VALUE> setCondition) {
        endSkill = EnemySkillType.REPLACE;
        this.setCondition = setCondition;
    }
    public void addIncSkill(Func<Orb, int> incCondition) {
        endSkill = EnemySkillType.DECREMENT;
        this.incCondition = incCondition;
    }
    public void addRmvSkill(Func<Orb, bool> rmvCondition) {
        endSkill = EnemySkillType.CLEAR;
        this.rmvCondition = rmvCondition;
    }
    public void addDOTSkill(Func<int> getDOT) {
        endSkill = EnemySkillType.ATTACK;
        this.getDOT = getDOT;
    }

    public void addTimeDelay(float offSet) => timeDelay = new WaitForSeconds(offSet);
    public override IEnumerator onActivate(Enemy e) {
        yield return StartCoroutine(base.onActivate(e));
        skillID = e.getSkillID(this, activatedTurn);
    }

    public void toggleSkill(bool isRunning) {
        if (this.isRunning != isRunning) {
            this.isRunning = isRunning;
            if (isRunning) StartCoroutine(useTimerSkill());
        }
    }
    private IEnumerator useTimerSkill() {
        if (timeDelay != null) yield return timeDelay;
        internalTimer = skillRate;
        while (isRunning) {
            while (internalTimer < skillRate) {
                internalTimer += Time.deltaTime;
                yield return null;
            }
            if (!isRunning) break;
            System.Random rand = new System.Random();
            List<Orb> potentialOrbs = new List<Orb>();
            Orb selectedOrb;
            switch (endSkill) {
                case EnemySkillType.CLEAR:
                    for (int c = 0; c < Board.COLUMNS; c++) {
                        for (int r = 0; r < Board.ROWS; r++) {
                            Orb o = Board.Instance.getOrb(c, r);
                            if (rmvCondition(o) && !o.getIsMarkedBy(skillID)) potentialOrbs.Add(o);
                        }
                    }
                    if(potentialOrbs.Count > 0) {
                        selectedOrb = potentialOrbs[rand.Next(potentialOrbs.Count)];
                        Vector2Int gridPos = selectedOrb.getGridPos();
                        yield return StartCoroutine(Board.Instance.markOrbAt(gridPos.x, gridPos.y, skillID, 0f));
                        Board.Instance.displayNumBar();
                    }
                    break;
                case EnemySkillType.REPLACE:
                    for (int c = 0; c < Board.COLUMNS; c++) {
                        for (int r = 0; r < Board.ROWS; r++) {
                            Orb o = Board.Instance.getOrb(c, r);
                            if(setCondition(o) != o.getOrbValue()) potentialOrbs.Add(o);
                        }
                    }
                    if (potentialOrbs.Count > 0) {
                        selectedOrb = potentialOrbs[rand.Next(potentialOrbs.Count)];
                        selectedOrb.changeValue(setCondition(selectedOrb));
                        Board.Instance.displayNumBar();
                    }
                    break;
                case EnemySkillType.DECREMENT:
                    for (int c = 0; c < Board.COLUMNS; c++) {
                        for (int r = 0; r < Board.ROWS; r++) {
                            Orb o = Board.Instance.getOrb(c, r);
                            int deltaVal = incCondition(o);
                            if (deltaVal != 0 && o.isDigit() && o.getIntValue() + deltaVal <= 9 && o.getIntValue() + deltaVal >= 0) potentialOrbs.Add(o);
                        }
                    }
                    if (potentialOrbs.Count > 0) {
                        selectedOrb = potentialOrbs[rand.Next(potentialOrbs.Count)];
                        selectedOrb.incrementValue(incCondition(selectedOrb));
                        Board.Instance.displayNumBar();
                    }
                    break;
                case EnemySkillType.ATTACK:
                    yield return StartCoroutine(Player.Instance.inflictDOT(getDOT()));
                    break;
            }
            internalTimer = 0;
        }
    }
    public IEnumerator clearAllMarkedOrbs() { if(endSkill == EnemySkillType.CLEAR) yield return StartCoroutine(Board.Instance.removeAllMarkedOrbsBy(skillID, 0f)); }
}
public class EnemyOrbSkill : EnemySkill {
    private Func<ORB_VALUE, OrbSpawnRate> newSpawnRates;
    public static EnemyOrbSkill Create(Func<bool> wtu, Func<ORB_VALUE, OrbSpawnRate> newSpawnRates, int turnDur, Transform parent) {
        EnemyOrbSkill orbSkill = Create(parent).AddComponent<EnemyOrbSkill>();
        orbSkill.initValues(EnemySkillType.ORB_SPAWN, wtu, turnDur, 0f);
        orbSkill.newSpawnRates = newSpawnRates;
        return orbSkill;
    }
    public override string getSkillText(bool useFirstSkill) {
        return base.getSkillText(useFirstSkill);  // TO-DO: FIX THIS
    }
    public override IEnumerator onActivate(Enemy e) {
        yield return StartCoroutine(base.onActivate(e));
        OrbSpawnRate[] newVals = Board.getDefaultOrbSpawnRates();
        foreach (ORB_VALUE orbVal in Enum.GetValues(typeof(ORB_VALUE))) newVals[(int)orbVal] = newSpawnRates(orbVal);
        e.getState().orbSpawnRates = newVals;
        GameController.Instance.adjustOrbRates();
    }
    public override void onDestroy(Enemy e) {
        e.getState().orbSpawnRates = Board.getDefaultOrbSpawnRates();
        GameController.Instance.adjustOrbRates();
        base.onDestroy(e);
    }
}