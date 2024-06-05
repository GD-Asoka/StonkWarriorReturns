using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(RectTransform))]
public class GraphScript : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private RectTransform _rectTransform;
    private Vector3 _rectZeroPosition;
    private Vector3 _rectMaxPosition;
    private Vector3 _graphZeroPosition;
    private Vector3 _graphMaxPosition;
    private SpriteRenderer _backgroundSpriteRenderer;
    [SerializeField] private RectTransform _targetSizeRect;
    [SerializeField] private TMP_Text _minValueText;
    [SerializeField] private TMP_Text _maxValueText;
    [SerializeField] private TMP_Text _midValueText;
    [SerializeField] private TMP_Text _stockValueText;
    [SerializeField] private float _sideTextSpacing = 10f;
    [SerializeField] private float _graphSpacing = 0.1f;
    [SerializeField] private float _minimumGraphSize = 90f;
    [SerializeField] private StocksScriptableObject _stockToWatch;
    [SerializeField] private int _maxPoints = 10;
    private float _graphMaxValue = 100f;
    private float _graphMinValue = 10f;
    private Dictionary<StocksScriptableObject, List<float>> _stocksPrices = new Dictionary<StocksScriptableObject, List<float>>();

    private void OnEnable()
    {
        StockPriceManager.UpdatePrices += AddPrices;
        PlayerScript.SwappedStock += ChangeStockWatched;
    }

    private void OnDisable()
    {
        StockPriceManager.UpdatePrices -= AddPrices;
        PlayerScript.SwappedStock -= ChangeStockWatched;
    }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _rectTransform = GetComponent<RectTransform>();
        _backgroundSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _rectTransform.position = Camera.main.ScreenToWorldPoint(_targetSizeRect.position);

        float targetWidthPercent = (_targetSizeRect.rect.width - _minValueText.rectTransform.rect.width) / Screen.width;
        float targetHeightPercent = (_targetSizeRect.rect.height - _stockValueText.rectTransform.rect.height) / Screen.height;

        float aspect = (float)Screen.width / (float)Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;

        float actualWidth = (worldWidth * targetWidthPercent);
        float actualHeight = (worldHeight * targetHeightPercent);

        _rectTransform.sizeDelta = new Vector2(actualWidth, actualHeight);

        _targetSizeRect.gameObject.SetActive(false);

        _rectZeroPosition = new Vector3(_rectTransform.position.x, _rectTransform.position.y, 0) - new Vector3(_rectTransform.rect.width / 2, _rectTransform.rect.height / 2, 0);
        _rectMaxPosition = new Vector3(_rectTransform.position.x, _rectTransform.position.y, 0) + new Vector3(_rectTransform.rect.width / 2, _rectTransform.rect.height /2, 0);
        
        _rectZeroPosition = new Vector3(_rectZeroPosition.x, _rectZeroPosition.y, 0);
        _rectMaxPosition = new Vector3(_rectMaxPosition.x, _rectMaxPosition.y, 0);

        _graphZeroPosition = new Vector3(_rectZeroPosition.x + _graphSpacing, _rectZeroPosition.y + _graphSpacing, 0);
        _graphMaxPosition = new Vector3(_rectMaxPosition.x - _graphSpacing, _rectMaxPosition.y - _graphSpacing, 0);

        if (_backgroundSpriteRenderer != null)
        {
            _backgroundSpriteRenderer.size = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        }

        Vector3 textPosition = Vector3.zero;

        if (_minValueText != null)
        {
            textPosition = Camera.main.WorldToScreenPoint(_rectZeroPosition);
            textPosition = new Vector3(textPosition.x - (_minValueText.rectTransform.rect.width / 2) - _sideTextSpacing, textPosition.y, textPosition.z);
            _minValueText.rectTransform.position = textPosition;
            _minValueText.text = "Minimum";
        }
        if (_maxValueText != null)
        {
            textPosition = new Vector3(_rectZeroPosition.x, _rectMaxPosition.y, 0);
            textPosition = Camera.main.WorldToScreenPoint(textPosition);
            textPosition = new Vector3(textPosition.x - (_maxValueText.rectTransform.rect.width / 2) - _sideTextSpacing, textPosition.y, textPosition.z);
            _maxValueText.rectTransform.position = textPosition;
            _maxValueText.text = "Maximum";
        }
        if (_midValueText != null)
        {
            textPosition = new Vector3(_rectZeroPosition.x, (_rectMaxPosition.y + _rectZeroPosition.y) / 2, 0);
            textPosition = Camera.main.WorldToScreenPoint(textPosition);
            textPosition = new Vector3(textPosition.x - (_maxValueText.rectTransform.rect.width / 2) - _sideTextSpacing, textPosition.y, textPosition.z);
            _midValueText.rectTransform.position = textPosition;
            _midValueText.text = "Midpoint";
        }
        if (_stockValueText != null)
        {
            textPosition = new Vector3((_rectMaxPosition.x + _rectZeroPosition.x) / 2, _rectMaxPosition.y, 0);
            textPosition = Camera.main.WorldToScreenPoint(textPosition);
            textPosition = new Vector3(textPosition.x, textPosition.y + (_stockValueText.rectTransform.rect.height / 2) + _sideTextSpacing, textPosition.z);
            _stockValueText.rectTransform.position = textPosition;
            _stockValueText.text = $"$0";
        }
    }

    public void ChangeStockWatched(StocksScriptableObject stock)
    {
        _stockToWatch = stock;
    }

    public void AddPrices(Dictionary<StocksScriptableObject, StockPriceManager.StockValues> stockData)
    {
        foreach (StocksScriptableObject stock in stockData.Keys)
        {
            if (!_stocksPrices.ContainsKey(stock))
            {
                _stocksPrices.Add(stock, new List<float>());
            }
            _stocksPrices[stock].Add(stockData[stock].currentPrice);
            while (_stocksPrices[stock].Count > _maxPoints)
            {
                _stocksPrices[stock].RemoveAt(0);
            }
        }

        PlacePoints(_stocksPrices[_stockToWatch].ToArray());
    }

    public void PlacePoints(float[] points)
    {
        float maxValue = 0;
        float minValue = float.PositiveInfinity;
        for (int p = 0; p < points.Length; p++)
        {
            maxValue = Mathf.Max(maxValue, points[p]);
            minValue = Mathf.Min(minValue, points[p]);
        }
        
        if (maxValue - minValue < _minimumGraphSize)
        {
            if (maxValue <= _minimumGraphSize)
            {
                maxValue = _minimumGraphSize;
                minValue = 0;
            }
        }

        if (minValue > maxValue - _minimumGraphSize)
        {
            minValue = maxValue - _minimumGraphSize;
        }
        
        if (minValue < 10)
        {
            minValue = 0;
        }

        Vector3[] graphPoints = new Vector3[points.Length];
        for (int p = 0; p < points.Length; p++)
        {
            float xPos = _graphZeroPosition.x + (((_graphMaxPosition.x - _graphZeroPosition.x) / points.Length) * p);
            float yPos = _graphZeroPosition.y + (((_graphMaxPosition.y - _graphZeroPosition.y) / maxValue) * points[p]);
            
            graphPoints[p] = new Vector3(xPos, yPos, 0);
        }

        _minValueText.text = $"${minValue}";
        _midValueText.text = $"${minValue + ((maxValue - minValue) / 2)}";
        _maxValueText.text = $"${maxValue}";
        _stockValueText.text = $"{_stockToWatch.stockName} Value: ${points[points.Length - 1]}";

        _lineRenderer.startColor = _stockToWatch.color;
        _lineRenderer.endColor = _stockToWatch.color;
                    
        _lineRenderer.positionCount = graphPoints.Length;
        _lineRenderer.SetPositions(graphPoints);
    }

}
