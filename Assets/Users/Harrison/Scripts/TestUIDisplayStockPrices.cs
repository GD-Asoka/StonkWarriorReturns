using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestUIDisplayStockPrices : MonoBehaviour
{
    [SerializeField] private StocksScriptableObject stockToWatch;
    private TMP_Text text;
    private List<float> prices = new List<float>();

    private void OnEnable()
    {
        text = GetComponent<TMP_Text>();
        if (StockPriceManager.INSTANCE != null)
        {
            StockPriceManager.INSTANCE.UpdatePrices += UpdateStock;
        }
    }
    private void Start()
    {
        text = GetComponent<TMP_Text>();
        if (StockPriceManager.INSTANCE != null)
        {
            StockPriceManager.INSTANCE.UpdatePrices += UpdateStock;
        }
    }

    private void OnDisable()
    {
        if (StockPriceManager.INSTANCE != null)
        {
            StockPriceManager.INSTANCE.UpdatePrices -= UpdateStock;
        }
    }

    public void UpdateStock(Dictionary<StocksScriptableObject, StockPriceManager.StockValues> stocksDict)
    {
        if (!stocksDict.ContainsKey(stockToWatch))
        {
            return;
        }
        prices.Add(stocksDict[stockToWatch].currentPrice);
        string textToDisplay = "";
        textToDisplay += $"{stockToWatch.stockName} Price: {stocksDict[stockToWatch].currentPrice} Trend: {stocksDict[stockToWatch].currentTrend}";
        text.text = textToDisplay;
    }
}
