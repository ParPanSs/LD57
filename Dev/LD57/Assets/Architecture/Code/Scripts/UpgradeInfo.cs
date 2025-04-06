using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UpgradeInfo : ScriptableObject
{
    public SerializedDictionary<UpgradeType, List<UpgradeInfoContainer>> upgradeDT;


    [System.Serializable]
    public struct UpgradeInfoContainer
    {
        public float currentStat;
        public int price;
    }
}
