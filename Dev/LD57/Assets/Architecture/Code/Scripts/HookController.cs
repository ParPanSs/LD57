using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HookController : MonoBehaviour
{
    private const KeyCode LEFT_KEY_CODE = KeyCode.A; 
    private const KeyCode RIGHT_KEY_CODE = KeyCode.D; 

    [SerializeField] private Transform[] _startHookingAnimPos;
    [SerializeField] private SpriteRenderer _activeBait;

    private float _mainSpeed = 25;
    private float _movementSpeed = 0.6f;
    private float _speed;
    private bool _isAnimated;
    private Transform _cameraTransform;

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

    public void StartHooking(float speed)
    {
        _isAnimated = true;
        _speed = speed;
        Vector3[] vectors = new Vector3[_startHookingAnimPos.Length];
        for (int i = 0; i < _startHookingAnimPos.Length; i++)
        {
            vectors[i] = _startHookingAnimPos[i].position;
        }
        LeanTween.moveSpline(gameObject, vectors, 1f).setOnComplete(() => _isAnimated = false);
    }

    public void StartCatching()
    {
        _isAnimated = true;
        LeanTween.moveX(gameObject, transform.position.x + 0.5f, 0.5f).setEaseShake().setOnComplete(() =>
        {
            _isAnimated = false;
        });
    }

    private void StopCatching()
    {
        GameManager.Instance.StopCatching();
        Vector3[] vectors = new Vector3[_startHookingAnimPos.Length];
        for (int i = 0; i < _startHookingAnimPos.Length; i++)
        {
            vectors[i] = _startHookingAnimPos[_startHookingAnimPos.Length - (i + 1)].position;

        }
        LeanTween.moveSpline(gameObject, vectors, 0.8f);
    }

    private void Update()
    {
        if (!_isAnimated)
        {
            if (GameManager.Instance.ActionState == PlayerActionState.Hooking)
            {
                Vector3 moveDirection = Vector3.down + MovementHandle();
                _speed -= Time.deltaTime * 0.1f;
                transform.position += moveDirection * Time.deltaTime * _speed * _mainSpeed;
                _cameraTransform.position += moveDirection * Time.deltaTime * _speed * _mainSpeed;
                if (_speed <= 0)
                {
                    GameManager.Instance.StartCatching();
                }
            }
            else if (GameManager.Instance.ActionState == PlayerActionState.Catching)
            {
                if (transform.position.y >= -0.5f)
                {
                    StopCatching();
                }
                Vector3 direction = (Vector3.zero - transform.position).normalized;

                transform.position += direction * _mainSpeed * Time.deltaTime; ;
                _cameraTransform.position += direction * _mainSpeed * Time.deltaTime; ;
            }


        }
    }

    private Vector3 MovementHandle()
    {
        if (Input.GetKey(LEFT_KEY_CODE) || Input.GetKey(KeyCode.LeftArrow))
        {
            return Vector3.left * _movementSpeed;
        }
        if (Input.GetKey(RIGHT_KEY_CODE) || Input.GetKey(KeyCode.RightArrow))
        {
            return Vector3.right * _movementSpeed;
        }
        return Vector3.zero;
    }
}
