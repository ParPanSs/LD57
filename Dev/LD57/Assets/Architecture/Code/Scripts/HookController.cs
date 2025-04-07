using UnityEngine;

public class HookController : MonoBehaviour
{
    private const KeyCode LEFT_KEY_CODE = KeyCode.A;
    private const KeyCode RIGHT_KEY_CODE = KeyCode.D;

    [SerializeField] private Transform[] _startHookingAnimPos;
    [SerializeField] private SpriteRenderer _activeBait;
    [SerializeField] private GameObject _hookingHint;

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

    public void StartHooking(float speed)
    {
        _isAnimated = true;
        _speed = speed;
        Vector3[] vectors = new Vector3[_startHookingAnimPos.Length];
        for (int i = 0; i < _startHookingAnimPos.Length; i++)
        {
            vectors[i] = _startHookingAnimPos[i].position;
        }
       //  LeanTween.moveSpline(gameObject, vectors, 1f).setOnComplete(() =>
        { 
            _isAnimated = false;
            _hookingHint.SetActive(true);
         }//);
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

    private void StopCatching()
    {
        GameManager.Instance.StopCatching(_catchedObject);
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
       // LeanTween.moveSpline(gameObject, vectors, 0.8f);
    }

    private void Update()
    {
        if (!_isAnimated)
        {
            if (GameManager.Instance.ActionState == PlayerActionState.Hooking)
            {
                Vector3 moveDirection = Vector3.down + MovementHandle();
                moveDirection.x += _xFactor;

                _xFactor -= Time.deltaTime * (Mathf.Sign(_xFactor) * 0.5f);
                _speed -= Time.deltaTime * 0.05f;

                var newPosition = transform.position;
                newPosition += moveDirection * Time.deltaTime * _speed * _mainSpeed * GameManager.Instance.SpeedStat;
                newPosition.x = Mathf.Clamp(newPosition.x, -15, 15);
                transform.position = newPosition;
                _cameraTransform.position += moveDirection * Time.deltaTime * _speed * _mainSpeed * GameManager.Instance.SpeedStat;
                if (_speed <= 0)
                {
                    GameManager.Instance.StartCatching();
                }
            }
            else if (GameManager.Instance.ActionState == PlayerActionState.Catching)
            {
                Vector3 direction = (Vector3.zero - transform.position).normalized;
                var deepFactor = -((transform.position.y / 20));
                deepFactor = Mathf.Max(1f, deepFactor);
                transform.position += direction * (deepFactor * _mainSpeed) * Time.deltaTime;
                _cameraTransform.position += direction * (deepFactor * _mainSpeed) * Time.deltaTime;
                if (_cameraTransform.position.y > 0)
                {
                    _cameraTransform.position = Vector3.zero;
                }
                  if (transform.position.y >= -0.5f)
                {
                    StopCatching();
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
