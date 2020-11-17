using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen Instance;
    private const float MIN_TIME_TO_SHOW = 1f;
    private const float DOT_DELAY = MIN_TIME_TO_SHOW / 2f;
    private readonly WaitForSeconds HIDE_DELAY = new WaitForSeconds(MIN_TIME_TO_SHOW);
    private AsyncOperation currentLoadingOperation;
    private bool isLoading;
    private int numDots = 1;
    private float timeElapsed, dotTimeElapsed;
    private Animator animator;
    private bool didTriggerFadeOutAnimation;

    [SerializeField]
    private TextMeshProUGUI loadNum = default, loadText = default;
    [SerializeField]
    private Image bg = default;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
        animator = GetComponent<Animator>();
        Hide();
    }
    private void Update() {
        if (isLoading) {
            if (currentLoadingOperation.isDone && !didTriggerFadeOutAnimation) {
                animator.SetTrigger("Hide");
                didTriggerFadeOutAnimation = true;
            }
            else {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= MIN_TIME_TO_SHOW) currentLoadingOperation.allowSceneActivation = true;
                updateLoading();
            }
        }
    }

    private void updateLoading() {
        dotTimeElapsed += Time.deltaTime;
        if (dotTimeElapsed >= DOT_DELAY) {
            numDots = (numDots % 3) + 1;
            loadText.text = "Loading";
            for (int i = 0; i < numDots; i++) loadText.text += ".";
            dotTimeElapsed -= DOT_DELAY;
        }
    }

    public void Show(AsyncOperation loadingOperation) {
        if (isLoading) return;
        setLoadColor();
        gameObject.SetActive(true);
        currentLoadingOperation = loadingOperation;
        currentLoadingOperation.allowSceneActivation = false;
        animator.SetTrigger("Show");
        didTriggerFadeOutAnimation = false;
        isLoading = true;
    }
    private void setLoadColor() {
        ORB_VALUE orbVal = (ORB_VALUE)(PlayerPrefs.HasKey("TitlePalette") ? PlayerPrefs.GetInt("TitlePalette") : Random.Range(0, 9));
        Color loadPrimary = ColorPalette.getPalette(orbVal, true);
        Color loadSecondary = ColorPalette.getPalette(orbVal, false);
        loadNum.color = loadPrimary;
        loadNum.text = "" + (int)orbVal;
        loadText.color = loadSecondary;
        bg.color = Color.black;
    }
    public IEnumerator HideDelay() {
        if (!gameObject.activeSelf) yield break;
        yield return HIDE_DELAY;
        Hide();
    }
    private void Hide() {
        gameObject.SetActive(false);
        currentLoadingOperation = null;
        isLoading = false;
        timeElapsed = 0f;
        loadText.text = "Loading.";
        numDots = 1;
    }
}