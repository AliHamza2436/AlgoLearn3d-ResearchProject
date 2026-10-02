
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public enum FrameInteractionType
{
    EducationalContent,
    Quiz
}

public class InteractiveWallFrame : MonoBehaviour
{
    public FrameInteractionType interactionType = FrameInteractionType.EducationalContent;
    public AlgorithmFrameData frameData;
    public QuizQuestionData singleQuizQuestion;

    public TextMeshProUGUI worldTitleText;
    public TextMeshProUGUI worldBodyText;
    public float interactDistance = 5.5f;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        UpdateInWorldDisplay();
    }

    private void UpdateInWorldDisplay()
    {
        if (interactionType == FrameInteractionType.Quiz)
        {
            UpdateQuizStatusText();
        }
    }

    public void UpdateQuizStatusText()
    {
        if (worldBodyText == null || singleQuizQuestion == null) return;

        if (QuizManagerUI.Instance != null && QuizManagerUI.Instance.IsQuestionSolved(singleQuizQuestion))
        {
            worldBodyText.text = "Completed";
        }
        else
        {
            worldBodyText.text = "Tap to answer the quiz question!";
        }
    }


    public void OpenEnlargeFrame()
    {
        AudioManager.Instance.PlayWallFrameClick();
        switch (interactionType)
        {
            case FrameInteractionType.EducationalContent:
                if (FrameDetailUI.Instance != null && frameData != null)
                {
                    FrameDetailUI.Instance.OpenDetailFramePanel(frameData);
                }

                if (GameplayProgressionManager.Instance != null && frameData != null)
                {
                    GameplayProgressionManager.Instance.RegisterFrameInspection(frameData.frameIndex);
                }
                break;

            case FrameInteractionType.Quiz:
                if (QuizManagerUI.Instance != null && singleQuizQuestion != null)
                {
                    QuizManagerUI.Instance.OpenSingleQuestionQuiz(singleQuizQuestion);
                }
                break;
        }
    }
}