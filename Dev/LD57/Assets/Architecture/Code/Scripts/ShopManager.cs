using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private UpgradeInfo upgradeInfo;
    [SerializeField] private List<UpgradeContainer> upgradeContainers;
     
}

[System.Serializable]
public struct UpgradeContainer
{
    public UpgradeType upgradeType;
    public TextMeshProUGUI currentStatText;
    public TextMeshProUGUI nextStatText;
    public TextMeshProUGUI priceText;
}

public enum UpgradeType
{
    Speed,
    Movement,
    Hook
}
