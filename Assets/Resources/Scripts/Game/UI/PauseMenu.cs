using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public GameController gc;
    public Button pauseButton;
    public Image backgImg;
    public GameObject pauseUI;
    public CanvasGroup pauseCanvas;

    public Slider musSlider;
    public Slider sfxSlider;

    private const float FADE_ANIM_TIME = 0.25f;

    public void enterPause() {
        StartCoroutine(enableAnimation());
    }
    private IEnumerator enableAnimation() {
        GameController.isPaused = true;
        pauseButton.interactable = false;
        backgImg.gameObject.SetActive(true);
        pauseUI.SetActive(true);
        yield return StartCoroutine(fadeAnimation(true));
    }

    public void exitPause() {
        StartCoroutine(disableAnimation());
    }

    public void exitGame() {
        gc.SaveGame();
        LoadingScreen.Instance.Show(Scenes.LoadAsync("Title"));
    }
    private IEnumerator disableAnimation() {
        pauseButton.interactable = true;
        yield return StartCoroutine(fadeAnimation(false));
        pauseUI.SetActive(false);
        backgImg.gameObject.SetActive(false);
        GameController.isPaused = false;
    }

    private IEnumerator fadeAnimation(bool fadeIn) {
        Color darkFG = new Color(0f, 0f, 0f, 0.75f);

        for(float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            float timeRatio = currTime / FADE_ANIM_TIME;
            if (fadeIn) {
                backgImg.color = Color.Lerp(Color.clear, darkFG, Mathf.SmoothStep(0f, 1f, timeRatio));
                pauseCanvas.alpha = timeRatio;
            }
            else {
                backgImg.color = Color.Lerp(darkFG, Color.clear, Mathf.SmoothStep(0f, 1f, timeRatio));
                pauseCanvas.alpha = 1f - timeRatio;
            }
            yield return null;
        }
        if (fadeIn) {
            backgImg.color = darkFG;
            pauseCanvas.alpha = 1f;
        }
        else {
            backgImg.color = Color.clear;
            pauseCanvas.alpha = 0f;
        }
    }
}
