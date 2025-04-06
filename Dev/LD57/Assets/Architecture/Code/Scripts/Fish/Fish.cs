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
    private Transform _detectedTransform;
    public FishStatus fishStatus;

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
        _startPosition = transform.position;
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
                float signOfDirection = Mathf.Sign(direction.x) * -1;
                transform.localScale = new Vector3(_startScale.x * signOfDirection, transform.localScale.y, transform.localScale.z);
                transform.position = Vector2.Lerp(_fromPosition, _targetPosition, curveT);
                 

                if (t >= 1f)
                {
                    _actionType = ActionType.Waiting;
                    _timer = 0f;
                }
                break;
            case ActionType.Detected:
                if (_detectedTransform.position.y > -2)
                {
                    UndetectHook();
                    return;
                }
                Vector3 toHookDirection = (_detectedTransform.position - transform.position).normalized;
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
        _fromPosition = transform.position;
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        _targetPosition = _startPosition + randomOffset;

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
        if(_actionType == ActionType.Hooked || _actionType == ActionType.Hitting )
        {
            return false;
        }
        if (fishStatus.needBait && baitId == fishStatus.baitId || fishStatus.baitId == BaitId.Empty)
        {
            fishParameters.animators[fishStatus.fishId].SetTrigger("OnTheHook");
            _actionType = ActionType.Hooked;
            float sign = Mathf.Sign(transform.localScale.x); 
            LeanTween.rotateZ(gameObject, 70 * -sign, 1).setDelay(0.3f);
            Vector3 newPos = new Vector3(1.5f * sign, -2, 0);
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

    public void DetectHook(Transform transform)
    {
        if (_currentCooldown > _detectCooldown)
        {
            _detectedTransform = transform;
            fishParameters.animators[fishStatus.fishId].SetTrigger("IsBaited");
            _actionType = ActionType.Detected;
        }
    }

    public void UndetectHook()
    {
        if (_actionType == ActionType.Detected)
        { 
            _detectedTransform = null;
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
