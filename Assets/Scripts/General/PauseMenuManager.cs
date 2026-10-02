
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public string homeSceneName = "MainMenuScene";
    public GameObject loadingPanel;
    public GameObject settingPanel;


    public void PauseGame()
    {
        AudioManager.Instance.PlayButtonClick();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlayButtonClick();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        AudioManager.Instance.PlayButtonClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToHome()
    {
        AudioManager.Instance.PlayButtonClick();
        Time.timeScale = 1f;
        if (loadingPanel != null) loadingPanel.SetActive(true);
        SceneManager.LoadScene(homeSceneName);
    }

    public void OpenGameSettings()
    {
        AudioManager.Instance.PlayButtonClick();
        if (settingPanel != null) settingPanel.SetActive(true);
    }
    public void CloseSettingPanel()
    {
        AudioManager.Instance.PlayButtonClick();
        if (settingPanel != null) settingPanel.SetActive(false);
    }

}