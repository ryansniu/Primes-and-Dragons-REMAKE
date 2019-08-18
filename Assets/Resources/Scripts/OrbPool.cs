using System.Collections.Generic;
using UnityEngine;

public class OrbPool : MonoBehaviour {
    public static OrbPool SharedInstance;
    private volatile List<GameObject> pooledOrbs;

    void Awake() {
        SharedInstance = this;
        pooledOrbs = new List<GameObject>();
    }

    public GameObject GetPooledOrb(Vector3 pos, ORB_VALUE val) {
        foreach (GameObject o in pooledOrbs) {
            if (!o.activeInHierarchy) {
                Orb orb = isOrb(o);
                orb.setInitValues(pos, val);
                o.SetActive(true);
                return o;
            }
        }

        GameObject obj = Orb.Create(pos, val).gameObject;
        pooledOrbs.Add(obj);
        return obj;
    }

    public void ReturnToPool(GameObject obj) {
        if (isOrb(obj) != null) obj.SetActive(false);
    }

    public void ReturnAllOrbsToPool() {
        foreach (GameObject o in pooledOrbs) {
            ReturnToPool(o);
        }
    }

    public Orb isOrb(GameObject obj) {
        Orb orb = obj.GetComponent<Orb>();
        if (orb == null) throw new System.Exception("Object is NOT an Orb!");
        return orb;
    }
}