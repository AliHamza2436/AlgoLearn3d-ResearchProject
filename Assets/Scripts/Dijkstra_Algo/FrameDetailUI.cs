using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrameDetailUI : MonoBehaviour
{
    public static FrameDetailUI Instance { get; private set; }

    public GameObject framePanel;
    public Image enlargedImageDisplay; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        if (framePanel != null)
            framePanel.SetActive(false);
    }

    public void OpenDetailFramePanel(AlgorithmFrameData data)
    {
        if (enlargedImageDisplay != null && data != null)
        {
            enlargedImageDisplay.sprite = data.frameSprite;
            enlargedImageDisplay.gameObject.SetActive(enlargedImageDisplay.sprite != null);
        }

        if (framePanel != null)
            framePanel.SetActive(true);
    }

    public void CloseFramePanel()
    {
        AudioManager.Instance.PlayButtonClick();
        if (framePanel != null)
            framePanel.SetActive(false);
    }
}