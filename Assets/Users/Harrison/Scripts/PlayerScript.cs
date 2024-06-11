using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerScript : TraderScript
{
    public static PlayerScript INSTANCE;
    public static event UnityAction<StocksScriptableObject> SwappedStock;
    public static event UnityAction<TraderScript> SwappedTrader;
    public static event UnityAction<StocksScriptableObject, int> PlayerStockAmountChange;
    [SerializeField] [Range(1f, 2f)] private float _holdMultiplyer = 1.25f;
    [SerializeField] private float _timeBetweenBuysSells = 0.05f;
    private float _buySellCooldown = 0;
    private float _buySellMod = 1;
    public int _stockSelected { get; private set; } = 0;
    public int _enemySelected { get; private set; } = 0;
    private List<TraderScript> _availableEnemies = new List<TraderScript>();

    StocksScriptableObject stockToWatch;

    public enum ActionSelected
    {
        BUYING,
        SELLING,
        ENEMY
    }

    public enum MarketState
    {
        NONE,
        BUYING,
        SELLING,
        PREFORMACTION
    }
    /*
     * List<stock1>
     */

    public MarketState state { get; private set; } = MarketState.NONE;
    private MarketState _lastState = MarketState.NONE;
    public ActionSelected actionSelected = ActionSelected.BUYING;

    public void InputBuy(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            state = MarketState.BUYING;
            _buySellCooldown = 0;
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
            _buySellCooldown = 0;
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
        if (!context.started)
        {
            return;
        }
        int direction = (int)Mathf.Sign(context.ReadValue<float>());
        if (actionSelected == ActionSelected.ENEMY)
        {
            _enemySelected += direction;
            if (_enemySelected < 0)
            {
                _enemySelected = _availableEnemies.Count - 1;
                actionSelected = ActionSelected.BUYING;
            }
            else if (_enemySelected >= _availableEnemies.Count)
            {
                _enemySelected = 0;
                actionSelected = ActionSelected.BUYING;
            }
        }
        else
        {
            _stockSelected += direction;
            if (_stockSelected < 0)
            {
                _stockSelected = _availableStocks.Count - 1;
                actionSelected = ActionSelected.ENEMY;
            }
            else if (_stockSelected >= _availableStocks.Count)
            {
                _stockSelected = 0;
                actionSelected = ActionSelected.ENEMY;
            }
        }
        if (actionSelected == ActionSelected.ENEMY)
        {
            SwappedStock?.Invoke(null);
            SwappedTrader?.Invoke(_availableEnemies[_enemySelected]);
        }
        else
        {
            SwappedStock?.Invoke(_availableStocks[_stockSelected]);
            SwappedTrader?.Invoke(null);
        }
    }

    public void InputSwapBuySell(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        if (actionSelected == ActionSelected.ENEMY)
        {
            return;
        }
        int direction = (int)Mathf.Sign(context.ReadValue<float>());
        if (direction > 0)
        {
            actionSelected = ActionSelected.SELLING;
        }
        else
        {
            actionSelected = ActionSelected.BUYING;
        }
        SwappedStock?.Invoke(_availableStocks[_stockSelected]);
    }

    public void InputAction(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            Debug.Log("Stopped Action");
            state = MarketState.NONE;
            return;
        }
        Debug.Log("Action");
        state = MarketState.PREFORMACTION;
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
        SwappedStock?.Invoke(_availableStocks[_stockSelected]);
        SwappedTrader?.Invoke(null);
        _buyoutMod = GameManager.INSTANCE.difficultySettings.playerBuyoutMod;
        TraderScript[] traders = FindObjectsByType<TraderScript>(FindObjectsSortMode.None);
        for (int t = 0; t < traders.Length; t++)
        {
            if (traders[t] != this)
            {
                _availableEnemies.Add(traders[t]);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        _buySellCooldown -= Time.deltaTime;
        if (_lastState != state)
        {
            _buySellMod = 1;
        }
        if (state != MarketState.NONE)
        {
            if (_buySellCooldown <= 0)
            {
                switch (state) 
                {
                    case MarketState.BUYING:
                        BuyStock(_availableStocks[_stockSelected], (int)_buySellMod);
                        break;
                    case MarketState.SELLING:
                        SellStock(_availableStocks[_stockSelected], (int)_buySellMod);
                        break;
                    default:
                        if (actionSelected == ActionSelected.BUYING)
                        {
                            BuyStock(_availableStocks[_stockSelected], (int)_buySellMod);
                        }
                        else if (actionSelected == ActionSelected.SELLING)
                        {
                            SellStock(_availableStocks[_stockSelected], (int)_buySellMod);
                        }
                        break;
                }
                _buySellMod = Mathf.Min(_holdMultiplyer * _buySellMod, 10000);
                _buySellCooldown = _timeBetweenBuysSells;
            }
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
        if (!_stocksOwned.ContainsKey(stock))
        {
            PlayerStockAmountChange?.Invoke(stock, 0);
            return;
        }
        PlayerStockAmountChange?.Invoke(stock, _stocksOwned[stock]);
    }

    protected override void SellStock(StocksScriptableObject stock, int numToSell)
    {
        base.SellStock(stock, numToSell);
        if (!_stocksOwned.ContainsKey(stock))
        {
            PlayerStockAmountChange?.Invoke(stock, 0);
            return;
        }
        PlayerStockAmountChange?.Invoke(stock, _stocksOwned[stock]);
    }
}
