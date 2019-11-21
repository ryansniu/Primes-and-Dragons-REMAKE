using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public Button pauseButton;
    public Image backgImg;
    public GameObject pauseUI;
    public CanvasGroup pauseCanvas;

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
    private IEnumerator disableAnimation() {
        pauseButton.interactable = true;
        yield return StartCoroutine(fadeAnimation(false));
        pauseUI.SetActive(false);
        backgImg.gameObject.SetActive(false);
        GameController.isPaused = false;
    }

    private IEnumerator fadeAnimation(bool fadeIn) {
        float animationTime = 0.25f;
        float currTime = 0f;
        Color darkFG = new Color(0f, 0f, 0f, 0.75f);
        while (currTime < animationTime) {
            float timeRatio = currTime / animationTime;
            if (fadeIn) {
                backgImg.color = Color.Lerp(Color.clear, darkFG, Mathf.SmoothStep(0f, 1f, timeRatio));
                pauseCanvas.alpha = timeRatio;
            }
            else {
                backgImg.color = Color.Lerp(darkFG, Color.clear, Mathf.SmoothStep(0f, 1f, timeRatio));
                pauseCanvas.alpha = 1f - timeRatio;
            }
            currTime += Time.deltaTime;
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
