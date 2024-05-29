using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StockUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _stockNameText;
    [SerializeField] private TMP_Text _ownedStockAmountText;
    [SerializeField] private StocksScriptableObject _stockToWatch;

    private void OnEnable()
    {
        PlayerScript.PlayerStockAmountChange += UpdateUI;
    }

    private void OnDisable()
    {
        PlayerScript.PlayerStockAmountChange -= UpdateUI;
    }

    private void Start()
    {
        _stockNameText.text = _stockToWatch.stockName;
    }

    public void UpdateUI(StocksScriptableObject stock, int newValue)
    {
        if (_stockToWatch != stock)
        {
            return;
        }
        _ownedStockAmountText.text = $"Owned: {newValue}";
    }

    public void Buy()
    {
        if (_stockToWatch != null)
        {
            if (StockPriceManager.INSTANCE.stockData.ContainsKey(_stockToWatch))
            {
                PlayerScript.INSTANCE.PlayerBuyStock(_stockToWatch);
            }
        }
    }

    public void Sell()
    {
        if (_stockToWatch != null)
        {
            if (StockPriceManager.INSTANCE.stockData.ContainsKey(_stockToWatch))
            {
                PlayerScript.INSTANCE.PlayerSellStock(_stockToWatch);
            }
        }
    }
}
