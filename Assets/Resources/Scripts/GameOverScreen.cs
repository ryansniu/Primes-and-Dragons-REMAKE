using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
    public Image backgImg;
    public GameObject gameOverUI;
    public CanvasGroup gameOverCanvas;
    public IEnumerator gameOverAnimation() {
        gameOverUI.SetActive(true);
        //game over text animation
        yield return new WaitForSeconds(2);
        //fade to next scene
        LoadingScreen.Instance.Show(Scenes.LoadAsync("Results Screen"));
    }
}
