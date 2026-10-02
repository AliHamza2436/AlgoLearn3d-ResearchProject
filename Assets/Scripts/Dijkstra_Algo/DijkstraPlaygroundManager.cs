using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DijkstraPlaygroundManager : MonoBehaviour
{
    public FloorNode startNode;
    public FloorNode destinationNode;
    public List<FloorNode> allNodes = new List<FloorNode>();

    public Animation exitDoor;

    public TextMeshProUGUI instructionBanner;

    private FloorNode currentNode;

    public RoomTransitionTrigger ref_roomTransitionTrigger;

    private void OnEnable()
    {
        FloorNode.OnPlayerSteppedOnNode += HandleNodeSelection;
    }

    private void OnDisable()
    {
        FloorNode.OnPlayerSteppedOnNode -= HandleNodeSelection;
    }

    public void InitializePuzzle()
    {
        foreach (var node in allNodes)
        {
            node.isVisited = false;
            node.currentDistance = int.MaxValue;
            node.UpdateDistanceDisplay();
        }

        if (startNode != null)
        {
            startNode.UpdateDistance(0);
            startNode.SetReachableHighlight(true);
        }

        SetInstruction("Welcome! Your start node is S and your destination is D. Step onto node S (0) to get things started!");
    }

    private void HandleNodeSelection(FloorNode steppedNode)
    {
        AudioManager.Instance.PlayWallFrameClick();
        if (steppedNode.isVisited)
        {
            SetInstruction("You've already visited node " + steppedNode.nodeName + "! Take a look around and pick an unvisited node.");
            return;
        }

        FloorNode lowestUnvisited = GetCheapestUnvisitedNode();

        if (steppedNode != lowestUnvisited && lowestUnvisited != null)
        {
            SetInstruction($"<color=#E74C3C>Not quite!</color> Node {steppedNode.nodeName} doesn't have the smallest tentative distance right now. Check your options and try another node.");
            return;
        }

        currentNode = steppedNode;
        currentNode.SetVisitedVisual();

        int shortcutsFound = RelaxEdgesFrom(currentNode);

        if (destinationNode != null && destinationNode.isVisited)
        {
            SetInstruction("<color=green>Fantastic job! You reached destination D with a total distance of " + destinationNode.currentDistance + ". The exit is open!</color>");
            if (exitDoor != null) exitDoor.Play();
            return;
        }

        FloorNode nextTarget = GetCheapestUnvisitedNode();
        if (nextTarget != null)
        {
            if (shortcutsFound > 0)
            {
                SetInstruction("<color=green>Great choice!</color> You updated some neighbor distances. Current path cost to " + currentNode.nodeName + " is <b>" + currentNode.currentDistance + "</b>. What's your next move?");
            }
            else
            {
                SetInstruction("<color=green>Nice!</color> Node " + currentNode.nodeName + " is locked in with a distance of <b>" + currentNode.currentDistance + "</b>. Look at the remaining unvisited nodes and pick the cheapest one.");
            }
        }
    }

    private int RelaxEdgesFrom(FloorNode node)
    {
        int shortcuts = 0;
        foreach (var edge in node.connectedEdges)
        {
            if (edge.targetNode != null && !edge.targetNode.isVisited)
            {
                int altDistance = node.currentDistance + edge.weight;
                if (altDistance < edge.targetNode.currentDistance)
                {
                    edge.targetNode.UpdateDistance(altDistance);
                    shortcuts++;
                }
            }
        }
        return shortcuts;
    }

    private FloorNode GetCheapestUnvisitedNode()
    {
        FloorNode best = null;
        int minCost = int.MaxValue;

        foreach (var node in allNodes)
        {
            if (!node.isVisited && node.currentDistance < minCost)
            {
                minCost = node.currentDistance;
                best = node;
            }
        }
        return best;
    }

    private void SetInstruction(string message)
    {
        if (instructionBanner != null)
        {
            instructionBanner.text = message;
        }
    }
}