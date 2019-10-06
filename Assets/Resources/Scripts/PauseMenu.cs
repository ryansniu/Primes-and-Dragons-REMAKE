using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public Image backgImg;
    public GameObject pauseUI;
    public Button pauseButton;

    public void enterPause() {
        StartCoroutine(enableAnimation());
    }
    private IEnumerator enableAnimation() {
        GameController.isPaused = true;
        pauseButton.interactable = false;
        backgImg.gameObject.SetActive(true);
        yield return StartCoroutine(toggleBackground(true));
        pauseUI.SetActive(true);
    }

    public void exitPause() {
        StartCoroutine(disableAnimation());
    }
    private IEnumerator disableAnimation() {
        pauseUI.SetActive(false);
        pauseButton.interactable = true;
        yield return StartCoroutine(toggleBackground(false));
        backgImg.gameObject.SetActive(false);
        GameController.isPaused = false;
    }

    private IEnumerator toggleBackground(bool darken) {
        float animationTime = 0.25f;
        float currTime = 0f;
        Color darkFG = new Color(0f, 0f, 0f, 0.75f);
        while (currTime < animationTime) {
            if (darken) backgImg.color = Color.Lerp(Color.clear, darkFG, currTime / animationTime);
            else backgImg.color = Color.Lerp(darkFG, Color.clear, currTime / animationTime);
            currTime += Time.deltaTime;
            yield return null;
        }
        if (darken) backgImg.color = darkFG;
        else backgImg.color = Color.clear;
    }
}
