using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private List<FishSpawnPointContainer> _spawnContainers;
    private float _timer;

    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 60)
        {
            _timer = 0;
            Spawn();
        }
    }

    private void Spawn()
    {
        foreach (var item in _spawnContainers)
        {
            item.TryToSpawn();
        }
    }

    [System.Serializable]
    private class FishSpawnPointContainer
    {
        public Transform minY_Point;
        public Transform maxY_Point;
        private List<Fish> _spawnedFish = new();


        public void TryToSpawn()
        {
            bool canSpawn = true;
            foreach (var item in _spawnedFish)
            {
                if (item != null)
                {
                    canSpawn = false;
                }
            }
            if (canSpawn)
            {
                _spawnedFish = GameManager.Instance.FishManager.SpawnRandomFish(minY_Point.position.y, maxY_Point.position.y);
            }
        }
    }
}
