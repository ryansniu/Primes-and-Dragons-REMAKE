using System.Collections.Generic;
using UnityEngine;

public class OrbPool : MonoBehaviour {
    public static OrbPool SharedInstance;
    private volatile List<GameObject> pooledOrbs;

    void Awake() {
        SharedInstance = this;
        pooledOrbs = new List<GameObject>();
    }

    public GameObject GetPooledOrb(Vector3 spawnPos, int fallDist, ORB_VALUE val) {
        foreach (GameObject o in pooledOrbs) {
            if (!o.activeInHierarchy) {
                Orb orb = isOrb(o);
                o.SetActive(true);
                orb.setInitValues(spawnPos, fallDist, val);
                return o;
            }
        }

        GameObject obj = Orb.Create(spawnPos, fallDist, val).gameObject;
        pooledOrbs.Add(obj);
        return obj;
    }

    public void ReturnToPool(GameObject obj) { if (isOrb(obj) != null) obj.SetActive(false); }
    public void ReturnAllOrbsToPool() { foreach (GameObject o in pooledOrbs) ReturnToPool(o); }

    public Orb isOrb(GameObject obj) {
        Orb orb = obj.GetComponent<Orb>();
        if (orb == null) throw new System.Exception("Object is NOT an Orb!");
        return orb;
    }
}