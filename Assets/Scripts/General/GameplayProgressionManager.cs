using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayProgressionManager : MonoBehaviour
{
    public static GameplayProgressionManager Instance { get; private set; }

    public GameObject[] ObjectToEnableForDijkstraAlgo;
    public GameObject[] ObjectToEnableForASearchAlgo;
    public int requiredFrameCount = 14;
    public GameObject galleryBlockingWall;
    public GameObject quizAreaGate;
    public GameObject practiceZoneGate;
    public TextMeshProUGUI objectiveTrackerText;
    private readonly HashSet<int> visitedFrameIDs = new HashSet<int>();
    private bool isGalleryComplete = false;
    private bool isQuizComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        string selectedMode = PlayerPrefs.GetString("SelectedAlgorithmMode", "Dijkstra");
        if (selectedMode == "Dijkstra")
        {
            foreach (GameObject obj in ObjectToEnableForDijkstraAlgo)
                obj.SetActive(true);
            foreach (GameObject obj in ObjectToEnableForASearchAlgo)
                obj.SetActive(false);
        }
        else if (selectedMode == "A Search")
        {
            foreach (GameObject obj in ObjectToEnableForASearchAlgo)
                obj.SetActive(true);
            foreach (GameObject obj in ObjectToEnableForDijkstraAlgo)
                obj.SetActive(false);
        }
    }
    private void Start()
    {
        UpdateObjectiveDisplay();

        if (galleryBlockingWall != null) galleryBlockingWall.SetActive(true);
        if (quizAreaGate != null) quizAreaGate.SetActive(true);
        if (practiceZoneGate != null) practiceZoneGate.SetActive(true);
    }

    public void RegisterFrameInspection(int frameId)
    {
        if (isGalleryComplete) return;

        if (visitedFrameIDs.Contains(frameId)) return;

        visitedFrameIDs.Add(frameId);
        UpdateObjectiveDisplay();

        if (visitedFrameIDs.Count >= requiredFrameCount)
        {
            UnlockGalleryPassage();
        }
    }

    private void UnlockGalleryPassage()
    {
        isGalleryComplete = true;
        if (galleryBlockingWall != null)
        {
            galleryBlockingWall.SetActive(false);
        }
        UpdateObjectiveDisplay();
    }

    public void OnQuizFinished()
    {
        isQuizComplete = true;
        if (practiceZoneGate != null)
        {
            practiceZoneGate.SetActive(false);
        }
        UpdateObjectiveDisplay();
    }

    private void UpdateObjectiveDisplay()
    {
        if (objectiveTrackerText == null) return;

        if (!isGalleryComplete)
        {
            objectiveTrackerText.text = "<b>Objective:</b> Inspect the wall info panels (" + visitedFrameIDs.Count + "/" + requiredFrameCount + ")";
        }
        else if (!isQuizComplete)
        {
            objectiveTrackerText.text = "<b>Objective:</b> Head over to the Quiz Area and test your knowledge!";
        }
        else
        {
            objectiveTrackerText.text = "<b>Objective:</b> Quiz passed! Enter the Practice Zone and solve the graph.";
        }
    }
}