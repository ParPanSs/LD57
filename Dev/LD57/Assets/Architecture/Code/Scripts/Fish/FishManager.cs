using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.U2D;


public class FishManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<FishId, FishStatus> fishes = new();
    [SerializeField] private float spawnRadius;
    [SerializeField] private Fish fishPrefab;
    private Fish fish;
    private float _timer = 5;
    private Camera _mainCamera;
    private Vector2 spawnPoint;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public List<Fish> SpawnFish(FishId fishId, int value, float minY, float maxY)
    {
        List<Fish> spawnedFish = new();
        spawnPoint = new Vector2(Random.Range(-15, 15), Random.Range(minY, maxY));
        for (int i = 0; i < value; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            fish = Instantiate(fishPrefab, spawnPoint + randomOffset, Quaternion.identity);
            fish.fishStatus = fishes[fishId];
            fish.InitAnimator(fishId);
            spawnedFish.Add(fish);
        }
        return spawnedFish;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? spawnPoint : transform.position, spawnRadius);
    }

    public List<Fish> SpawnRandomFish(float minY, float maxY)
    {
        Dictionary<FishId, FishStatus> fishList = new(fishes);
        if (!GameManager.isEndlessMode)
        {
            fishList.Remove(FishId.Angler);
            foreach (var fish in fishList.ToList())
            {
                if (fish.Value.maxYPos < minY)
                {
                    fishList.Remove(fish.Key);
                }
            }
        }
        var randomFishId = fishList.ElementAt(Random.Range(0, fishList.Count));
        return SpawnFish(randomFishId.Key, randomFishId.Value.fishAmount, minY, maxY);
    }

    public SerializedDictionary<FishId, FishStatus> GetAllFish()
    {
        return fishes;
    }
}
