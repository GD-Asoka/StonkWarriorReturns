using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UILineRenderer : Graphic
{
    public UIGridRenderer grid;
    public GameObject cnvs;

    public Vector2Int gridSize;
    public List<Vector2> points;

    private float width, height, unitWidth, unitHeight;

    public float thickness = 10, moveSpeed = 1;

    public int xUpdateValue = 6, pointsLimit = 10, xLimit = 12;

    public float repeatRate = 0.1f;

    private float currentPrice; 
    
    public StocksScriptableObject stockToWatch;

    private new void OnEnable()
    {
        if (StockPriceManager.INSTANCE != null)
        {
            StockPriceManager.INSTANCE.UpdatePrices += UpdateStock;
        }
        InvokeRepeating(nameof(UpdateGraph), repeatRate, repeatRate);
        if(UnityEngine.Application.isPlaying)
        {
            transform.SetParent(cnvs.transform);
        }
    }

    private new void OnDisable()
    {
        if (StockPriceManager.INSTANCE != null)
        {
            StockPriceManager.INSTANCE.UpdatePrices += UpdateStock;
        }
        CancelInvoke(nameof(UpdateGraph));
    }

    public void UpdateStock(Dictionary<StocksScriptableObject, StockPriceManager.StockValues> stocksDict)
    {
        currentPrice = stocksDict[stockToWatch].currentPrice;
    }

    //protected override void Start()
    //{
    //    InvokeRepeating(nameof(UpdateGraph), repeatRate, repeatRate);
    //}

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unitWidth = width / (float)gridSize.x;
        unitHeight = height / (float)gridSize.y;

        if(points.Count < 2)
        {
            return;
        }

        float angle = 0;
        for(int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            if(i < points.Count - 1)
            {
                angle = GetAngle(points[i], points[i + 1]) + 45f;
            }
            DrawVertsForPoint(point, vh, angle);
        }

        for(int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    public void DrawVertsForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0,0,angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0); 
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);
    }

    public float GetAngle(Vector2 point, Vector2 target)
    {
        return (float)Mathf.Atan2(target.y - point.y, target.x - point.x) * (180 / Mathf.PI);
    }

    private void UpdateGraph()
    {
        if (UnityEngine.Application.isPlaying)
        {
            if (points.Count > 0)
            {
                points.Add(new Vector2(points[points.Count - 1].x + xUpdateValue, currentPrice / 10));
                if (points.Count > pointsLimit)
                {
                    points.RemoveAt(0);
                    for (int i = 0; i < points.Count; i++)
                    {
                        points[i] = new Vector2(i * xUpdateValue, points[i].y);
                    }
                }
                SetVerticesDirty(); // Mark vertices as dirty to redraw the line
            }
            else if (points.Count == 0)
            {
                points.Add(new Vector2(0.5f, currentPrice / 10));
                SetVerticesDirty(); // Mark vertices as dirty to redraw the line
            }

            // Update the color based on the last point
            if (points.Count >= 2)
            {
                if (points[points.Count - 1].y > points[points.Count - 2].y)
                {
                    this.color = Color.green;
                }
                else if (points[points.Count - 1].y < points[points.Count - 2].y)
                {
                    this.color = Color.red;
                }
            }
        }
    }

    public void DrawPoint()
    {
        if (grid)
        {
            if (gridSize != grid.gridSize)
            {
                gridSize = grid.gridSize;
                SetVerticesDirty();
            }
        }
    }
}


//private void UpdateGraph()
//{
//    if (UnityEngine.Application.isPlaying)
//    {

//        if (points.Count > 0)
//        {
//            points.Add(new Vector2(points[points.Count - 1].x + xUpdateValue, currentPrice / 10));
//            Canvas.ForceUpdateCanvases();
//            DrawPoint();
//            if (points[points.Count - 1].y > points[points.Count - 2].y)
//            {
//                this.color = Color.green;
//            }
//            if (points[points.Count - 1].y < points[points.Count - 1].y)
//            {
//                this.color = Color.red;
//            }
//        }
//        else if (points.Count == 0)
//        {
//            points.Add(new Vector2(0.5f, currentPrice / 10));
//        }

//        if (points.Count >= pointsLimit)
//        {
//            for (int p = points.Count - 1; p > 0; p--)
//            {
//                points[p] = new Vector2(points[p - 1].x, points[p].y);
//            }
//            points.RemoveAt(0);
//        }
//    }
//}
