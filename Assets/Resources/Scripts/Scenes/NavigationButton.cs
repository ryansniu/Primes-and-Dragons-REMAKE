using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour {
    [SerializeField] private Button continueButton = default;

    void Start() { if (continueButton) continueButton.interactable = PlayerPrefs.GetInt("SaveExists") == 1; }
    public void goToScene(string sceneName) => LoadingScreen.Instance.Show(Scenes.LoadAsync(sceneName));
    public void goToMain(bool loadFile) {
        PlayerPrefs.SetInt("LoadFromSaveFile", loadFile ? 1 : 0);
        goToScene("Main");
    }
}

//TO-DO: CLEAN UP CODE