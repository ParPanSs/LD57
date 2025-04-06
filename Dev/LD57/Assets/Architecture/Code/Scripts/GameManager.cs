using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const KeyCode CANCEL_KEY_CODE = KeyCode.Escape;
    private const KeyCode HOOK_KEY_CODE = KeyCode.E;
    private const KeyCode BAIT_KEY_CODE = KeyCode.Q;
    private const KeyCode BOOK_KEY_CODE = KeyCode.R;
    private const KeyCode CATCH_KEY_CODE = KeyCode.Space;

    [SerializeField] private float _aimingSpeed;
    [SerializeField] private HookController _hookController;
    [SerializeField] private Slider _aimingSlider;
    [SerializeField] private GameObject _bookPlane;
    [SerializeField] private FishManager _fishManager;
    [SerializeField] private BaitManager _baitManager;
    [SerializeField] private List<GameObject> tails;
    public List<GameObject> Tails => tails;
    
    public BaitManager BaitManager => _baitManager;
    [SerializeField] private SerializedDictionary<PlayerActionState, GameObject> _hints;
    [SerializeField] private SerializedDictionary<CatchableObjectType, List<GameObject>> _catchedObjects;

    private PlayerActionState _actionState;
    public PlayerActionState ActionState => _actionState;
    public static bool isAnglerCatched;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {

        if (_actionState == PlayerActionState.Aiming)
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
        if (Input.GetKeyDown(BAIT_KEY_CODE) && IsCatchableObjectOpened(CatchableObjectType.Bucket))
        {
            BaitHandler();
        }
        if (Input.GetKeyDown(BOOK_KEY_CODE) && IsCatchableObjectOpened(CatchableObjectType.Book))
        {
            BookHandler();
        }
    }

    private bool IsCatchableObjectOpened(CatchableObjectType catchableObjectType)
    {
        return _catchedObjects.ContainsKey(catchableObjectType) && _catchedObjects[catchableObjectType][0].activeSelf;
    }

    #region Hook Methods
    private void HookHandle()
    {
        if (_actionState == PlayerActionState.Idle)
        {
            StartAiming();
        }
        else if (_actionState == PlayerActionState.Aiming)
        {
            StartHooking();
        }
    }

    private void StartAiming()
    {
        _aimingSlider.gameObject.SetActive(true);
        _actionState = PlayerActionState.Aiming;
        _hints[_actionState].SetActive(false);
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
        _actionState = PlayerActionState.Hooking;
        _hints[_actionState].SetActive(false);
        _hookController.StartHooking(_aimingSlider.value);
    }
    #endregion

    #region Catch Methods
    private void CatchHandle()
    {
        if (_actionState == PlayerActionState.Hooking)
        {
            StartCatching();
        }
    }

    public void StartCatching()
    {
        if (_actionState == PlayerActionState.Hooking || _actionState == PlayerActionState.Catching)
        {
            _actionState = PlayerActionState.Catching;
            _hookController.StartCatching();
        }
    }

    public void StopCatching(Catchable catchable)
    {
        _actionState = PlayerActionState.Idle;
        CheckCatched(catchable);
    }

    private void CheckCatched(Catchable catchable)
    {
        if (catchable == null)
            return;
        switch (catchable.CatchableType)
        {
            case CatchableType.Bait:
                BaitManager.InitizlizeBait((catchable as Bait).BaitId);
                break;
            case CatchableType.Fish:
                break;
            case CatchableType.Object:
                var type = (catchable as CatchableObject).CatchableObjectType;
                foreach (var item in _catchedObjects[type])
                {
                    item.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #region Bait Methods
    private void BaitHandler()
    {
        if (_actionState == PlayerActionState.Idle)
        {
            _actionState = PlayerActionState.SelectingBait;
            _hints[_actionState].SetActive(false);
            _baitManager.ToggleBaitsPanelActive(true);
        }
        else if (_actionState == PlayerActionState.SelectingBait)
        {
            _actionState = PlayerActionState.Idle;
            _baitManager.ToggleBaitsPanelActive(false);
        }
    }

    public void SelectBait(BaitId baitId)
    {
        _actionState = PlayerActionState.Idle;
        _baitManager.ToggleBaitsPanelActive(false);
        _hookController.SetBait(baitId);
    }
    #endregion

    #region Book Methods

    private void BookHandler()
    {
        if (_actionState == PlayerActionState.Idle)
        {
            _actionState = PlayerActionState.Reading;
            _hints[_actionState].SetActive(false);
            _bookPlane.SetActive(true);
        }
        else if (_actionState == PlayerActionState.Reading)
        {
            _actionState = PlayerActionState.Idle;
            _bookPlane.SetActive(false);
        }
    }
    #endregion

    private void CancelHandle()
    {
        switch (_actionState)
        {
            case PlayerActionState.Idle:
                break;
            case PlayerActionState.Aiming:
                _actionState = PlayerActionState.Idle;
                _aimingSlider.gameObject.SetActive(false);
                break;
            case PlayerActionState.Hooking:
                break;
            case PlayerActionState.SelectingBait:
                _actionState = PlayerActionState.Idle;
                _baitManager.ToggleBaitsPanelActive(false);
                break;
            case PlayerActionState.Reading:
                _bookPlane.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void SpawnFish()
    {
        _fishManager.SpawnRandomFish();
    }
    
    [Button("SpawnSardine")]
    public void SpawnSardine()
    {
        _fishManager.SpawnFish(FishId.Sardine, 3);
    }
}
public enum PlayerActionState
{
    Idle,
    Aiming,
    Hooking,
    Catching,
    SelectingBait,
    Reading,

}