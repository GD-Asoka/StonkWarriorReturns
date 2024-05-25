using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestUIDisplayPlayerMoney : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }


    // Update is called once per frame
    void Update()
    {
        _text.text = $"Money: ${PlayerScript.INSTANCE.money}";
    }
}
