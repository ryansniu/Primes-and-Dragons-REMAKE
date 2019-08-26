using System.Collections;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour {
    private const string PREFAB_PATH = "Prefabs/Enemy";
    public TextMeshPro textNum;
    public HealthBar HPBar;
    public int number;
    private int currHealth;
    private int maxHealth;
    private int damage;

    private SpriteRenderer spr;
    private Transform trans;

    public static Enemy Create(string name, Vector3 spawnPos, int num, int health, int dmg) {
        Enemy e = (Instantiate((GameObject)Resources.Load(PREFAB_PATH), spawnPos, Quaternion.identity) as GameObject).GetComponent<Enemy>();
        e.setInitValues(num, health, dmg);
        return e;
    }
    public void setInitValues(int num, int health, int dmg) {
        number = num;
        textNum.text = number.ToString();
        maxHealth = health;
        currHealth = maxHealth;
        damage = dmg;
    }

    void Awake() {
        trans = transform;
        spr = GetComponent<SpriteRenderer>();
    }
    void Start() {
        HPBar.displayHP(currHealth, maxHealth);
    }
    public void addToHealth(int value) {
        if(value >= 0) HPBar.setHPNumColor(Color.green);
        else if(value == 0) HPBar.setHPNumColor(Color.black);
        else HPBar.setHPNumColor(Color.red);
        currHealth = Mathf.Clamp(currHealth + value, 0, maxHealth);  //adjust health bar bit by bit
        HPBar.displayHP(currHealth, maxHealth);
        HPBar.setHPNumColor(Color.black);
    }

    public virtual IEnumerator Attack(Player p, Board b) {
        p.addToHealth(-damage);
        //board.orbSpawnRates
        //remove or change orbs
        yield return null;
    }

    public void setPosition(Vector2 newPos){
        trans.position = newPos; 
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
