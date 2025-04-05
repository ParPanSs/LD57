using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const KeyCode CANCEL_KEY_CODE = KeyCode.Escape;
    private const KeyCode HOOK_KEY_CODE = KeyCode.E;
    private const KeyCode CATCH_KEY_CODE = KeyCode.Space;

    [SerializeField]
    private float _aimingSpeed;
    [SerializeField]
    private HookController _hookController;
    [SerializeField]
    private Slider _aimingSlider;

    private PlayerActionType _actionType;
    public PlayerActionType ActionType => _actionType;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(_actionType == PlayerActionType.Aiming)
        {
            Aiming();
        }

        Handle();
    }

    private void Handle()
    {
        if (Input.GetKeyDown(HOOK_KEY_CODE))
        {
            HookHandle();
        }
        if (Input.GetKeyDown(CANCEL_KEY_CODE))
        {
            CancelHandle();
        }
        if (Input.GetKeyDown(CATCH_KEY_CODE))
        {
            CatchHandle();
        }
    }

    #region Hook Methods
    private void HookHandle()
    {
        if (_actionType == PlayerActionType.Idle)
        {
            StartAiming();
        }
        else if (_actionType == PlayerActionType.Aiming)
        {
            StartHooking();
        }
    }

    private void StartAiming()
    {
        _aimingSlider.gameObject.SetActive(true);
        _actionType = PlayerActionType.Aiming;
        _aimingSlider.value = _aimingSlider.minValue; 
    } 
    private void Aiming()
    {
        float t = Mathf.PingPong(Time.time * _aimingSpeed, 1f);
        _aimingSlider.value = Mathf.Lerp(_aimingSlider.minValue, _aimingSlider.maxValue, t);
    }

    private void StartHooking()
    {
        _aimingSlider.gameObject.SetActive(false); 
        _actionType = PlayerActionType.Hooking;
        _hookController.StartHooking(_aimingSlider.value);
    }
    #endregion

    #region Catch Methods
    private void CatchHandle()
    {
        if(_actionType == PlayerActionType.Hooking)
        {
            StartCatching();
        }
    }

    public void StartCatching()
    {
        _actionType = PlayerActionType.Catching;
        _hookController.StartCatching();
    }

    public void StopCatching()
    {
        _actionType = PlayerActionType.Idle;
    }
    #endregion

    private void CancelHandle()
    {
        switch (_actionType)
        {
            case PlayerActionType.Idle:
                break;
            case PlayerActionType.Aiming:
                _actionType = PlayerActionType.Idle;
                _aimingSlider.gameObject.SetActive(false);
                break;
            case PlayerActionType.Hooking:
                break;
            default:
                break;
        }
    }
}
public enum PlayerActionType
{
    Idle,
    Aiming,
    Hooking, 
    Catching
}