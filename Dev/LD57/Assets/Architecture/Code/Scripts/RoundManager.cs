using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> fishSprites;
    
    [SerializeField] private Image fishImage;
    private FishId _fishId;

    private void Start()
    {
        SetNewFish();
    }
    
    public void SetNewFish()
    {
        var fishNumber = Random.Range(0, fishSprites.Count);
        fishImage.sprite = fishSprites[fishNumber];
        _fishId = GameManager.Instance.FishManager.GetAllFish().ElementAt(fishNumber).Key;
    }

    public void CheckFish(FishId fishId)
    {
        if (fishId != _fishId)
        {
            GameManager.Instance.Lose();
            return;
        }
    }

    public bool RightFish(FishId fishId)
    {
        return fishId == _fishId;
    }
}
