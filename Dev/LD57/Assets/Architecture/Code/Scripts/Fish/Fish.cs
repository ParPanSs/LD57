using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public struct FishParameters
{
    public SerializedDictionary<FishId, Animator> animators;
}

public class Fish : Catchable
{
    [SerializeField] private float wanderRadius;
    [SerializeField] private float waitTime;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private FishParameters fishParameters;
    
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

    public void InitAnimator(FishId fishId)
    {
        fishParameters.animators[FishId.Carp].enabled = true;
    }

    private void Update()
    {
        if (_isWaiting)
        {
            fishParameters.animators[FishId.Carp].SetBool("isSwimming", false);
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
            fishParameters.animators[FishId.Carp].SetBool("isSwimming", true);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out HookController hookController))
        {
            fishParameters.animators[FishId.Carp].SetBool("OnTheHook", true);
        }
    }
}
