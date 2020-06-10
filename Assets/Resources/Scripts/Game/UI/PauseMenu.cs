using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    [SerializeField] private Button pauseButton = default;
    [SerializeField] private Image backgImg = default;
    [SerializeField] private GameObject pauseUI = default;
    [SerializeField] private CanvasGroup pauseCanvas = default;
    [SerializeField] private Slider musSlider = default, sfxSlider = default;

    private const float FADE_ANIM_TIME = 0.25f;

    void Start() {
        musSlider.onValueChanged.AddListener(delegate { AudioController.Instance.setMusicVolume(musSlider.value); });
        musSlider.value = PlayerPrefs.GetFloat("musVol", 1f);
        sfxSlider.onValueChanged.AddListener(delegate { AudioController.Instance.setSFXVolume(sfxSlider.value); });
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 1f);
    }

    public void enterPause() => StartCoroutine(enableAnimation());
    private IEnumerator enableAnimation() {
        GameController.Instance.isPaused = true;
        pauseButton.interactable = false;
        backgImg.gameObject.SetActive(true);
        pauseUI.SetActive(true);
        yield return StartCoroutine(fadeAnimation(true));
    }

    public void exitPause() => StartCoroutine(disableAnimation());

    public void exitGame() {
        GameController.Instance.saveGame();
        LoadingScreen.Instance.Show(Scenes.LoadAsync("Title"));
    }
    private IEnumerator disableAnimation() {
        pauseButton.interactable = true;
        yield return StartCoroutine(fadeAnimation(false));
        pauseUI.SetActive(false);
        backgImg.gameObject.SetActive(false);
        GameController.Instance.isPaused = false;
    }

    private IEnumerator fadeAnimation(bool fadeIn) {
        Color darkFG = new Color(0f, 0f, 0f, 0.75f);
        for(float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            float timeRatio = fadeIn ? currTime / FADE_ANIM_TIME : 1f - currTime / FADE_ANIM_TIME;
            backgImg.color = Color.Lerp(Color.clear, darkFG, Mathf.SmoothStep(0f, 1f, timeRatio));
            pauseCanvas.alpha = timeRatio;
            yield return null;
        }
        backgImg.color = fadeIn ? darkFG : Color.clear;
        pauseCanvas.alpha = fadeIn ? 1f : 0f;
    }
}
