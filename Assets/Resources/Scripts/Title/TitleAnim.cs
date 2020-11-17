using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnim : MonoBehaviour {
    [SerializeField] private Animator menuOptions = default, titleLogo = default;
    private const float TITLE_DURATION = 2f;
    private float introTimer = 0f;
    private bool introEnded = false;

    private List<GameObject> visibleOrbs;
    private const int MAX_ORBS = 4;
    private float orbTimer = 0f;

    void Start() {
        StartCoroutine(LoadingScreen.Instance.HideDelay());
        visibleOrbs = new List<GameObject>();
    }

    void Update() {
        if (!introEnded) {
            if ((introTimer >= TITLE_DURATION || Input.GetMouseButton(0) || Input.touchCount > 0)) {
                titleLogo.SetBool("IntroEnd", true);
                menuOptions.SetBool("IntroEnd", true);
                introEnded = true;
            }
            introTimer += Time.deltaTime;
        }
        else {
            orbTimer += Time.deltaTime;
            if (orbTimer >= TitleOrb.spawnDelay) {
                spawnTitleOrb();
                orbTimer = 0f;
            }
        }
    }

    void spawnTitleOrb() {
        ORB_VALUE randOrb = (ORB_VALUE)Random.Range(0, 9);
        bool fallDir = Random.value <= 0.5f;
        Vector3 orbSpawn = new Vector3((fallDir ? 1 : -1) * Random.Range(280f, 320f), Random.Range(240f, 400f));
        GameObject newOrb = TitleOrbPool.Instance.GetPooledOrb(orbSpawn, fallDir, randOrb);
        visibleOrbs.Add(newOrb);
        if (visibleOrbs.Count > MAX_ORBS) {
            TitleOrbPool.Instance.ReturnToPool(visibleOrbs[0]);
            visibleOrbs.RemoveAt(0);
        }
    }
}
