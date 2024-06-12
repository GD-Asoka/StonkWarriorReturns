using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StockUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _stockNameText;
    [SerializeField] private TMP_Text _ownedStockAmountText;
    [SerializeField] private StocksScriptableObject _stockToWatch;
    [SerializeField] private Image _selectedImage;
    [SerializeField] private Color _selectedColour;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private Color _buyActiveColour;
    [SerializeField] private Color _buyInactiveColour;
    [SerializeField] private Image _sellButtonImage;
    [SerializeField] private Color _sellActiveColour;
    [SerializeField] private Color _sellInactiveColour;

    private void OnEnable()
    {
        PlayerScript.PlayerStockAmountChange += UpdateUI;
        PlayerScript.SwappedStock += SelectionChange;
    }

    private void OnDisable()
    {
        PlayerScript.PlayerStockAmountChange -= UpdateUI;
        PlayerScript.SwappedStock -= SelectionChange;
    }

    private void Start()
    {
        _stockNameText.text = _stockToWatch.stockName;
        _selectedImage.color = _selectedColour;
    }

    public void SelectionChange(StocksScriptableObject stock)
    {
        if (!_buyButtonImage || _buyInactiveColour == null || !_sellButtonImage || _sellInactiveColour == null)
            print(gameObject.name);

        _buyButtonImage.color = _buyInactiveColour;
        _sellButtonImage.color = _sellInactiveColour;
        if (_stockToWatch != stock)
        {
            _selectedImage.gameObject.SetActive(false);
            return;
        }
        if (PlayerScript.INSTANCE != null)
        {
            if (PlayerScript.INSTANCE.actionSelected == PlayerScript.ActionSelected.BUYING)
            {
                _buyButtonImage.color = _buyActiveColour;
            }
            else
            {
                _sellButtonImage.color = _sellActiveColour;
            }
        }
        _selectedImage.gameObject.SetActive(true);
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
            Debug.Log("HELLO");
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
