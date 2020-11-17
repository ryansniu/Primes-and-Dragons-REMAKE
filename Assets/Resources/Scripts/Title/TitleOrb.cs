using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleOrb : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/Title Orb";
    private const string ORB_PATH = "Sprites/Player Board/Orbs";
    private Sprite[] orbSprites;
    [SerializeField]
    private SpriteRenderer spr = default, bgSpr = default, pulseSpr = default;
    private Transform orbTrans;

    public static float spawnDelay = 1.25f;
    private const float bounceDelay = 1f;
    private bool bounced = false;
    private float timer = 0f;
    private Vector3 velocity = Vector3.zero;
    private float fallSpd = 1f;

    private float pulseSpd = 2000;
    private float radiusDiff = 200f;
    public static TitleOrb Create(Vector3 spawnPos, bool fallDir, ORB_VALUE val) {
        TitleOrb orb = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH), Vector3.zero, Quaternion.identity, TitleOrbPool.Instance.transform)).GetComponent<TitleOrb>();
        orb.setInitValues(spawnPos, fallDir, val);
        return orb;
    }
    void Awake() {
        orbTrans = spr.transform;
        orbSprites = Resources.LoadAll<Sprite>(ORB_PATH);
    }

    public void setInitValues(Vector3 spawnPos, bool fallDir, ORB_VALUE val) {
        bgSpr.transform.localScale = Vector3.zero;
        pulseSpr.transform.localScale = Vector3.zero;
        spr.sprite = orbSprites[(int)val * 2 + 1];
        name = val.ToString();
        orbTrans.position = spawnPos;
        velocity = (fallDir ? Vector3.left : Vector3.right) * 0.5f;
        bounced = false;
        timer = 0f;
        setColor(val);
    }

    private void setColor(ORB_VALUE val) {
        bgSpr.color = ColorPalette.getPalette(val, true);
        pulseSpr.color = ColorPalette.getPalette(val, false);
    }

    void Update() {
        fall();
        if (bounced) pulseColor();
    }
    private void fall() {
        orbTrans.position += velocity;
        timer += Time.deltaTime;
        velocity.y -= Time.deltaTime * fallSpd;
        if (timer >= bounceDelay && !bounced) {
            velocity.y *= -0.5f;
            bgSpr.transform.position = orbTrans.position + Vector3.forward * 9.9f;
            pulseSpr.transform.position = orbTrans.position + Vector3.forward * 10f;
            timer = 0;
            bounced = true;
        }
        if(bounced && timer >= spawnDelay) {
            timer = 0;
            bgSpr.transform.position += Vector3.forward;
            pulseSpr.transform.position += Vector3.forward;
        }
    }

    private void pulseColor() {
        Vector3 diff = Time.deltaTime * Vector3.one * pulseSpd;
        pulseSpr.transform.localScale += diff;
        if (pulseSpr.transform.localScale.x > radiusDiff) bgSpr.transform.localScale += diff;
    }
}
