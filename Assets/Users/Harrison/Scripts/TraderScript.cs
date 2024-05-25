using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderScript : MonoBehaviour
{
    [SerializeField] private float _startingMoney = 1000f;
    protected Dictionary<StocksScriptableObject, int> _stocksOwned = new Dictionary<StocksScriptableObject, int>();
    protected List<StocksScriptableObject> _availableStocks = new List<StocksScriptableObject>();
    public float money { get; protected set; } = 0f;

    protected virtual void Start()
    {
        _availableStocks = new List<StocksScriptableObject>(StockPriceManager.INSTANCE.stockData.Keys);
        money = _startingMoney;
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
}
