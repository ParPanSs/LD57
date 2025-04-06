using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FishParameters
{
    public List<Animator> animators;
}

public class Fish : MonoBehaviour
{
    [SerializeField] private float wanderRadius;
    [SerializeField] private float waitTime;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private FishParameters parameters;
    
    private Collider2D _collider;
    private Vector2 _startPosition;
    private Vector2 _fromPosition;
    private Vector2 _targetPosition;
    private float _timer;
    private bool _isMoving;
    private bool _isWaiting;

    public FishStatus fishStatus;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    
    private void Start()
    {
        _startPosition = transform.position;
        BeginNewMovement();
    }

    private void Update()
    {
        if (_isWaiting)
        {
            _timer += Time.deltaTime;
            if (_timer >= waitTime)
            {
                _isWaiting = false;
                BeginNewMovement();
            }
            return;
        }

        if (_isMoving)
        {
            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / moveDuration);
            float curveT = speedCurve.Evaluate(t);

            transform.position = Vector2.Lerp(_fromPosition, _targetPosition, curveT);

            Vector2 direction = _targetPosition - _fromPosition;
            
            if (t >= 1f)
            {
                _isMoving = false;
                _isWaiting = true;
                _timer = 0f;
            }
        }
    }

    private void BeginNewMovement()
    {
        _fromPosition = transform.position;
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        _targetPosition = _startPosition + randomOffset;

        _timer = 0f;
        _isMoving = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? _startPosition : transform.position, wanderRadius);
    }
}
