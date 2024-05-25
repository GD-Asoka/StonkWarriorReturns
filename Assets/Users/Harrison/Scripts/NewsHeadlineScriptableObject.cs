using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "News Headline", menuName = "Scriptable Objects/News Headline")]
public class NewsHeadlineScriptableObject : ScriptableObject
{
    public string newsHeadline = "Headline";
    public string flavourText = "This is a Description";
    public struct Effect
    {
        public float minGainLoss;
        public float maxGainLoss;
    }

    public float minEffectTime;
    public float maxEffectTime;
    public SerializedDictionary<StocksScriptableObject, Vector2> stockEffects;
}
