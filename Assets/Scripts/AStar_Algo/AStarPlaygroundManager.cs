using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

public class AStarPlaygroundManager : MonoBehaviour
{
    [System.Serializable]
    public struct NodeUITextMapping
    {
        public AStarFloorNode targetNode;
        public TextMeshProUGUI gCostTextDisplay;
    }

    public List<NodeUITextMapping> nodeUIMappings = new List<NodeUITextMapping>();
    public AStarFloorNode startNode;
    public AStarFloorNode targetNode;
    public List<AStarFloorNode> allNodes = new List<AStarFloorNode>();
    public Animation exitDoor;
    public TextMeshProUGUI instructionBanner;
    public TextMeshProUGUI staticHeuristicPanel;

    private AStarFloorNode currentNode;

    private void OnEnable()
    {
        AStarFloorNode.OnPlayerSteppedOnAStarNode += HandleNodeInteraction;
    }

    private void OnDisable()
    {
        AStarFloorNode.OnPlayerSteppedOnAStarNode -= HandleNodeInteraction;
    }

    public void InitializeAStarRoom()
    {
        if (targetNode == null || startNode == null) return;

        foreach (var node in allNodes)
        {
            node.isClosed = false;
            node.gCost = int.MaxValue;
        }

        currentNode = null;
        UpdateStaticHeuristicPanel();

        SetInstruction("Welcome to the Graph! Step on the START node (S) to begin.");
    }

    private void HandleNodeInteraction(AStarFloorNode steppedNode)
    {
        if (currentNode == null)
        {
            if (steppedNode == startNode)
            {
                startNode.SetGCost(0);
                currentNode = startNode;
                startNode.MarkAsClosed();

                EvaluateNeighbors(currentNode);
                UpdateStaticHeuristicPanel();

                string optionsText = GetNeighborOptionsString(currentNode);
                SetInstruction("You are at <b>START (S)</b>.\n" + optionsText);
                return;
            }
            else
            {
                SetInstruction("Please start by stepping on the <b>START (S)</b> node!");
                return;
            }
        }

        if (steppedNode.isClosed && steppedNode != currentNode)
        {
            SetInstruction("Node " + steppedNode.nodeName + " is already visited! Choose an active neighbor.");
            return;
        }

        if (steppedNode == currentNode) return;

        AStarFloorNode optimalChoice = GetLowestFCostFromCurrentNeighbors();
        string validationMsg = "";

        if (optimalChoice != null && steppedNode == optimalChoice)
        {
            validationMsg = "<color=green><b>Correct Choice!</b></color> Node " + steppedNode.nodeName + " has the lowest f-cost (" + steppedNode.FCost + ").\n";
        }
        else if (optimalChoice != null)
        {
            validationMsg = "<color=red><b>Suboptimal Choice!</b></color> You picked Node " + steppedNode.nodeName + ", but A* prefers Node " + optimalChoice.nodeName + ".\n";
        }

        currentNode = steppedNode;
        currentNode.MarkAsClosed();

        if (currentNode == targetNode)
        {
            SetInstruction("<color=green><b>Goal Reached!</b></color> Final optimal path cost: " + targetNode.gCost + ". Gate unlocked!");
            if (exitDoor != null) exitDoor.Play();
            return;
        }

        EvaluateNeighbors(currentNode);
        UpdateStaticHeuristicPanel();

        string nextOptions = GetNeighborOptionsString(currentNode);
        SetInstruction(validationMsg + nextOptions);
    }

    private void EvaluateNeighbors(AStarFloorNode node)
    {
        if (node.gCost == int.MaxValue) return;

        foreach (var edge in node.connectedEdges)
        {
            if (edge.targetNode != null && !edge.targetNode.isClosed)
            {
                int tentativeG = node.gCost + edge.weight;
                if (tentativeG < edge.targetNode.gCost)
                {
                    edge.targetNode.SetGCost(tentativeG);
                }
            }
        }
    }

    private string GetNeighborOptionsString(AStarFloorNode fromNode)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>Available Moves:</b>");

        var sortedEdges = fromNode.connectedEdges
            .Where(edge => edge.targetNode != null && !edge.targetNode.isClosed)
            .OrderBy(edge => edge.targetNode.nodeName);

        foreach (var edge in sortedEdges)
        {
            int tentativeG = fromNode.gCost + edge.weight;
            int fVal = tentativeG + edge.targetNode.hCost;

            sb.AppendLine("• Node " + edge.targetNode.nodeName + " -> f: " + fVal + " (g:" + tentativeG + " + h:" + edge.targetNode.hCost + ")");
        }

        return sb.ToString().TrimEnd();
    }

    private AStarFloorNode GetLowestFCostFromCurrentNeighbors()
    {
        AStarFloorNode bestNode = null;
        int lowestF = int.MaxValue;

        foreach (var edge in currentNode.connectedEdges)
        {
            if (edge.targetNode != null && !edge.targetNode.isClosed)
            {
                int tentativeG = currentNode.gCost + edge.weight;
                int fVal = tentativeG + edge.targetNode.hCost;
                if (fVal < lowestF)
                {
                    lowestF = fVal;
                    bestNode = edge.targetNode;
                }
            }
        }
        return bestNode;
    }

    private void UpdateStaticHeuristicPanel()
    {
        foreach (var mapping in nodeUIMappings)
        {
            if (mapping.targetNode != null && mapping.gCostTextDisplay != null)
            {
                string gText = (mapping.targetNode.gCost == int.MaxValue) ? "∞" : mapping.targetNode.gCost.ToString();
                mapping.gCostTextDisplay.text = gText;
            }
        }
    }

    private void SetInstruction(string text)
    {
        if (instructionBanner != null)
        {
            instructionBanner.text = text.Trim();
        }
    }
}