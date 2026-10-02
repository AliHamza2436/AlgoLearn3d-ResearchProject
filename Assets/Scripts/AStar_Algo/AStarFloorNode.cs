using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class AStarEdge
{
    public AStarFloorNode targetNode;
    public int weight;
}

public class AStarFloorNode : MonoBehaviour
{
    public string nodeName = "A";
    public List<AStarEdge> connectedEdges = new List<AStarEdge>();

    public MeshRenderer padRenderer;
    public TextMeshPro worldLabel;
    public Material defaultMat;
    public Material openSetMat;
    public Material closedSetMat;

    [HideInInspector] public int gCost = int.MaxValue;
    public int hCost = 0;
    public int FCost => (gCost == int.MaxValue) ? int.MaxValue : gCost + hCost;

    [HideInInspector] public bool isClosed = false;

    public static event Action<AStarFloorNode> OnPlayerSteppedOnAStarNode;

    private void Start()
    {
        SetOpenSetHighlight(false);
        UpdateWorldDisplay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerSteppedOnAStarNode?.Invoke(this);
        }
    }


    public void SetGCost(int newG)
    {
        gCost = newG;
        UpdateWorldDisplay();
    }

    public void UpdateWorldDisplay()
    {
        if (worldLabel == null) return;

        if (gCost == int.MaxValue)
        {
            worldLabel.text = "<b>" + nodeName + "</b>\ng: ∞";
        }
        else
        {
            worldLabel.text = "<b>" + nodeName + "</b>\ng: " + gCost;
        }
    }

    public void SetOpenSetHighlight(bool active)
    {
        if (isClosed) return;
        if (padRenderer != null)
        {
            padRenderer.material = active ? openSetMat : defaultMat;
        }
    }

    public void MarkAsClosed()
    {
        isClosed = true;
        if (padRenderer != null)
        {
            padRenderer.material = closedSetMat;
        }
    }
}