using UnityEngine;

public class TailChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tile"))
        {
            if (GameManager.Instance.Tails.FindLast((x => x == other.gameObject)))
            {
                var parent = other.transform.parent;
                Vector2 spawnPosition = new(other.transform.position.x, other.transform.position.y - other.bounds.size.y);
                var tail = Instantiate(GameManager.Instance.Tails[Random.Range(0, GameManager.Instance.Tails.Count)],
                    spawnPosition, Quaternion.identity);
                tail.transform.SetParent(parent);
                GameManager.Instance.Tails.Add(tail);
            }
        }
    }    
}
