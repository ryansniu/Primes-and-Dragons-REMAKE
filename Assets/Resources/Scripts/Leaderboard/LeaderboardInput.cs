using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardInput : MonoBehaviour {
    [SerializeField] private Image backgImg = default;
    [SerializeField] private GameObject leaderboardInputUI = default;
    [SerializeField] private CanvasGroup leaderboardInputCanvas = default;
    [SerializeField] private TMP_InputField input = default;

    private const float UI_FADE_ANIM_TIME = 0.3f, FG_FADE_ANIM_TIME = 0.5f;
    private readonly WaitForSeconds FADE_DELAY = new WaitForSeconds(0.2f);
    private readonly WaitForSeconds END_DELAY = new WaitForSeconds(0.2f);

    private bool gotInput = false;
    public void setGotInput(bool gi) => gotInput = gi;

    public IEnumerator getInput() {
        leaderboardInputUI.SetActive(true);
        yield return StartCoroutine(fadeAnimation(true));
        yield return new WaitUntil(() => gotInput == true);
    }
    public string getName() => input.text;
    public IEnumerator exitInput() {
        yield return StartCoroutine(fadeAnimation(false));
        backgImg.gameObject.SetActive(false);
        if (gotInput) leaderboardInputUI.SetActive(false);
    }

    public IEnumerator fadeAnimation(bool fadeIn) {
        Color darkFG = new Color(0f, 0f, 0f, 1f);
        for (float currTime = 0f; currTime < UI_FADE_ANIM_TIME; currTime += Time.deltaTime) {
            float timeRatio = currTime / UI_FADE_ANIM_TIME;
            if (fadeIn) leaderboardInputCanvas.alpha = timeRatio;
            else if (gotInput) leaderboardInputCanvas.alpha = 1f - timeRatio;
            yield return null;
        }
        if (fadeIn) leaderboardInputCanvas.alpha = 1f;
        else if (gotInput) leaderboardInputCanvas.alpha = 0f;
        yield return FADE_DELAY;

        for(float currTime = 0f; currTime < FG_FADE_ANIM_TIME; currTime += Time.deltaTime){
            float timeRatio = currTime / FG_FADE_ANIM_TIME;
            if (!fadeIn) backgImg.color = Color.Lerp(darkFG, Color.clear, Mathf.SmoothStep(0f, 1f, timeRatio));
            yield return null;
        }
        if (!fadeIn) backgImg.color = Color.clear;
        yield return END_DELAY;
    }
}
