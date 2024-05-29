using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerScript : TraderScript
{
    public static PlayerScript INSTANCE;
    public static event UnityAction<StocksScriptableObject> SwappedStock;
    public static event UnityAction<StocksScriptableObject, int> PlayerStockAmountChange;
    [SerializeField] [Range(1f, 2f)] private float _holdMultiplyer = 1.25f;
    private float _buySellMod = 1;
    public int _stockSelected { get; private set; } = 0;

    StocksScriptableObject stockToWatch;

    public enum MarketState
    {
        NONE,
        BUYING,
        SELLING
    }
    /*
     * List<stock1>
     */

    public MarketState state { get; private set; } = MarketState.NONE;
    private MarketState _lastState = MarketState.NONE;

    public void InputBuy(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            state = MarketState.BUYING;
        }
        if (context.canceled)
        {
            if (state == MarketState.BUYING)
            {
                state = MarketState.NONE;
            }
        }
    }

    public void InputSell(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            state = MarketState.SELLING;
        }
        if (context.canceled)
        {
            if (state == MarketState.SELLING)
            {
                state = MarketState.NONE;
            }
        }
    }

    public void InputSwapStock(InputAction.CallbackContext context)
    {
        int direction = (int)Mathf.Sign(context.ReadValue<float>());
        _stockSelected += direction;
        if (_stockSelected < 0)
        {
            _stockSelected = _availableStocks.Count - 1;
        }
        else if (_stockSelected >= _availableStocks.Count)
        {
            _stockSelected = 0;
        }
        SwappedStock?.Invoke(_availableStocks[_stockSelected]);
    }

    protected override void Awake()
    {
        base.Awake();
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
        isPlayer = true;
    }

    protected override void Start()
    {
        base.Start();
        _stockSelected = 0;
        _buyoutMod = GameManager.INSTANCE.difficultySettings.playerBuyoutMod;
    }

    protected override void Update()
    {
        base.Update();
        if (_lastState != state)
        {
            _buySellMod = 1;
        }
        if (state != MarketState.NONE)
        {
            if (state == MarketState.BUYING)
            {
                BuyStock(_availableStocks[_stockSelected], (int)_buySellMod);
            }
            else
            {
                SellStock(_availableStocks[_stockSelected], (int)_buySellMod);
            }
            _buySellMod = Mathf.Min(_holdMultiplyer * _buySellMod, 10000);
        }

        _lastState = state;
    }

    public void SwapStock(StocksScriptableObject stock)
    {
        if (!_availableStocks.Contains(stock))
        {
            Debug.LogWarning($"Stock {stock.name} isn't an available stock.");
            return;
        }

        _stockSelected = _availableStocks.IndexOf(stock);
        SwappedStock?.Invoke(_availableStocks[_stockSelected]);
    }

    public void PlayerBuyStock(StocksScriptableObject stock, int amount = 1)
    {
        BuyStock(stock, amount);
    }

    public void PlayerSellStock(StocksScriptableObject stock, int amount = 1)
    {
        SellStock(stock, amount);
    }

    public void PlayerSellAllStock()
    {
        SellAllStocks();
    }

    public override void GetBoughtOut()
    {
        base.GetBoughtOut();
        GameManager.INSTANCE.PlayerLost();
    }

    protected override void BuyStock(StocksScriptableObject stock, int numToBuy)
    {
        base.BuyStock(stock, numToBuy);
        PlayerStockAmountChange?.Invoke(stock, _stocksOwned[stock]);
    }

    protected override void SellStock(StocksScriptableObject stock, int numToSell)
    {
        base.SellStock(stock, numToSell);
        PlayerStockAmountChange?.Invoke(stock, _stocksOwned[stock]);
    }
}
