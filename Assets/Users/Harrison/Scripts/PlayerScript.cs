using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : TraderScript
{
    public static PlayerScript INSTANCE;
    [SerializeField] [Range(1f, 2f)] private float _holdMultiplyer = 1.25f;
    private float _buySellMod = 1;
    public int _stockSelected { get; private set; } = 0;
    

    public enum MarketState
    {
        NONE,
        BUYING,
        SELLING
    }

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
    }

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
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

    public override void GetBoughtOut()
    {
        base.GetBoughtOut();
        GameManager.INSTANCE.PlayerLost();
    }
}
