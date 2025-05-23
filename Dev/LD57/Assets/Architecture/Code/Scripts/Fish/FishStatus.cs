using Sirenix.OdinInspector;
using UnityEngine;

public enum FishId
{
    Angler,
    Carp,
    Sardine,
    GoldenFish,
    SiameseFish,
}

[CreateAssetMenu(fileName = "Fish", menuName = "Fish/New Fish")]
public class FishStatus : ScriptableObject
{
    public FishId fishId;
    public float fishSpeed;
    [Range(1, 7)]
    public int fishAmount;
    public bool needBait;
    [ShowIf("needBait")]
    public BaitId baitId;
    public float maxYPos;
    public int goldReward;
    public int scoreReward;
}
