using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stock", menuName = "Scriptable Objects/Stock")]
public class StocksScriptableObject : ScriptableObject
{
    public string stockName = "Stock Name";
    public string acronym = "";
    public Vector2 startingValueRange = new Vector2(1, 1);
    public Vector2 minMaxDefaultTrend = new Vector2(-1, 1);
}
