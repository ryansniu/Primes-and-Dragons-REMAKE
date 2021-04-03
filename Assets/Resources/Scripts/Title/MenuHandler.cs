using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour {
    [SerializeField] private NavigationButton nav = default;
    [SerializeField] private Slider musSlider = default, sfxSlider = default, HPSlider = default, ATKSlider = default;
    [SerializeField] private Button continueButton = default, resetScores = default, deleteSave = default;
    [SerializeField] private Toggle screenShake = default, autoSave = default, endlessMode = default;
    private bool isNewGame = true;

    void Start() {
        //musSlider.onValueChanged.AddListener(delegate { AudioController.Instance.setMusicVolume(musSlider.value); });
        musSlider.value = PlayerPrefs.GetFloat("musVol", 1f);
        //sfxSlider.onValueChanged.AddListener(delegate { AudioController.Instance.setSFXVolume(sfxSlider.value); });
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 1f);
        HPSlider.value = PlayerPrefs.GetFloat("EnemyHP", 1f);
        ATKSlider.value = PlayerPrefs.GetFloat("EnemyATK", 1f);

        continueButton.interactable = PlayerPrefs.GetInt("SaveExists", 0) == 1;
        deleteSave.interactable = PlayerPrefs.GetInt("SaveExists", 0) == 1;
        resetScores.interactable = PlayerPrefs.GetInt("ScoresExist", 0) == 1;

        screenShake.isOn = PlayerPrefs.GetInt("ScreenShake", 1) == 1;
        autoSave.isOn = PlayerPrefs.GetInt("AutoSave", 1) == 1;
        endlessMode.isOn = PlayerPrefs.GetInt("EndlessMode", 0) == 1;
    }

    public void OnNewOrContinue(bool newGame) {
        isNewGame = newGame;
        if(isNewGame) Debug.Log("new game");
        else Debug.Log("continue");
    }
    public void OnOptionSelect() {
        Debug.Log("option");
    }
    public void OnGuideSelect() {
        Debug.Log("guide");
    }
    public void OnExitSelect() {
        Debug.Log("exit");
    }
    public void OnStartSelect() {
        nav.goToMain(!isNewGame);
        Debug.Log("start");
    }


    public void toggleScreenShake(bool isOn) {
        PlayerPrefs.SetInt("ScreenShake", isOn ? 1 : 0);
        Debug.Log("screen shake " + isOn);
    }
    public void toggleAutoSave(bool isOn) {
        PlayerPrefs.SetInt("AutoSave", isOn ? 1 : 0);
        Debug.Log("auto save " + isOn);
    }
    public void toggleEndlessMode(bool isOn) {
        PlayerPrefs.SetInt("EndlessMode", isOn ? 1 : 0);
        Debug.Log("endless mode " + isOn);
    }
    public void OnResetScoresSelect() {
        resetScores.interactable = false;
        PlayerPrefs.SetInt("ScoresExist", 0);
        GameData.deleteFile("/lData.binary");
        Debug.Log("reset scores");
    }
    public void OnDeleteSave() {
        deleteSave.interactable = false;
        continueButton.interactable = false;
        PlayerPrefs.SetInt("SaveExists", 0);
        GameData.deleteFile("/sData.binary");
        Debug.Log("delete save");
    }

    public void setEnemyHP(float newHP)
    {
        PlayerPrefs.SetFloat("EnemyHP", newHP);
        Debug.Log("set hp " + newHP);
    }
    public void setEnemyATK(float newATK)
    {
        PlayerPrefs.SetFloat("EnemyATK", newATK);
        Debug.Log("set atk " + newATK);
    }
}
