using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour {
    public static readonly float MAX_ANIM_TIME = 1f;
    public static readonly int ANIM_SPEED = 100;

    [SerializeField] private TextMeshProUGUI HPNum = default;
    [SerializeField] private Slider HPBar = default;

    public void setHPNumColor(Color c) => HPNum.color = c;
    public void displayHP(int currHealth, int maxHealth){
        HPNum.text = currHealth + "/" + maxHealth;
        HPBar.value = (float)Mathf.Clamp(currHealth, 0, maxHealth) / maxHealth;
    }
}
