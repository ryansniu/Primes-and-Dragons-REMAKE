using System.Collections.Generic;
using UnityEngine;

public class TitleOrbPool : MonoBehaviour {
    public static TitleOrbPool Instance;
    private volatile List<GameObject> pooledOrbs;

    void Awake() {
        Instance = this;
        pooledOrbs = new List<GameObject>();
    }

    public GameObject GetPooledOrb(Vector3 spawnPos, bool fallDir, ORB_VALUE val) {
        foreach (GameObject o in pooledOrbs) {
            if (!o.activeInHierarchy) {
                TitleOrb orb = isTitleOrb(o);
                o.SetActive(true);
                o.transform.SetAsLastSibling();
                orb.setInitValues(spawnPos, fallDir, val);
                return o;
            }
        }

        GameObject obj = TitleOrb.Create(spawnPos, fallDir, val).gameObject;
        pooledOrbs.Add(obj);
        obj.transform.SetAsLastSibling();
        return obj;
    }

    public void ReturnToPool(GameObject obj) { if (isTitleOrb(obj) != null) obj.SetActive(false); }
    public void ReturnAllOrbsToPool() { foreach (GameObject o in pooledOrbs) ReturnToPool(o); }

    public TitleOrb isTitleOrb(GameObject obj) {
        TitleOrb orb = obj.GetComponent<TitleOrb>();
        if (orb == null) throw new System.Exception("Object is NOT a TitleOrb!");
        return orb;
    }
}