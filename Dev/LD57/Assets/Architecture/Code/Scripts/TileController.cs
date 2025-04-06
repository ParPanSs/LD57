using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private List<FishSpawnPointContainer> _spawnContainers;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        foreach (var item in _spawnContainers)
        {
            if(_timer%item.time == 0)
            {
                item.TryToSpawn();
            }
        }
    }


    [System.Serializable]
    private class FishSpawnPointContainer
    {
        public Transform minY_Point;
        public Transform maxY_Point;
        [SerializeField] private float minTime;
        [SerializeField] private float maxTime;
        public float time;
        private List<Fish> _spawnedFish = new();

        
        public void TryToSpawn()
        {
            if (_spawnedFish.Count>0)
                return;
            _spawnedFish = GameManager.Instance.FishManager.SpawnRandomFish(minY_Point.position.y, maxY_Point.position.y);
            time = Random.Range(minTime, maxTime);
        }
    }
}
