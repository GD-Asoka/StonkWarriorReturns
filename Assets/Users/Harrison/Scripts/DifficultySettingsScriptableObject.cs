using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty Settings", menuName = "Scriptable Objects/Difficulty Settings")]
public class DifficultySettingsScriptableObject : ScriptableObject
{
    public float playerBuyoutMod = 2.25f;
    public float enemyBuyoutMod = 1.75f;
}
