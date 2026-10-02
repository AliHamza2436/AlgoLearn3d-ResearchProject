using UnityEngine;
using TMPro;

public class GraphVisualizer : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.15f;
    public float floorYOffset = 0.05f;
    public float fontSize = 3.5f;
    public Color fontColor = Color.yellow;
    public float labelYOffset = 0.4f;

    private void Start()
    {
        DrawAllConnections();
    }

    private void DrawAllConnections()
    {
        FloorNode[] allNodes = FindObjectsByType<FloorNode>(FindObjectsSortMode.None);

        foreach (var node in allNodes)
        {
            foreach (var edge in node.connectedEdges)
            {
                if (edge.targetNode != null)
                {
                    CreateVisualEdge(node, edge.targetNode, edge.weight);
                }
            }
        }
    }

    private void CreateVisualEdge(FloorNode from, FloorNode to, int weight)
    {
        Vector3 startPos = from.transform.position;
        startPos.y += floorYOffset;

        Vector3 endPos = to.transform.position;
        endPos.y += floorYOffset;

        GameObject lineObj = new GameObject("Edge_" + from.nodeName + "_to_" + to.nodeName);
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Vector3 midpoint = (startPos + endPos) * 0.5f;
        midpoint.y += labelYOffset;

        GameObject textObj = new GameObject("Weight_" + weight);
        textObj.transform.SetParent(lineObj.transform);
        textObj.transform.position = midpoint;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = "[" + weight + "]";
        tmp.fontSize = fontSize;
        tmp.color = fontColor;
        tmp.alignment = TextAlignmentOptions.Center;
        textObj.AddComponent<FaceCamera>();
    }
}