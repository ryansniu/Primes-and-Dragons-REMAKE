using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySkillUI : MonoBehaviour {
    public CanvasGroup cGroup;
    public Slider BG;
    public TextMeshProUGUI skillText;

    public const float FADE_ANIM_TIME = 0.25f;
    private const float MIN_ANIM_TIME = 0.25f;

    private bool isAnimating = false;
    private WaitUntil waitForAnimStop;
    
    void Awake() {
        waitForAnimStop = new WaitUntil(() => { return !isAnimating; });
    }

    public IEnumerator displaySkill(string name, float duration) {
        StartCoroutine(skillTextAnimation(name, duration));
        yield return new WaitForSeconds(FADE_ANIM_TIME);
    }

    private IEnumerator skillTextAnimation(string n, float dur) {
        yield return waitForAnimStop;
        isAnimating = true;
        skillText.text = n;
        dur = Math.Max(dur, MIN_ANIM_TIME);

        // Fade in.
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            cGroup.alpha = currTime / FADE_ANIM_TIME;
            yield return null;
        }
        cGroup.alpha = 1f;

        // Slider.
        for (float currTime = 0f; currTime < dur; currTime += Time.deltaTime) {
            BG.value = currTime / dur;
            yield return null;
        }
        BG.value = 1;

        // Fade out.
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            cGroup.alpha = 1f - currTime / FADE_ANIM_TIME;
            yield return null;
        }
        cGroup.alpha = 0f;

        // Finish animation.
        resetValues();
        yield return null;
        isAnimating = false;
    }
    public void resetValues() {  // BUG
        BG.value = 0;
        skillText.text = "";
    }
}
