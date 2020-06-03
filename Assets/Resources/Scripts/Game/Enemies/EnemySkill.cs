using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySkill : MonoBehaviour {
    public CanvasGroup cGroup;
    public Slider BG;
    public TextMeshProUGUI skillText;

    public const float FADE_ANIM_TIME = 0.25f;
    private const float MIN_ANIM_TIME = 0.25f;
    public IEnumerator displaySkill(string name, float duration) {
        skillText.text = name;
        StartCoroutine(skillTextAnimation(duration));
        yield return new WaitForSeconds(FADE_ANIM_TIME);
    }

    private IEnumerator skillTextAnimation(float dur) {
        float duration = Math.Max(dur, MIN_ANIM_TIME);

        // Fade in.
        for (float currTime = 0f; currTime < FADE_ANIM_TIME; currTime += Time.deltaTime) {
            cGroup.alpha = currTime / FADE_ANIM_TIME;
            yield return null;
        }
        cGroup.alpha = 1f;

        // Slider.
        for (float currTime = 0f; currTime < duration; currTime += Time.deltaTime) {
            BG.value = currTime / duration;
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
    }
    public void resetValues() {
        BG.value = 0;
        skillText.text = "";
    }
}
