
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject modeSelectionPanel;
    public GameObject settingPanel;

    void Start()
    {
        ShowMainMenu();
    }

    public void OnPlayClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (modeSelectionPanel != null) modeSelectionPanel.SetActive(true);
    }

    public void OnBackClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        ShowMainMenu();
    }

    public void OnSettingClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(true);
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (modeSelectionPanel != null) modeSelectionPanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    public void OnExitClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}