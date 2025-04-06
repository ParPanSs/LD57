using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;


public class FishManager : MonoBehaviour
{
    public static FishManager Instance;
    [SerializeField] private SerializedDictionary<FishId, FishStatus> fishes = new();
    [SerializeField] private float spawnRadius;
    [SerializeField] private GameObject fishPrefab;
    private Fish fish;
    private float _timer = 5;
    private Camera _mainCamera;
    private Vector2 spawnPoint;

    private void Awake()
    {
        Instance = this;
        if (Instance == null) Destroy(gameObject);
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        if (_timer >= 5)
        {
            _timer = 0;
            SpawnRandomFish();
        }
        _timer += Time.deltaTime;
    }

    public void SpawnFish(FishId fishId, int value)
    {
        Vector2 randomOffset;
        spawnPoint = new Vector2(Random.Range(-15, 15), _mainCamera.transform.position.y - 25);
        for (int i = 0; i < value; i++)
        {
            if (fishId == FishId.Sardine)
            {
                randomOffset = Random.insideUnitCircle * spawnRadius;
                var sardineFish = Instantiate(fishPrefab, spawnPoint + randomOffset, Quaternion.identity);
                fish = sardineFish.GetComponent<Fish>();
                fish.fishStatus = fishes[fishId];
                fish.InitAnimator(fishId);
                continue;
            }
            var spawnedFish = Instantiate(fishPrefab, spawnPoint, Quaternion.identity);
            fish = spawnedFish.GetComponent<Fish>();
            fish.fishStatus = fishes[fishId];
            fish.InitAnimator(fishId);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? spawnPoint : transform.position, spawnRadius);
    }

    public void SpawnRandomFish()
    {
        var randomFishId = fishes.ElementAt(Random.Range(0, fishes.Count));
        if (!GameManager.isAnglerCatched && randomFishId.Key == FishId.Angler)
        {
            SpawnRandomFish();
            return;
        }
        SpawnFish(randomFishId.Key, randomFishId.Value.fishAmount);
    }

    public SerializedDictionary<FishId, FishStatus> GetAllFish()
    {
        return fishes;
    }
}
