using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderScript : MonoBehaviour
{
    [SerializeField] private float _startingMoney = 1000f;
    protected Dictionary<StocksScriptableObject, int> _stocksOwned = new Dictionary<StocksScriptableObject, int>();
    protected List<StocksScriptableObject> _availableStocks = new List<StocksScriptableObject>();
    public float money { get; protected set; } = 0f;
    public float buyoutValue { get; protected set; } = 0f;
    protected float _buyoutMod = 1;
    public bool boughtOut { get; protected set; } = false;
    public bool isPlayer { get; protected set; } = false;

    public string traderName { get; protected set; }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        _availableStocks = new List<StocksScriptableObject>(StockPriceManager.INSTANCE.stockData.Keys);
        money = _startingMoney;
        _buyoutMod = GameManager.INSTANCE.difficultySettings.enemyBuyoutMod;
    }

    protected virtual void Update()
    {
        float tempValue = money;
        List<StocksScriptableObject> ownedStocks = new List<StocksScriptableObject>(_stocksOwned.Keys);
        for (int s = 0; s < ownedStocks.Count; s++)
        {
            float stockValue = StockPriceManager.INSTANCE.stockData[ownedStocks[s]].currentPrice;
            tempValue += ((float)_stocksOwned[ownedStocks[s]] * stockValue);
        }
        tempValue *= _buyoutMod;
        buyoutValue = tempValue;
    }

    public int GetStocksOwned(StocksScriptableObject stock)
    {
        if (!_stocksOwned.ContainsKey(stock))
        {
            return 0;
        }
        return _stocksOwned[stock];
    }

    protected virtual void BuyStock(StocksScriptableObject stock, int numToBuy)
    {
        if (numToBuy > GetMaxStocksBuy(stock))
        {
            numToBuy = GetMaxStocksBuy(stock);
        }
        if (numToBuy == 0)
        {
            return;
        }
        if (StockPriceManager.INSTANCE.BuyStock(stock, numToBuy, money))
        {
            if (!_stocksOwned.ContainsKey(stock))
            {
                _stocksOwned.Add(stock, 0);
            }
            _stocksOwned[stock] += numToBuy;
            money -= (numToBuy * StockPriceManager.INSTANCE.stockData[stock].currentPrice);
        }
    }

    protected int GetMaxStocksBuy(StocksScriptableObject stock)
    {
        if (!StockPriceManager.INSTANCE.stockData.ContainsKey(stock))
        {
            Debug.LogWarning($"{stock.stockName} is not in stock data.");
            return 0;
        }
        return (int)(money / StockPriceManager.INSTANCE.stockData[stock].currentPrice);
    }

    protected virtual void SellStock(StocksScriptableObject stock, int numToSell)
    {
        if (!_stocksOwned.ContainsKey(stock))
        {
            return;
        }
        if (_stocksOwned[stock] <= 0)
        {
            _stocksOwned[stock] = 0;
            return;
        }
        if (_stocksOwned[stock] < numToSell)
        {
            numToSell = _stocksOwned[stock];
        }
        if (StockPriceManager.INSTANCE.SellStock(stock, numToSell, out float moneyGainedFromSell))
        {
            _stocksOwned[stock] -= numToSell;
            money += moneyGainedFromSell;
        }
    }

    protected void SellAllStocks()
    {
        if (_stocksOwned.Count == 0)
        {
            return;
        }
        foreach (StocksScriptableObject stock in _stocksOwned.Keys)
        {
            if (_stocksOwned[stock] > 0)
            {
                SellStock(stock, _stocksOwned[stock]);
            }
        }
    }

    public bool AttemptBuyout(TraderScript target)
    {
        if (money < target.buyoutValue)
        {
            return false;
        }

        money -= target.buyoutValue;
        target.GetBoughtOut();
        return true;
    }

    public virtual void GetBoughtOut()
    {
        boughtOut = true;
    }
}
