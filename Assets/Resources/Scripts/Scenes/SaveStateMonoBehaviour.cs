using UnityEngine;

public class SaveStateMonoBehaviour : MonoBehaviour {
    public static SaveStateMonoBehaviour Instance;
    [HideInInspector] public SaveState SaveInstance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        if(!PlayerPrefs.HasKey("SaveExists")) PlayerPrefs.SetInt("SaveExists", 0);
    }
}
