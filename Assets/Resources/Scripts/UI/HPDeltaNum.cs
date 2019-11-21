using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPDeltaNum : MonoBehaviour{
    private const string PREFAB_PATH = "Prefabs/HPDeltaNum";
    public TextMeshPro HPtext;
    private Transform trans;

    public static HPDeltaNum Create(Vector3 spawnPos, int value, float fontSize){
        HPDeltaNum hpdn = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<HPDeltaNum>();
        hpdn.initValues(value, fontSize);
        return hpdn;
    }
    void Awake() {
        trans = transform;
    }

    private void initValues(int value, float fontSize){
        HPtext.fontSize = fontSize;
        if (value > 0){
            HPtext.text = "+" + value.ToString();
            HPtext.color = Color.green;
        }
        else if (value < 0){
            HPtext.text = value.ToString();
            HPtext.color = Color.red;
        }
        else{
            HPtext.text = "=" + value.ToString();
            HPtext.color = Color.gray;
        }
        StartCoroutine(animate());
    }
    private IEnumerator animate(){
        float fadeIn_animTime = 0.2f;
        Vector3 fadeIn_moveDist = new Vector3(0f, 0.08f, 0f);
        float fadeOut_animTime = 0.4f;
        Vector3 fadeOut_moveDist = new Vector3(0f, 0.1f, 0f);

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
