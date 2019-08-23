using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/Enemy";
    public int number;
    private int currHealth;
    private int maxHealth;
    private int damage;
    //health bar obj

    private SpriteRenderer spr;
    private Transform trans;

    public static Enemy Create(string name, Vector3 spawnPos, int num, int health, int dmg) {
        Enemy e = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<Enemy>();
        e.setInitValues(num, health, dmg);
        return e;
    }
    public void setInitValues(int num, int health, int dmg) {
        StopAllCoroutines();
        number = num;
        maxHealth = health;
        currHealth = maxHealth;
        damage = dmg;
    }

    void Awake() {
        trans = transform;
        spr = GetComponent<SpriteRenderer>();
    }
    void Start() {
        
    }

    void Update() {
        
    }
    public void addToHealth(int value) {
        currHealth += value;
        //take damage && heal animation
    }

    public virtual IEnumerator Attack(Player p, Board b) {
        p.addToHealth(-damage);
        yield return null;
    }

    public bool isAlive() {
        return currHealth > 0;
    }

    //spawn animation
    //attack animation
    //hurt animation
    //special animation
    //heal animation
}
