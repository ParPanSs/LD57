using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    private const KeyCode LEFT_KEY_CODE = KeyCode.A;// & KeyCode.LeftArrow;
    private const KeyCode RIGHT_KEY_CODE = KeyCode.D;// & KeyCode.RightArrow;

    [SerializeField]
    private Transform[] _startHookingAnimPos;

    [SerializeField]
    private float _mainSpeed;

    private float _speed;
    private float _catchSpeed = 1;
    [SerializeField]
    private float _movementSpeed = 1;
    private bool _isAnimated;
    private Transform _cameraTransform;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
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
        LeanTween.moveX(gameObject, transform.position.x + 0.5f, 0.5f).setEaseShake().setOnComplete(() => _isAnimated = false);
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

    public float lerpSpeed;
    private void Update()
    {
        if (!_isAnimated)
        {
            Vector3 moveDirection = Vector3.zero;
            if (GameManager.Instance.ActionType == PlayerActionType.Hooking)
            { 
                moveDirection = Vector3.down + MovementHandle();
                _speed -= Time.deltaTime * 0.1f;
                if (_speed <= 0)
                {
                    GameManager.Instance.StartCatching();
                }
                transform.position += moveDirection * Time.deltaTime * _speed * _mainSpeed;
                _cameraTransform.position += moveDirection * Time.deltaTime * _speed * _mainSpeed;
            }
            else if (GameManager.Instance.ActionType == PlayerActionType.Catching)
            {
                /*moveDirection = Vector3.up + (Vector3.left * (Mathf.Sign(transform.position.x)* 0.7f));
                _speed = _catchSpeed;*/
                if (transform.position.y >= -0.5f)
                {
                    StopCatching();
                }
                Vector3 lerp = Vector3.Lerp(transform.position, Vector3.zero, lerpSpeed*Time.deltaTime);
                transform.position = lerp;
                _cameraTransform.position = lerp;
            }

            
        }
    }

    private Vector3 MovementHandle()
    {
        if (Input.GetKey(LEFT_KEY_CODE))
        {
            return Vector3.left * _movementSpeed;
        }
        if (Input.GetKey(RIGHT_KEY_CODE))
        {
            return Vector3.right * _movementSpeed;
        }
        return Vector3.zero;
    }
}
