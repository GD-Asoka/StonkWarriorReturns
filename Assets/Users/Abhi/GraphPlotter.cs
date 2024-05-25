using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GraphPlotter : MonoBehaviour, IScrollHandler
{
    public GameObject graphLinePrefab;
    public RectTransform graphContainer;
    public float zoomSpeed = 0.1f;
    public ScrollRect scrollRect;
    public float moveSpeed = 10f; // Speed of movement in units per second

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Dictionary<string, List<Vector2>> stockData = new Dictionary<string, List<Vector2>>();
    private float xScale = 1.0f;
    private float yScale = 1.0f;

    void Start()
    {
        AddStock("Stock1");
        AddStock("Stock2");
        StartCoroutine(MoveLeftRoutine());
    }

    IEnumerator MoveLeftRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Move left every second
            MoveLineLeft();
        }
    }

    void MoveLineLeft()
    {
        foreach (var lineRenderer in lineRenderers)
        {
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 position = lineRenderer.GetPosition(i);
                position.x -= moveSpeed * Time.deltaTime;
                positions[i] = position;
            }
            lineRenderer.SetPositions(positions);
        }
    }

    public void AddStock(string stockName)
    {
        if (!stockData.ContainsKey(stockName))
        {
            stockData[stockName] = new List<Vector2>();
            GameObject lineObject = Instantiate(graphLinePrefab, graphContainer);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            lineRenderers.Add(lineRenderer);
        }
    }

    public void AddStockDataPoint(string stockName, Vector2 dataPoint)
    {
        if (stockData.ContainsKey(stockName))
        {
            stockData[stockName].Add(dataPoint);
            AutoZoomY();
            AutoZoomX();
            PlotAllGraphs();
        }
    }

    void PlotAllGraphs()
    {
        int index = 0;
        float maxWidth = 0;
        foreach (var stock in stockData)
        {
            Vector3[] positions = new Vector3[stock.Value.Count];
            for (int i = 0; i < stock.Value.Count; i++)
            {
                Vector2 point = stock.Value[i];
                Vector2 uiPoint = new Vector2(point.x * xScale, point.y * yScale);
                positions[i] = new Vector3(uiPoint.x, uiPoint.y, 0);
                if (uiPoint.x > maxWidth)
                {
                    maxWidth = uiPoint.x;
                }
            }
            lineRenderers[index].positionCount = positions.Length;
            lineRenderers[index].SetPositions(positions);
            index++;
        }

        // Update the content size to fit the new graph width
        RectTransform contentRectTransform = scrollRect.content;
        contentRectTransform.sizeDelta = new Vector2(maxWidth, contentRectTransform.sizeDelta.y);

        // Auto-scroll to the right to show the latest values
        Canvas.ForceUpdateCanvases();
        scrollRect.horizontalNormalizedPosition = 1f;
    }

    void AutoZoomY()
    {
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var stock in stockData)
        {
            foreach (var point in stock.Value)
            {
                if (point.y < minY) minY = point.y;
                if (point.y > maxY) maxY = point.y;
            }
        }

        float rangeY = maxY - minY;
        yScale = (rangeY == 0) ? 1 : (1.0f / rangeY);

        // Optionally, you can set a minimum yScale to avoid too much zoom-in
        yScale = Mathf.Max(yScale, 0.1f);
    }

    void AutoZoomX()
    {
        int maxPointsToShow = 10;
        int maxPoints = 0;

        foreach (var stock in stockData)
        {
            if (stock.Value.Count > maxPoints)
            {
                maxPoints = stock.Value.Count;
            }
        }

        xScale = (maxPoints > maxPointsToShow) ? 1.0f / (maxPoints - maxPointsToShow + 1) : 1.0f;

        // Update the xScale to fit the latest maxPointsToShow values
        xScale = Mathf.Max(xScale, 0.1f);
    }

    public void OnScroll(PointerEventData eventData)
    {
        float scroll = eventData.scrollDelta.y;
        xScale = Mathf.Clamp(xScale + scroll * zoomSpeed, 0.1f, 10f);
        PlotAllGraphs();
    }
}
