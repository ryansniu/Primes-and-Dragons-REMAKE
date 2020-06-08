using UnityEngine;

public class TitleAnim : MonoBehaviour {
    public Animator menuOptions, background, titleLogo;

    private const double TITLE_DURATION = 2f;
    private double timer = 0.0;
    private bool introEnded = false;

    void Start() {
        StartCoroutine(LoadingScreen.Instance.HideDelay());
    }

    void Update() {
        if (!introEnded) {
            if ((timer >= TITLE_DURATION || Input.GetMouseButton(0) || Input.touchCount > 0)) {
                titleLogo.SetBool("IntroEnd", true);
                menuOptions.SetBool("IntroEnd", true);
                background.SetBool("IntroEnd", true);
                introEnded = true;
            }
            timer += Time.deltaTime;
        }
    }
}
