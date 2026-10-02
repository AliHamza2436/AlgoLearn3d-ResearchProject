using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneTransitionManager : MonoBehaviour
{
    public string targetSceneName = "GameplayScene";

    public Slider loadingProgressBar;
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        StartCoroutine(TransitionToRoutine(targetSceneName));
    }


    private IEnumerator TransitionToRoutine(string sceneToLoad)
    {
        yield return null;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; 

        float displayProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.unscaledDeltaTime * 2f);

            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = displayProgress;
            }

            if (loadingText != null)
            {
                int percentage = Mathf.RoundToInt(displayProgress * 100f);
                loadingText.text = "Loading...          " + percentage + "%";
            }

            if (operation.progress >= 0.9f && displayProgress >= 0.99f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

}