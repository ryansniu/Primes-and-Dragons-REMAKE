using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPDeltaNum : MonoBehaviour{
    private const string PREFAB_PATH = "Prefabs/UI/HPDeltaNum";
    public TextMeshProUGUI HPtext;
    private Transform trans;

    public static HPDeltaNum Create(Vector3 spawnPos, int value, float fontSize = 80f, Color? col = null) {
        HPDeltaNum hpdn = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<HPDeltaNum>();
        hpdn.initValues(value, fontSize, col);
        return hpdn;
    }
    void Awake() {
        trans = transform;
        trans.SetParent(GameObject.Find("Game Controller").GetComponentInChildren<Canvas>().transform, false);
    }

    private void initValues(int value, float fontSize, Color? col){
        HPtext.fontSize = fontSize;
        if (value > 0){
            HPtext.text = "+" + value.ToString();
            HPtext.color = ColorPalette.getColor(6, 2);
        }
        else if (value < 0){
            HPtext.text = value.ToString();
            HPtext.color = ColorPalette.getColor(1, 1);
        }
        else{
            HPtext.text = "=" + value.ToString();
            HPtext.color = ColorPalette.getColor(2, 2);
        }
        if (col != null) HPtext.color = (Color)col; //purple for poison?
        StartCoroutine(animate());
    }
    private IEnumerator animate(){
        float fadeIn_animTime = 0.2f;
        Vector3 fadeIn_moveDist = new Vector3(0f, 15f, 0f);
        float fadeOut_animTime = 0.4f;
        Vector3 fadeOut_moveDist = new Vector3(0f, 20f, 0f);

        float currTime = 0f;
        Color origColor = HPtext.color;
        Vector3 origPos = trans.position;
        while (currTime < fadeIn_animTime){
            float ratio = currTime/fadeIn_animTime;
            HPtext.color = new Color(origColor.r, origColor.g, origColor.b, (ratio * ratio));
            trans.position = origPos + (ratio * ratio) * fadeIn_moveDist;
            currTime += Time.deltaTime;
            yield return null;
        }
        currTime = 0f;
        origPos = trans.position;
        while (currTime < fadeOut_animTime){
            float ratio = currTime/fadeOut_animTime;
            HPtext.color = new Color(origColor.r, origColor.g, origColor.b, 1f - ratio);
            trans.position = origPos + ratio * fadeOut_moveDist;
            currTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
