using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct FishParameters
{
    public SerializedDictionary<FishId, Animator> animators;
}

public class Fish : Catchable
{
    [SerializeField] private bool initOnStart;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float waitTime;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private FishParameters fishParameters;
    private Collider2D _collider;
    private Vector2 _startPosition;
    private Vector2 _startScale;
    private Vector2 _fromPosition;
    private Vector2 _targetPosition;
    private float _timer;
    private float _currentCooldown = 2;
    private float _detectCooldown = 2f;
    private ActionType _actionType;
    private HookController _detectedHook;
    public FishStatus fishStatus;
    
    private Vector3 _targetScale;
    private Quaternion _startRotation;
    private float _rotationDuration = 0.5f;
    private float _rotationTimer = 0f;
    private bool _isRotating = false;

    [SerializeField] private UnityEvent OnCatch;

    private void Awake()
    {
        foreach (var item in fishParameters.animators)
        {
            item.Value.gameObject.SetActive(false);
        }
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _actionType = ActionType.Waiting;
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _startScale = transform.localScale;
        if (initOnStart)
        {
            InitAnimator(fishStatus.fishId);
        }
        BeginNewMovement();
    }

    public void InitAnimator(FishId fishId)
    {
        fishParameters.animators[fishId].gameObject.SetActive(true);
    }

    private void Update()
    {
        switch (_actionType)
        {
            case ActionType.Waiting:
                fishParameters.animators[fishStatus.fishId].SetBool("IsSwimming", false);
                _timer += Time.deltaTime;
                if (_timer >= waitTime)
                {
                    BeginNewMovement();
                }
                return;
            case ActionType.Moving:
                _currentCooldown += Time.deltaTime;
                fishParameters.animators[fishStatus.fishId].SetBool("IsSwimming", true);
                _timer += Time.deltaTime;
                float t = Mathf.Clamp01(_timer / moveDuration);
                float curveT = speedCurve.Evaluate(t);

                Vector2 direction = (_targetPosition - (Vector2)transform.position).normalized;

                /*if (Mathf.Abs(direction.x) > 0.01f)
                {
                    float signOfDirection = Mathf.Sign(direction.x) * -1;
                    transform.localScale = new Vector3(_startScale.x * signOfDirection, transform.localScale.y, transform.localScale.z);
                }*/
                if (Mathf.Abs(direction.x) > 0.01f)
                {
                    float signOfDirection = Mathf.Sign(direction.x);
                    float targetYAngle = signOfDirection < 0f ? 1f : -1f;
                    _targetScale = new Vector3(targetYAngle,1,1);
                    _rotationTimer = 0f;
                    _isRotating = true;
                }
                
                if (_isRotating)
                {
                    _rotationTimer += Time.deltaTime;
                    float d = Mathf.Clamp01(_rotationTimer / _rotationDuration);
                    transform.localScale = Vector3.Slerp(transform.localScale, _targetScale, d);

                    if (d >= 1f)
                    {
                        _isRotating = false;
                    }
                }

                transform.position = Vector2.Lerp(_fromPosition, _targetPosition, curveT);

                if (Vector2.Distance(transform.position, _targetPosition) < 0.01f)
                {
                    transform.position = _targetPosition;
                    _actionType = ActionType.Waiting;
                    _timer = 0f;
                }

                break;
            case ActionType.Detected:
                if (_detectedHook.isDetected && fishStatus.fishId == FishId.GoldenFish)
                {
                    UndetectHook();
                    return;
                }
                if (_detectedHook.transform.position.y > -2 || _detectedHook.IsCatched())
                {
                    UndetectHook();
                    return;
                }
                Vector3 toHookDirection = (_detectedHook.transform.position - transform.position).normalized;
                float sign = Mathf.Sign(toHookDirection.x) * -1;
                transform.localScale = new Vector3(_startScale.x * sign, transform.localScale.y, transform.localScale.z);
                transform.position += toHookDirection * 10 * Time.deltaTime;
                break;
            default:
                break;
        }
    }

    private void BeginNewMovement()
    {
        for (int i = 0; i < 10; i++) 
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            Vector2 potentialPosition = _startPosition + randomOffset;

            if (Vector2.Distance(potentialPosition, _startPosition) <= wanderRadius)
            {
                _targetPosition = potentialPosition;
                break;
            }
        }

        Vector2 direction = _targetPosition - (Vector2)transform.position;
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            float sign = Mathf.Sign(direction.x);
            _targetScale = new Vector3(sign < 0f ? -0 : 1f, 1f, 1);
        }

        _fromPosition = transform.position;
        _timer = 0f;
        _actionType = ActionType.Moving;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? _startPosition : transform.position, wanderRadius);
    }

    public bool TryToHook(BaitId baitId)
    {
        if (_actionType == ActionType.Hooked || _actionType == ActionType.Hitting )
        {
            return false;
        }
        if (fishStatus.needBait && baitId == fishStatus.baitId || fishStatus.baitId == BaitId.Empty)
        {
            //transform.localScale = _startRotation;
            fishParameters.animators[fishStatus.fishId].SetTrigger("OnTheHook");
            _actionType = ActionType.Hooked;
            float sign = Mathf.Sign(transform.localScale.x); 
            LeanTween.rotateZ(gameObject, 70 * -sign, 1).setDelay(0.3f);
            Vector3 newPos = new Vector3(0, 0, 0);
            LeanTween.moveLocal(gameObject, newPos, 1).setDelay(0.3f);
            OnCatch.Invoke();
            return true;
        }
        else
        {
            fishParameters.animators[fishStatus.fishId].SetTrigger("Hit");
                _actionType = ActionType.Hitting;
            LeanTween.delayedCall(0.7f, () =>
            { 
                _actionType = ActionType.Waiting;
                _currentCooldown = 0;
            });
            return false;
        }
    }

    public void DetectHook(HookController hook)
    {
        if (_currentCooldown > _detectCooldown)
        {
            transform.rotation = _startRotation; 
            _detectedHook = hook;
            fishParameters.animators[fishStatus.fishId].SetTrigger("IsBaited");
            _actionType = ActionType.Detected;
        }
        _detectedHook.isDetected = true;
    }

    public void UndetectHook()
    {
        if (_actionType == ActionType.Detected)
        {
            _detectedHook = null;
            fishParameters.animators[fishStatus.fishId].SetTrigger("Idle");
            _actionType = ActionType.Waiting;
        }
    }

    private enum ActionType
    {
        Waiting,
        Moving,
        Hooked,
        Detected,
        Hitting,
    }
}
