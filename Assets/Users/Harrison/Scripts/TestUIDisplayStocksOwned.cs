using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestUIDisplayStocksOwned : MonoBehaviour
{
    private TMP_Text _text;
    private List<StocksScriptableObject> _stocks = new List<StocksScriptableObject>();

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _stocks = new List<StocksScriptableObject>(StockPriceManager.INSTANCE.stockData.Keys);
    }

    private void Update()
    {
        string textToDisplay = "";
        for (int s = 0; s < _stocks.Count; s++)
        {
            //Debug.Log(_stocks[s].stockName);
            //ABHI: CHANGED STOCK NAME TO ACRONYM
            textToDisplay += $"{_stocks[s].acronym} Owned: {PlayerScript.INSTANCE.GetStocksOwned(_stocks[s])} ";
            if (PlayerScript.INSTANCE._stockSelected == s)
            {
                switch (PlayerScript.INSTANCE.state)
                {
                    case PlayerScript.MarketState.BUYING:
                        textToDisplay += "Buying";
                        break;
                    case PlayerScript.MarketState.SELLING:
                        textToDisplay += "Selling";
                        break;
                    default:
                        textToDisplay += "Selected";
                        break;
                }
            }
            textToDisplay += "\n";
        }
        _text.text = textToDisplay;
    }
}
