using UnityEngine;

public class HookController : MonoBehaviour
{
    private const KeyCode LEFT_KEY_CODE = KeyCode.A;
    private const KeyCode RIGHT_KEY_CODE = KeyCode.D;
    private const string HOOK_TRIGGER = "Hook";
    private const string CATCH_TRIGGER = "Catch";

    [SerializeField] private Transform[] _startHookingAnimPos;
    [SerializeField] private SpriteRenderer _activeBait;
    [SerializeField] private GameObject _hookingHint;
    [SerializeField] private Animator _playerAnimator;

    private Vector3 _startCatchPosition;
    private Vector3 _startLocalPosition;
    private Quaternion _startRotation;
    private float _mainSpeed = 25;
    private float _movementSpeed = 0.6f;
    private float _xFactor = 0f;
    private float _shakingTime = 0.3f;
    private float _speed;
    private bool _isAnimated; 
    private Transform _cameraTransform;
    private Catchable _catchedObject;
    private BaitId _baitId;
    public BaitId BaitId => _baitId;

    private void Start()
    {
        _startLocalPosition = transform.localPosition; 
        _baitId = BaitId.Empty;
        SetBait(_baitId);
        _cameraTransform = Camera.main.transform;
    }

    public void SetBait(BaitId baitId)
    {
        _baitId = baitId;
        _activeBait.sprite = GameManager.Instance.BaitManager.GetBaitSprite(baitId);
    }

    public bool IsCatched()
    {
        return _catchedObject != null;
    }

    public Catchable GetCatchedObject()
    {
        return _catchedObject;
    }
    [SerializeField]
    float _cameraParentOffset;
    public void StartHooking(float speed)
    {
        _isAnimated = true;
        _speed = speed;
        Vector3[] vectors = new Vector3[_startHookingAnimPos.Length];
        for (int i = 0; i < _startHookingAnimPos.Length; i++)
        {
            vectors[i] = _startHookingAnimPos[i].position;
        }
        _playerAnimator.SetTrigger(HOOK_TRIGGER);
        LeanTween.delayedCall(0.4f, () =>
        { 
            LeanTween.moveY(_cameraTransform.parent.gameObject, _cameraTransform.parent.position.y + _cameraParentOffset, 0.5f).setOnComplete(()=>
            {  
                });

            _startCatchPosition = transform.position;
            transform.rotation = Quaternion.Euler(Vector3.forward * -110);
            _isAnimated = false;
            _hookingHint.SetActive(true);
        });
    }

    public void StartCatching()
    {
        _hookingHint.SetActive(false);
        _xFactor = 0;
        _isAnimated = true;
        LeanTween.moveX(gameObject, transform.position.x + 0.5f, _shakingTime).setEaseShake().setOnComplete(() =>
        {
            _isAnimated = false;
        });
    }
    private void ReturnCamera()
    { 
        LeanTween.moveY(_cameraTransform.parent.gameObject, 0, 0.3f); 
    }
    private void StopCatching()
    {
        GameManager.Instance.StopCatching(_catchedObject);
        transform.localPosition = _startLocalPosition;
        transform.localRotation = Quaternion.Euler(Vector3.forward * -24.38f);

        if (_catchedObject != null)
        {
            Destroy(_catchedObject.gameObject);
            _catchedObject = null;
        }
        Vector3[] vectors = new Vector3[_startHookingAnimPos.Length];
        for (int i = 0; i < _startHookingAnimPos.Length; i++)
        {
            vectors[i] = _startHookingAnimPos[_startHookingAnimPos.Length - (i + 1)].position;

        }
        _playerAnimator.SetTrigger(CATCH_TRIGGER);

        // LeanTween.moveSpline(gameObject, vectors, 0.8f);
    }

    private void Update()
    {
        if (!_isAnimated)
        {
            GameManager.Instance.SetLowpassForMixers(transform.position.y);
            if (GameManager.Instance.ActionState == PlayerActionState.Hooking)
            {
                Vector3 moveDirection = Vector3.down + MovementHandle();
                moveDirection.x += _xFactor;

                _xFactor -= Time.deltaTime * (Mathf.Sign(_xFactor) * 0.5f);
                _speed -= Time.deltaTime * 0.05f;

                var newPosition = transform.position;
                newPosition += moveDirection * Time.deltaTime * _speed * _mainSpeed * GameManager.Instance.SpeedStat;
                newPosition.x = Mathf.Clamp(newPosition.x, -15, 15);
                
                {
                    _cameraTransform.localPosition = Vector3.up * transform.position.y;
                }
                transform.position = newPosition;
                //_cameraTransform.position += (moveDirection+Vector3.down*5) * Time.deltaTime * _speed * _mainSpeed * GameManager.Instance.SpeedStat;
                if (_speed <= 0)
                {
                    GameManager.Instance.StartCatching();
                }
            }
            else if (GameManager.Instance.ActionState == PlayerActionState.Catching)
            {
                Vector3 direction = (_startCatchPosition - transform.position).normalized;
                var deepFactor = -((transform.position.y / 20));
                deepFactor = Mathf.Max(1f, deepFactor);
                transform.position += direction * (deepFactor * _mainSpeed) * Time.deltaTime;
                 
                {
                    _cameraTransform.localPosition += direction * (deepFactor * _mainSpeed) * Time.deltaTime;
                }

                if (transform.position.y >= _startCatchPosition.y - 5)
                {
                    ReturnCamera();
                }

                if (transform.position.y >= _startCatchPosition.y - 1)
                {
                    StopCatching();
                    _cameraTransform.localPosition = Vector3.zero;
                }
            }
        }
    }

    private Vector3 MovementHandle()
    {
        if (Input.GetKey(LEFT_KEY_CODE) || Input.GetKey(KeyCode.LeftArrow))
        {
            return Vector3.left * _movementSpeed * GameManager.Instance.MovementStat;
        }
        if (Input.GetKey(RIGHT_KEY_CODE) || Input.GetKey(KeyCode.RightArrow))
        {
            return Vector3.right * _movementSpeed * GameManager.Instance.MovementStat;
        }
        return Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_catchedObject == null && collision.gameObject.TryGetComponent<Catchable>(out Catchable catchable))
        {
            bool canCatch = true;
            Vector3 pos = Vector3.zero;
            _shakingTime = catchable.ShakingTime;
            Transform parent = transform;
            if (catchable.CatchableType == CatchableType.Fish)
            {
                canCatch = (catchable as Fish).TryToHook(_baitId);
                float sign = Mathf.Sign(catchable.transform.localScale.x);
                //pos.x = 2 * sign;
            }
            parent = _activeBait.transform;
            if (canCatch)
            {
                _catchedObject = catchable;
                 
                    catchable.OnHook(); 
                _catchedObject.transform.SetParent(parent);
                _catchedObject.transform.localPosition = pos;
            }
            GameManager.Instance.StartCatching();
        }
        else if (collision.gameObject.TryGetComponent<Detectable>(out Detectable detectable))
        {
            OnDetected(detectable.DetectableType);
        }
    }

    private void OnDetected(DetectableType detectableType)
    {
        if (GameManager.Instance.ActionState != PlayerActionState.Hooking)
        {
            return;
        }
        switch (detectableType)
        {
            case DetectableType.Obstacle:
                GameManager.Instance.StartCatching();
                break;
            case DetectableType.Speedup:
                _speed += 0.15f;
                break;
            case DetectableType.Slowdown:
                _speed -= 0.15f;
                break;
            case DetectableType.MoveLeft:
                _xFactor -= 0.6f;
                break;
            case DetectableType.MoveRight:
                _xFactor += 0.6f;
                break;
            default:
                break;
        }
    }
}
