using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StockPriceManager : MonoBehaviour
{
    public static StockPriceManager INSTANCE;
    public struct StockValues
    {
        public float currentPrice;
        public float currentTrend;
        public float trendModifier;
        public int numberInCirculation;
        public int numberInPreviousCirculation;
    }
    [Header("Setup")]
    [SerializeField] private List<StocksScriptableObject> stocks = new List<StocksScriptableObject>();
    [SerializeField] private List<NewsHeadlineScriptableObject> newsHeadlines = new List<NewsHeadlineScriptableObject>();
    public Dictionary<StocksScriptableObject, StockValues> stockData { get; private set; } = new Dictionary<StocksScriptableObject, StockValues>();
    [Header("Market Variables")]
    [SerializeField] private float updateTime = 0.2f;
    [SerializeField] private float timeBetweenHeadlines = 15f;
    [SerializeField] [Range(0.5f, 1000f)] private float stockWeightDivisor = 2.5f;
    [SerializeField] [Range(0.001f, 2.5f)] private float minStockWeight = 0.01f;
    [SerializeField] [Range(2.5f, 35f)] private float maxStockWeight = 10f;
    [SerializeField] private float _stockStartDelay = 1f;
    public bool gameRunning { get; private set; } = true;
    public event UnityAction<Dictionary<StocksScriptableObject, StockValues>> UpdatePrices;
    public event UnityAction<NewsHeadlineScriptableObject> NewsCall;
    private List<NewsHeadlineScriptableObject> activeNewsHeadlines = new List<NewsHeadlineScriptableObject>();


    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            Debug.LogError("More than one stock price manager found.");
            return;
        }
        INSTANCE = this;

        for (int s = 0; s < stocks.Count; s++)
        {
            if (!stockData.ContainsKey(stocks[s]))
            {
                float startingPrice = Random.Range(stocks[s].startingValueRange.x, stocks[s].startingValueRange.y);
                float startingTrend = Random.Range(stocks[s].minMaxDefaultTrend.x, stocks[s].minMaxDefaultTrend.y);
                StockValues values = new StockValues();
                values.currentPrice = startingPrice;
                values.currentTrend = startingTrend;
                values.numberInCirculation = 0;
                values.numberInPreviousCirculation = 0;
                values.trendModifier = 1;
                stockData.Add(stocks[s], values);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(SimulateMarket());
    }

    private IEnumerator SimulateMarket()
    {
        float headlineTime = timeBetweenHeadlines;
        yield return null;
        while (gameRunning)
        {
            List<StocksScriptableObject> stocks = new List<StocksScriptableObject>(stockData.Keys);
            foreach (StocksScriptableObject stock in stocks)
            {
                float trend = Random.Range(stock.minMaxDefaultTrend.x, stock.minMaxDefaultTrend.y);
                float trendMod = CalculateStockTrendChange(stock);
                float stockPrice = CalculateStockPrice(stock, trend, trendMod);
                stockData[stock] = ChangeStockValue(stock, stockPrice, trend, trendMod);
                //Debug.Log($"{stock.stockName}: {stockData[stock].currentPrice}: {stockData[stock].currentTrend}");
            }
            UpdatePrices?.Invoke(stockData);
            if (headlineTime <= 0)
            {
                headlineTime = timeBetweenHeadlines;
                CallNewsHeadline();
            }
            yield return new WaitForSeconds(updateTime);
            headlineTime -= (updateTime + Time.deltaTime);
        }
    }

    private float CalculateStockTrendChange(StocksScriptableObject stock)
    {
        int numInCir = stockData[stock].numberInCirculation;
        int numInPrevCir = stockData[stock].numberInPreviousCirculation;
        float trendChange = 0;
        float stockInCirTrendModifier = 0;
        if (numInCir - numInPrevCir != 0)
        {
            stockInCirTrendModifier = (float)(numInCir - numInPrevCir);
            if (stockInCirTrendModifier < 0)
            {
                stockInCirTrendModifier = -1 * Mathf.Log(-1 * stockInCirTrendModifier);
            }
            else
            {
                stockInCirTrendModifier = Mathf.Log(stockInCirTrendModifier);
            }
        }

        trendChange += stockInCirTrendModifier;

        if (activeNewsHeadlines.Count > 0)
        {
            for (int n = 0; n < activeNewsHeadlines.Count; n++)
            {
                if (activeNewsHeadlines[n].stockEffects.ContainsKey(stock))
                {
                    float change = Random.Range(activeNewsHeadlines[n].stockEffects[stock].x, activeNewsHeadlines[n].stockEffects[stock].y);
                    trendChange += change;
                }
            }
        }

        return trendChange;
    }

    private float CalculateStockPrice(StocksScriptableObject stock, float trend, float trendMod)
    {
        float newPrice = stockData[stock].currentPrice;
        float actualTrend = trend + trendMod;
        newPrice = Mathf.Max(0.01f, newPrice + actualTrend);
        return newPrice;
    }

    private IEnumerator ApplyNewsEffect(NewsHeadlineScriptableObject headline)
    {
        yield return new WaitForSeconds(_stockStartDelay);
        activeNewsHeadlines.Add(headline);
        yield return new WaitForSeconds(Random.Range(headline.minEffectTime, headline.maxEffectTime));
        if (activeNewsHeadlines.Contains(headline))
        {
            activeNewsHeadlines.Remove(headline);
            Debug.Log($"{headline.newsHeadline} ended");
        }
    }

    private void CallNewsHeadline()
    {
        NewsHeadlineScriptableObject newsHeadline = ChooseRandomNewsHeadline();
        if (newsHeadline == null)
        {
            return;
        }
        NewsCall?.Invoke(newsHeadline);
        if (newsHeadline.stockEffects.Keys.Count != 0)
        {
            StartCoroutine(ApplyNewsEffect(newsHeadline));
        }
        Debug.Log($"{newsHeadline.newsHeadline} started");
    }

    private NewsHeadlineScriptableObject ChooseRandomNewsHeadline()
    {
        if (newsHeadlines.Count == 0)
        {
            Debug.LogError("No headlines assigned!");
            return null;
        }
        NewsHeadlineScriptableObject chosenNewsHeadline = newsHeadlines[Random.Range(0, newsHeadlines.Count)];
        while (activeNewsHeadlines.Contains(chosenNewsHeadline))
        {
            chosenNewsHeadline = newsHeadlines[Random.Range(0, newsHeadlines.Count)];
            if (activeNewsHeadlines.Count == newsHeadlines.Count)
            {
                chosenNewsHeadline = null;
                break;
            }
        }
        return chosenNewsHeadline;
    }

    private StockValues ChangeStockValue(StocksScriptableObject stock, float currentPrice = -1, float currentTrend = -1, float trendModifier = -1, int numInCir = -1, int numInPrevCir = -1)
    {
        StockValues value = new StockValues();
        if (!stockData.ContainsKey(stock))                                    
        {
            Debug.LogError($"Stock Data does not contain {stock.stockName}");
            return value;
        }
        if (currentPrice < 0)
        {
            value.currentPrice = stockData[stock].currentPrice;
        }
        else
        {
            value.currentPrice = currentPrice;
        }
        if (currentTrend < 0)
        {
            value.currentTrend = stockData[stock].currentTrend;
        }
        else
        {
            value.currentTrend = currentTrend;
        }
        if (trendModifier < 0)
        {
            value.trendModifier = stockData[stock].trendModifier;
        }
        else
        {
            value.trendModifier = trendModifier;
        }
        if (numInCir < 0)
        {
            value.numberInCirculation = stockData[stock].numberInCirculation;
        }
        else
        {
            value.numberInCirculation = numInCir;
        }
        if (numInPrevCir < 0)
        {
            value.numberInPreviousCirculation = stockData[stock].numberInPreviousCirculation;
        }
        else
        {
            value.numberInPreviousCirculation = numInPrevCir;
        }

        return value;
    }

    public bool BuyStock(StocksScriptableObject stock, int numToBuy, float totalMoney)
    {
        if (!stockData.ContainsKey(stock))
        {
            return false;
        }
        if (totalMoney < (stockData[stock].currentPrice * (float)numToBuy))
        {
            return false;
        }
        stockData[stock] = ChangeStockValue(stock, -1, -1, -1, stockData[stock].numberInCirculation + numToBuy);
        return true;
    }

    public bool SellStock(StocksScriptableObject stock, int numToSell, out float moneyGainedFromSell)
    {
        if (!stockData.ContainsKey(stock))
        {
            moneyGainedFromSell = 0f;
            return false;
        }
        int stocksInCir = Mathf.Max(stockData[stock].numberInCirculation - numToSell, 0);
        stockData[stock] = ChangeStockValue(stock, -1, -1, -1, stocksInCir);
        moneyGainedFromSell = stockData[stock].currentPrice * numToSell;
        return true;
    }
}
