using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private float wanderRadius;
    [SerializeField] private float waitTime;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve speedCurve;
    
    private SpriteRenderer _spriteRenderer;
    private Vector2 startPosition;
    private Vector2 fromPosition;
    private Vector2 targetPosition;
    private float timer;
    private bool isMoving;
    private bool isWaiting;

    public FishStatus fishStatus;
    
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (fishStatus != null)
            _spriteRenderer.sprite = fishStatus.fishSprite;

        startPosition = transform.position;
        BeginNewMovement();
    }

    private void Update()
    {
        if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                isWaiting = false;
                BeginNewMovement();
            }

            return;
        }

        if (isMoving)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / moveDuration);
            float curveT = speedCurve.Evaluate(t);

            transform.position = Vector2.Lerp(fromPosition, targetPosition, curveT);

            Vector2 direction = targetPosition - fromPosition;
            if (direction.x != 0)
                _spriteRenderer.flipX = direction.x < 0;

            if (t >= 1f)
            {
                isMoving = false;
                isWaiting = true;
                timer = 0f;
            }
        }
    }

    private void BeginNewMovement()
    {
        fromPosition = transform.position;
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + randomOffset;

        timer = 0f;
        isMoving = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, wanderRadius);
    }
}
