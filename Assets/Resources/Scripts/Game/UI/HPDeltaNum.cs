using System.Collections;
using UnityEngine;
using TMPro;

public class HPDeltaNum : MonoBehaviour{
    private const string PREFAB_PATH = "Prefabs/UI/HPDeltaNum";
    [SerializeField] private TextMeshProUGUI HPtext;
    private Transform trans;

    public static HPDeltaNum Create(Vector3 spawnPos, int value, Color? col = null, float fontSize = 80f) {
        HPDeltaNum hpdn = (Instantiate(Resources.Load<GameObject>(PREFAB_PATH), spawnPos, Quaternion.identity)).GetComponent<HPDeltaNum>();
        hpdn.initValues(value, col, fontSize);
        return hpdn;
    }
    void Awake() {
        trans = transform;
        trans.SetParent(GameObject.Find("Game Controller").GetComponentInChildren<Canvas>().transform, false);
    }

    private void initValues(int value, Color? col, float fontSize){
        HPtext.text = string.Concat(value >= 0 ? "+" : "", value.ToString());
        HPtext.fontSize = fontSize;

        if (col != null) HPtext.color = (Color)col; // TO-DO: purple for poison?
        else if (value > 0) HPtext.color = ColorPalette.getColor(6, 2);
        else if (value < 0) HPtext.color = ColorPalette.getColor(1, 1);
        else HPtext.color = ColorPalette.getColor(2, 2);

        StartCoroutine(animate());
    }
    private IEnumerator animate(){
        float fadeIn_animTime = 0.2f;
        Vector3 fadeIn_moveDist = new Vector3(0f, 15f, 0f);
        float fadeOut_animTime = 0.4f;
        Vector3 fadeOut_moveDist = new Vector3(0f, 20f, 0f);

        Color origColor = HPtext.color;
        Vector3 origPos = trans.position;
        for(float currTime = 0f; currTime < fadeIn_animTime; currTime += Time.deltaTime) {
            float ratio = currTime / fadeIn_animTime;
            HPtext.color = new Color(origColor.r, origColor.g, origColor.b, (ratio * ratio));
            trans.position = origPos + (ratio * ratio) * fadeIn_moveDist;
            yield return null;
        }
        origPos = trans.position;
        for (float currTime = 0f; currTime < fadeOut_animTime; currTime += Time.deltaTime) {
            float ratio = currTime / fadeOut_animTime;
            HPtext.color = new Color(origColor.r, origColor.g, origColor.b, 1f - ratio);
            trans.position = origPos + ratio * fadeOut_moveDist;
            yield return null;
        }
        Destroy(gameObject);
    }
}
