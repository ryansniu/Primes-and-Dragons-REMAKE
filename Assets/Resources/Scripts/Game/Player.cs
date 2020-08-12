using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class PlayerState {
    public float currHealth;
    public int maxHealth;
}

public class Player : MonoBehaviour {
    public static Player Instance;
    private WaitUntil DELTA_ZERO;
    [SerializeField] private HealthBar HPBar = default;
    [SerializeField] private Image HPBarIMG = default, heartIMG = default;
    [SerializeField] private SpriteRenderer boardTopSpr = default;
    private Sprite[] playerHPBars, playerHearts, boardTops;

    private PlayerState currState = new PlayerState();
    private string causeOfDeath = "alive";

    private volatile float deltaHealth;
    private bool isUpdatingHealth = false;
    private float HPSpeed = 0f;
    private readonly Vector3 HPDelta_POS = new Vector3(400f, 42f, 2f);

    // vv SAVING AND LOADING vv
    public PlayerState getState() => currState;
    public void setState(PlayerState ps) {
        currState = ps;
        isUpdatingHealth = true;
    }
    // ^^ SAVING AND LOADING ^^

    void Awake() {
        Instance = this;
        playerHPBars = Resources.LoadAll<Sprite>("Sprites/Main Screen/Player UI/Health Bars");
        playerHearts = Resources.LoadAll<Sprite>("Sprites/Main Screen/Player UI/Hearts");
        boardTops = Resources.LoadAll<Sprite>("Sprites/Main Screen/Board/Board Tops");
        DELTA_ZERO = new WaitUntil(() => deltaHealth == 0f);
    }
    void Update() {
        if(isUpdatingHealth){
            if(deltaHealth == 0f){
                HPBar.setHPNumColor(Color.black);
                isUpdatingHealth = false;
            }
            else{
                HPBar.setHPNumColor(deltaHealth > 0f ? ColorPalette.getColor(6, 2) : ColorPalette.getColor(1, 1));
                float diff = Mathf.Sign(deltaHealth) * Mathf.Min(Time.deltaTime * HPSpeed, Math.Abs(deltaHealth));
                if (currState.currHealth >= currState.maxHealth && diff > 0) currState.currHealth += diff / 2; // Nerfed the overheals by half TO-DO: bruh it doesn't accurately show hp delta num anymore
                else currState.currHealth += diff;
                deltaHealth -= diff;
            }
            updateHPBar((int)Mathf.Round(currState.currHealth), currState.maxHealth);
        }
    }
    public IEnumerator addToHealth(int value, Color? col = null) {
        deltaHealth += value;
        HPSpeed = Mathf.Clamp(Math.Abs(deltaHealth), currState.maxHealth / 5, currState.maxHealth) * 2;
        HPDeltaNum.Create(HPDelta_POS, value, col);
        isUpdatingHealth = true;
        yield return DELTA_ZERO;
    }
    public IEnumerator inflictDOT(int DOT) {
        DOT = (int)Mathf.Min(DOT, currState.maxHealth - currState.currHealth);
        setHPSprites(DOT);
        Color hpDeltaCol = DOT > 0 ? ColorPalette.getColor(4, 1) : ColorPalette.getColor(1, 2);
        yield return StartCoroutine(addToHealth(DOT, hpDeltaCol));
    }
    public IEnumerator resetDeltaHealth(){
        yield return DELTA_ZERO;
        currState.currHealth = (int)Mathf.Round(Mathf.Clamp(currState.currHealth, 0, currState.maxHealth));
        updateHPBar((int)Mathf.Round(currState.currHealth), currState.maxHealth);
        setHPSprites(0);
    }
    public IEnumerator setMaxHealth(int value) {
        if(currState.maxHealth == value) yield break;
        int oldMaxHealth = currState.maxHealth;
        currState.maxHealth = value;
        StartCoroutine(addToHealth(currState.maxHealth - oldMaxHealth));
        yield return StartCoroutine(resetDeltaHealth());
    }
    public bool isAlive() => currState.currHealth > 0;
    public void updateHPBar(int currHealth, int maxHealth){
        HPBar.displayHP(currHealth, maxHealth);
        float ratio = (float)currHealth/maxHealth;
        int HPIndex = 0, topIndex = 0;
        if (ratio > 0) topIndex++;
        if (ratio > 0.25f) HPIndex++;
        if (ratio > 0.50f) HPIndex++;
        if (ratio > 1) topIndex++;
        HPBarIMG.sprite = playerHPBars[HPIndex];
        boardTopSpr.sprite = boardTops[topIndex];
    }
    public void setCauseOfDeath(string s) => causeOfDeath = (s == "alive" || causeOfDeath == "alive" ? s : causeOfDeath);
    public string getCauseOfDeath() => causeOfDeath;

    private void setHPSprites(int DOT) {
        int heartIndex = 0;
        if (DOT >= 0) heartIndex++;
        if (DOT > 0) heartIndex++;
        heartIMG.sprite = playerHearts[heartIndex];
    }
}