using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class GraphEdge
{
    public FloorNode targetNode;
    public int weight;
}

public class FloorNode : MonoBehaviour
{
    public string nodeName = "A";
    public List<GraphEdge> connectedEdges = new List<GraphEdge>();

    public MeshRenderer padRenderer;
    public TextMeshPro distanceLabel;
    public Material defaultMat;
    public Material reachableMat;
    public Material visitedMat;

    [HideInInspector] public int currentDistance = int.MaxValue;
    [HideInInspector] public bool isVisited = false;

    public static event Action<FloorNode> OnPlayerSteppedOnNode;

    private void Start()
    {
        SetReachableHighlight(false);
        UpdateDistanceDisplay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerSteppedOnNode?.Invoke(this);
        }
    }

    public void UpdateDistance(int newDistance)
    {
        currentDistance = newDistance;
        UpdateDistanceDisplay();
    }

    public void UpdateDistanceDisplay()
    {
        if (distanceLabel != null)
        {
            distanceLabel.text = currentDistance == int.MaxValue ? nodeName + "\n[∞]" : nodeName + "\n[" + currentDistance + "]";
        }
    }

    public void SetReachableHighlight(bool active)
    {
        if (isVisited) return;
        if (padRenderer != null)
        {
            padRenderer.material = active ? reachableMat : defaultMat;
        }
    }

    public void SetVisitedVisual()
    {
        isVisited = true;
        if (padRenderer != null)
        {
            padRenderer.material = visitedMat;
        }
    }
}