using System.Collections.Generic;
using UnityEngine;

public class TailChecker : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> tailsToSpawn;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tile"))
        {
            if (GameManager.Instance.Tails[GameManager.Instance.Tails.Count-1] == other.gameObject)
            {
                var parent = other.transform.parent;
                Vector2 spawnPosition = new(other.transform.position.x, other.transform.position.y - other.bounds.size.y);
                var tail = Instantiate(tailsToSpawn[Random.Range(0, tailsToSpawn.Count)],
                    spawnPosition, Quaternion.identity);
                tail.transform.SetParent(parent);
                GameManager.Instance.Tails.Add(tail);
            }
        }
    }    
}
