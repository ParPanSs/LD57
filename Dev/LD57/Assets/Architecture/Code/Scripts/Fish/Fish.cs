using UnityEngine;

public class Fish : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;

    public FishStatus fishStatus;
    
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = fishStatus.fishSprite;
    }
}
