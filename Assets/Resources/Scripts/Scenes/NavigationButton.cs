using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour {
    public Button continueButton;

    void Start() {
        if(continueButton) continueButton.interactable = SaveStateMonoBehaviour.Instance.SaveInstance.doesSaveExist();
    }
    public void goToScene(string sceneName) {
        LoadingScreen.Instance.Show(Scenes.LoadAsync(sceneName));
    }
    public void goToMain(bool loadFile) {
        GameController.loadSaveFile = loadFile;
        goToScene("Main");
    }
}

//TO-DO: CLEAN UP CODE