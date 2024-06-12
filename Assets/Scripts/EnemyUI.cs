using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _AINameText;
    [SerializeField] private TMP_Text _AIBuyoutValueText;
    [SerializeField] private Image _selectedImage;
    [SerializeField] private TraderScript _trader;
    private void Start()
    {
        if (_trader != null)
        {
            if (_AINameText != null)
            {
                _AINameText.text = _trader.traderName;
            }
        }
        _selectedImage.gameObject.SetActive(false);
        PlayerScript.SwappedTrader += AISelected;
    }

    private void OnDisable()
    {
        PlayerScript.SwappedTrader -= AISelected;
    }

    private void Update()
    {
        if (_trader != null)
        {
            if (_AIBuyoutValueText != null)
            {
                _AIBuyoutValueText.text = $"Buyout Value: {_trader.buyoutValue}";
            }
        }
    }

    public void AISelected(TraderScript trader)
    {
        if (trader != _trader || trader == null)
        {
            _selectedImage.gameObject.SetActive(false);
            return;
        }
        _selectedImage.gameObject.SetActive(true);
    }

    public void AttemptBuyout()
    {
        PlayerScript.INSTANCE.AttemptBuyout(_trader);
    }
}
