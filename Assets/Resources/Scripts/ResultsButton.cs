using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsButton : MonoBehaviour {
    public void returnToTitle() {
        LoadingScreen.Instance.Show(Scenes.LoadAsync("Title"));
    }
    public void tryAgain() {
        LoadingScreen.Instance.Show(Scenes.LoadAsync("Main"));
    }
}
