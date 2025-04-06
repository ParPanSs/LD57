using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;


public class FishManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<FishId, FishStatus> fishes = new();
    
    [SerializeField] private GameObject fishPrefab;
    private Fish fish;

    public static FishManager Instance;
    
    private void Awake()
    {
        Instance = this;
        if (Instance == null) Destroy(gameObject);
    }

    private void SpawnFish(FishId fishId, int value)
    {
        for (int i = 0; i < value; i++)
        {
            var spawnedFish = Instantiate(fishPrefab, transform.position, Quaternion.identity);
            fish = spawnedFish.GetComponent<Fish>();
            fish.fishStatus = fishes[fishId];
            fish.InitAnimator(fishId);
        }
    }

    public void SpawnRandomFish()
    {
        var randomFishId = fishes.ElementAt(Random.Range(0, fishes.Count));
        SpawnFish(randomFishId.Key, randomFishId.Value.fishAmount);
    }

    public SerializedDictionary<FishId, FishStatus> GetAllFish()
    {
        return fishes;
    }
}
