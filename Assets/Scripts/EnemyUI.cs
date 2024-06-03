using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _AINameText;
    [SerializeField] private TMP_Text _AIBuyoutValueText;
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

    public void AttemptBuyout()
    {
        PlayerScript.INSTANCE.AttemptBuyout(_trader);
    }
}
