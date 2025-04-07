using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private UpgradeInfo upgradeInfo;
    [SerializeField] private Animator _merchantAnimator;

    [SerializeField] private List<UpgradeContainer> upgradeContainers;

    private void Start()
    {
        UpdateInfo();
    }

    private void Update()
    {
        if(GameManager.Instance.ActionState == PlayerActionState.Shop)
        {
            UpgradeHandle();
        }
       
    }

    private void UpgradeHandle()
    { 
        for (int i = 0; i < upgradeContainers.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            { 
                TryToUpgrade(upgradeContainers[i]);
            }
        }
    }

    private void TryToUpgrade(UpgradeContainer container)
    { 
        var info = upgradeInfo.upgradeDT[container.upgradeType];
        int price = info[container.CurrentLevel + 1].price;
        if (info.Count > container.CurrentLevel + 1 && GameManager.Instance.ScoreManager.Coin>= price)
        {
            _merchantAnimator.SetTrigger("Buy");
            GameManager.Instance.ScoreManager.DecreaseGold(price);
            container.CurrentLevel += 1; 
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        foreach (var upgradeContainer in upgradeContainers)
        {
            var info = upgradeInfo.upgradeDT[upgradeContainer.upgradeType];
            int index = upgradeContainer.CurrentLevel;
            upgradeContainer.currentStatText.text = info[index].currentStat.ToString();
            if(info.Count > index + 1)
            {
                upgradeContainer.nextStatText.text = info[index + 1].currentStat.ToString();
                upgradeContainer.priceText.text = info[index + 1].price.ToString(); 
            }
            else
            {
                upgradeContainer.nextStatText.gameObject.SetActive(false);
                upgradeContainer.priceText.gameObject.SetActive(false);
            }
            switch (upgradeContainer.upgradeType)
            {
                case UpgradeType.Speed:
                    GameManager.Instance.SpeedStat = info[index].currentStat;
                    break;
                case UpgradeType.Movement:
                    GameManager.Instance.MovementStat = info[index].currentStat;
                    break;
                case UpgradeType.Hook:
                    GameManager.Instance.FishPriceStat = info[index].currentStat;
                    break; 
            }
        }
    }
}

[System.Serializable]
public class UpgradeContainer
{
    public UpgradeType upgradeType;
    public TextMeshProUGUI currentStatText;
    public TextMeshProUGUI nextStatText;
    public TextMeshProUGUI priceText;
    public int CurrentLevel { get; set; }
}

public enum UpgradeType
{
    Speed,
    Movement,
    Hook
}
