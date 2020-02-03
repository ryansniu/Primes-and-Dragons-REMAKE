using UnityEngine;

public class SaveStateMonoBehaviour : MonoBehaviour {
    public static SaveStateMonoBehaviour Instance;

    public SaveState SaveInstance;
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        SaveInstance.init();
    }
}
