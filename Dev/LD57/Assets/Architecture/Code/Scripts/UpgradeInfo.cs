using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UpgradeInfo : ScriptableObject
{
    public SerializedDictionary<UpgradeType, List<UpgradeInfoContainer>> upgradeInfo;


    [System.Serializable]
    public struct UpgradeInfoContainer
    {
        public int nextStat;
        public int price;
    }
}
