using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public struct AlgorithmButtonMapping
    {
        public string modeName;      
        public Button modeButton;    
    }

    public List<AlgorithmButtonMapping> algorithmButtons = new List<AlgorithmButtonMapping>();

    public Button continueButton;

    public Color normalColor;
    public Color selectedColor;
    public GameObject loadingPanel;


    private string selectedMode = "";

    void Start()
    {
        ResetAllButtonColors();

        if (continueButton != null) continueButton.interactable = false;

        foreach (var algoBtn in algorithmButtons)
        {
            if (algoBtn.modeButton != null)
            {
                string modeCopy = algoBtn.modeName;
                Button btnCopy = algoBtn.modeButton;

                btnCopy.onClick.AddListener(() => SelectMode(modeCopy, btnCopy));
            }
        }
    }

    public void SelectMode(string modeName, Button clickedButton)
    {
        AudioManager.Instance.PlayButtonClick();
        selectedMode = modeName;
        ResetAllButtonColors();

        SetButtonColor(clickedButton, selectedColor);

        if (continueButton != null)
        {
            continueButton.interactable = true;
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ConfirmAndLoadGame);
        }
    }

    private void ResetAllButtonColors()
    {
        foreach (var mapping in algorithmButtons)
        {
            SetButtonColor(mapping.modeButton, normalColor);
        }
    }

    private void SetButtonColor(Button btn, Color col)
    {
        if (btn == null) return;
        var colors = btn.colors;
        colors.normalColor = col;
        colors.selectedColor = col;
        btn.colors = colors;
    }

    private void ConfirmAndLoadGame()
    {
        AudioManager.Instance.PlayButtonClick();
        PlayerPrefs.SetString("SelectedAlgorithmMode", selectedMode);
        PlayerPrefs.Save();
        loadingPanel.SetActive(true);
    }
}