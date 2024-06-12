using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIScript : TraderScript
{
    [SerializeField] private List<StocksScriptableObject> _stocksToWatch = new List<StocksScriptableObject>();
    private Dictionary<StocksScriptableObject, float> _stockPrices = new Dictionary<StocksScriptableObject, float>();
    [SerializeField] [Range(0.01f, 100f)] private float _buyPercent = 25f;
    [SerializeField] int _numberStocksToBuy = 10;
    [SerializeField] [Range(0.01f, 100f)] private float _sellPercentGain = 15f;
    [SerializeField] private float _buyCooldown = 10f;
    [SerializeField] [Range(0.01f, 100f)] private float chaosBuy = 1f;
    [SerializeField] [Range(0.01f, 100f)] private float chaosSell = 1f;
    private StocksScriptableObject _boughtStock;
    public float currentBuy, currentSell;
    public bool chaosMode = false;

    private void OnEnable()
    {
        StockPriceManager.UpdatePrices += UpdatePrices;
    }

    private void OnDisable()
    {
        StockPriceManager.UpdatePrices -= UpdatePrices;
    }

    protected override void Awake()
    {
        base.Awake();
        for (int s = 0; s < _stocksToWatch.Count; s++)
        {
            if (!_stockPrices.ContainsKey(_stocksToWatch[s]))
            {
                _stockPrices.Add(_stocksToWatch[s], -1f);
            }
        }
        _stocksToWatch = new List<StocksScriptableObject>(_stockPrices.Keys);
        currentBuy = _buyPercent;
        currentSell = _sellPercentGain;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CheckStocksToBuy());
    }

    protected override void Update()
    {
        base.Update();
        AttemptBuyout(PlayerScript.INSTANCE);
    }

    private IEnumerator CheckStocksToBuy()
    {
        while (!boughtOut)
        {
            yield return null;
            if (_boughtStock != null && !chaosMode)
            {
                continue;
            }
            for (int s = 0; s < _stocksToWatch.Count; s++)
            {
                StocksScriptableObject stock = _stocksToWatch[s];
                if (_stockPrices[stock] * (float)_numberStocksToBuy <= money * (_buyPercent/100))
                {
                    BuyStock(stock, _numberStocksToBuy);
                    _boughtStock = stock;
                    CheckStockToSell(stock, _stockPrices[stock] * (float)_numberStocksToBuy);
                }
            }
        }
    }

    private IEnumerator CheckStockToSell(StocksScriptableObject stock, float buyPrice)
    {
        yield return null;
        while (_stocksOwned[stock] > 0)
        {
            if (_stocksOwned[stock] * _stockPrices[stock] > buyPrice * ((_sellPercentGain / 100) + 1))
            {
                SellStock(stock, GetStocksOwned(stock));
                if (!chaosMode)
                {
                    yield return new WaitForSeconds(_buyCooldown);
                    _boughtStock = null;
                }
            }
            yield return null;
        }
    }

    public void UpdatePrices(Dictionary<StocksScriptableObject, StockPriceManager.StockValues> newPrices)
    {
        for (int s = 0; s < _stocksToWatch.Count; s++)
        {
            StocksScriptableObject stock = _stocksToWatch[s];
            if (newPrices.ContainsKey(stock))
            {
                _stockPrices[stock] = newPrices[stock].currentPrice;
            }
        }
    }

    public void ChaosModeToggle()
    {
        chaosMode = !chaosMode;
        if(chaosMode)
        {
            _buyPercent = chaosBuy;
            _sellPercentGain = chaosSell;
        }
        else
        {
            _buyPercent = currentBuy;
            _sellPercentGain = currentSell;
        }
    }
}
