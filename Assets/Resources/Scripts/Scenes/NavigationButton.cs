using UnityEngine;

public class NavigationButton : MonoBehaviour {
    public void goToScene(string sceneName) {
        LoadingScreen.Instance.Show(Scenes.LoadAsync(sceneName));
    }
    public void goToMain(bool loadFile) {
        GameController.loadSaveFile = loadFile;
        goToScene("Main");
    }
}

//TO-DO: DISABLE CONTINUE BUTTON IF NO SAVE EXISTS
//TO-DO: MOVE ALL CLASS SERIALIZABLE STUFF IN ONE CLASS WOO
//TO-DO: CLEAN UP CODE