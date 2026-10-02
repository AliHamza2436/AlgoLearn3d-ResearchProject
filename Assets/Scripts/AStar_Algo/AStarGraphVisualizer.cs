using UnityEngine;
using TMPro;

public class AStarGraphVisualizer : MonoBehaviour
{
    public Material edgeMaterial;
    public Color fontColor;
    public float fontSize;
    public float lineWidth = 0.15f;
    public float groundYOffset = 0.05f;

    private void Start()
    {
        DrawGraph();
    }

    private void DrawGraph()
    {
        AStarFloorNode[] nodes = FindObjectsByType<AStarFloorNode>(FindObjectsSortMode.None);

        foreach (var node in nodes)
        {
            foreach (var edge in node.connectedEdges)
            {
                if (edge.targetNode != null)
                {
                    CreateVisualLineWithChevron(node, edge.targetNode, edge.weight);
                }
            }
        }
    }

    private void CreateVisualLineWithChevron(AStarFloorNode from, AStarFloorNode to, int weight)
    {
        Vector3 startPos = from.transform.position + Vector3.up * groundYOffset;
        Vector3 endPos = to.transform.position + Vector3.up * groundYOffset;

        GameObject lineObj = new GameObject("Edge_" + from.nodeName + "_to_" + to.nodeName);
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        Material activeMat = edgeMaterial != null ? edgeMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.material = activeMat;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Color lineColor = activeMat.HasProperty("_Color") ? activeMat.color : new Color(0f, 0.9f, 1f, 1f);

        Vector3 midpoint = (startPos + endPos) * 0.5f;

        GameObject arrowContainer = new GameObject($"Arrow_{from.nodeName}_to_{to.nodeName}");
        arrowContainer.transform.SetParent(lineObj.transform);
        arrowContainer.transform.position = midpoint + Vector3.up * 0.03f;
        arrowContainer.transform.LookAt(endPos);

        GameObject wingLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wingLeft.transform.SetParent(arrowContainer.transform);
        wingLeft.transform.localPosition = new Vector3(-0.2f, 0f, -0.3f);
        wingLeft.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
        wingLeft.transform.localScale = new Vector3(0.1f, 0.03f, 0.6f);

        GameObject wingRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wingRight.transform.SetParent(arrowContainer.transform);
        wingRight.transform.localPosition = new Vector3(0.2f, 0f, -0.3f);
        wingRight.transform.localRotation = Quaternion.Euler(0f, -45f, 0f);
        wingRight.transform.localScale = new Vector3(0.1f, 0.03f, 0.6f);

        Material arrowMat = new Material(Shader.Find("Sprites/Default"));
        arrowMat.color = lineColor;

        foreach (Transform child in arrowContainer.transform)
        {
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) mr.material = arrowMat;

            Collider col = child.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }

        Vector3 textPos = midpoint + Vector3.up * 0.4f;
        GameObject txtObj = new GameObject("Weight_" + weight);
        txtObj.transform.SetParent(lineObj.transform);
        txtObj.transform.position = textPos;

        TextMeshPro tmp = txtObj.AddComponent<TextMeshPro>();
        tmp.text = "[" + weight + "]";
        tmp.fontSize = fontSize;
        tmp.color = fontColor;
        tmp.alignment = TextAlignmentOptions.Center;

        txtObj.AddComponent<FaceCamera>();
    }
}