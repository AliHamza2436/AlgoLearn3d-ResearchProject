using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManagerUI : MonoBehaviour
{
    public static QuizManagerUI Instance { get; private set; }

    public int requiredCorrectAnswers = 6;

    public GameObject quizPanel;
    public GameObject optionsContainer;
    public Slider progressSlider;
    public TextMeshProUGUI questionNumberText;
    public TextMeshProUGUI questionContentText;
    public Button[] optionButtons = new Button[4];
    public TextMeshProUGUI[] optionTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI feedbackStatusText;
    
    public Sprite defaultButtonSprite;
    public Sprite correctButtonSprite;
    public Sprite wrongButtonSprite;

    public InteractiveWallFrame[] QuizInteractiveWallFrames;

    private QuizQuestionData activeQuestion;
    private bool isAnsweringBlocked = false;

    private readonly HashSet<QuizQuestionData> correctlyAnsweredQuestions = new HashSet<QuizQuestionData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        if (quizPanel != null)
            quizPanel.SetActive(false);
    }

    private void Start()
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    public bool IsQuestionSolved(QuizQuestionData question)
    {
        return question != null && correctlyAnsweredQuestions.Contains(question);
    }

    public void OpenSingleQuestionQuiz(QuizQuestionData question)
    {
        if (question == null) return;

        activeQuestion = question;
        isAnsweringBlocked = false;

        if (quizPanel != null) quizPanel.SetActive(true);
        if (optionsContainer != null) optionsContainer.SetActive(true);

        SetFeedbackAsButton(false);

        int solvedCount = correctlyAnsweredQuestions.Count;
        int displayIndex = Mathf.Clamp(solvedCount + 1, 1, requiredCorrectAnswers);

        if (questionNumberText != null)
        {
            questionNumberText.text = "Question " + displayIndex + " of " + requiredCorrectAnswers;
        }

        if (progressSlider != null)
        {
            progressSlider.value = Mathf.Clamp01((float)solvedCount / requiredCorrectAnswers);
        }

        if (questionContentText != null)
            questionContentText.text = activeQuestion.questionText;

        if (feedbackStatusText != null)
            feedbackStatusText.text = "";

        ResetOptionButtons();
    }

    private void ResetOptionButtons()
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].gameObject.SetActive(true);
            SetButtonSprite(optionButtons[i], defaultButtonSprite);
            optionButtons[i].interactable = true;

            if (i < activeQuestion.options.Length && optionTexts[i] != null)
            {
                optionTexts[i].text = activeQuestion.options[i];
            }
        }
    }

    private void OnOptionSelected(int selectedIndex)
    {
        if (isAnsweringBlocked || activeQuestion == null) return;

        AudioManager.Instance.PlayButtonClick();
        bool isCorrect = (selectedIndex == activeQuestion.correctOptionIndex);

        if (isCorrect)
        {
            isAnsweringBlocked = true;
            SetButtonSprite(optionButtons[selectedIndex], correctButtonSprite);

            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].interactable = false;
            }

            if (!correctlyAnsweredQuestions.Contains(activeQuestion))
            {
                correctlyAnsweredQuestions.Add(activeQuestion);
            }

            int solvedCount = correctlyAnsweredQuestions.Count;

            if (progressSlider != null)
            {
                progressSlider.value = Mathf.Clamp01((float)solvedCount / requiredCorrectAnswers);
            }

            if (solvedCount >= requiredCorrectAnswers)
            {
                if (feedbackStatusText != null)
                {
                    feedbackStatusText.text = "<color=green>All quizzes solved! The wall has opened!</color>";
                }
                GameplayProgressionManager.Instance.OnQuizFinished();
            }
            else
            {
                if (feedbackStatusText != null)
                {
                    feedbackStatusText.text = "<color=green>Correct! (" + solvedCount + "/" + requiredCorrectAnswers + " solved)</color>";
                }
            }


            foreach(InteractiveWallFrame obj in QuizInteractiveWallFrames)
            obj.UpdateQuizStatusText();

        }
        else
        {
            isAnsweringBlocked = true;
            SetButtonSprite(optionButtons[selectedIndex], wrongButtonSprite);

            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].interactable = false;
            }

            if (feedbackStatusText != null)
            {
                feedbackStatusText.text = "<color=red><u>WRONG! Tap here to try again.</u></color>";
                SetFeedbackAsButton(true);
            }
        }
    }

    private void SetButtonSprite(Button btn, Sprite targetSprite)
    {
        if (btn != null && targetSprite != null)
        {
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.sprite = targetSprite;
            }
        }
    }

    private void SetFeedbackAsButton(bool enabled)
    {
        if (feedbackStatusText == null) return;

        Button textButton = feedbackStatusText.GetComponent<Button>();
        if (textButton == null)
        {
            textButton = feedbackStatusText.gameObject.AddComponent<Button>();
        }

        textButton.enabled = enabled;
        textButton.onClick.RemoveAllListeners();
        if (enabled)
        {
            textButton.onClick.AddListener(ResetQuestionAttempt);
        }
    }

    private void ResetQuestionAttempt()
    {
        isAnsweringBlocked = false;
        SetFeedbackAsButton(false);

        if (feedbackStatusText != null)
            feedbackStatusText.text = "";

        ResetOptionButtons();
    }



    public void CloseQuizPanel()
    {
        AudioManager.Instance.PlayButtonClick();
        if (quizPanel != null)
            quizPanel.SetActive(false);
    }
}