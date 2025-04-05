using UnityEngine;

public class Fish : MonoBehaviour
{
    //[SerializeField] private Collider2D _fishZone;
    [SerializeField] private float wanderRadius = 5f;     // Радиус области, по которой NPC может ходить
    [SerializeField] private float moveSpeed = 2f;        // Скорость движения
    [SerializeField] private float waitTime = 2f;         // Время ожидания перед следующим движением

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    
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
        startPosition = transform.position;
        ChooseNewTarget();
    }

    private void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                ChooseNewTarget();
            }
            return;
        }

        // Двигаем NPC к цели
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Если достиг цели
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }
    
    private void ChooseNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + randomOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, wanderRadius);
    }
    
    
}
