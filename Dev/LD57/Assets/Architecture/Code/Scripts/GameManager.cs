using System.Collections;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
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
    private const KeyCode SHOP_KEY_CODE = KeyCode.T;
    private const KeyCode CATCH_KEY_CODE = KeyCode.Space;

    [SerializeField] private float _aimingSpeed;
    [SerializeField] private HookController _hookController;
    [SerializeField] private Slider _aimingSlider;
    [SerializeField] private GameObject _bookPlane;
    [SerializeField] private GameObject _shopPlane;
    [SerializeField] private FishManager _fishManager;
    [SerializeField] private BaitManager _baitManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private RoundManager _roundManager;
    [SerializeField] private List<GameObject> tails;
    [SerializeField] private SerializedDictionary<PlayerActionState, GameObject> _hints;
    [SerializeField] private SerializedDictionary<CatchableObjectType, List<GameObject>> _catchedObjects;
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private GameObject highScoreSpawnPoint;
    [SerializeField] private GameObject playerInPanel;
    [SerializeField] private ApiConnector apiConnector;
    [SerializeField] private GameObject input;
    [SerializeField] private TMP_InputField namePanel;

    public List<GameObject> Tails => tails;
    public FishManager FishManager => _fishManager;
    public BaitManager BaitManager => _baitManager;
    public ScoreManager ScoreManager => _scoreManager;
    public float SpeedStat { get; set; }
    public float MovementStat { get; set; }
    public float HookStat { get; set; }

    private PlayerActionState _actionState;
    public PlayerActionState ActionState => _actionState;
    public static bool isAnglerCatched;
    public static bool isEndlessMode;
    public bool isFirstTimePlaying = true;
    public bool isDataGot;
    private string _playerName;
    private int _fishCaught;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        apiConnector.GetPlayers();
        if (isFirstTimePlaying && isEndlessMode)
        {
            input.gameObject.SetActive(true);
        }
    }
    
    public void SetName()
    {
        _playerName = namePanel.text;
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
        if (Input.GetKeyDown(SHOP_KEY_CODE) && IsCatchableObjectOpened(CatchableObjectType.Shop))
        {
            ShopHandler();
        }
    }

    [Button("LOSE TEST")]
    public void Lose()
    {
        isDataGot = false;
        _actionState = PlayerActionState.Lose;
        
        StartCoroutine(GetDataFromServer());
    }

    private IEnumerator GetDataFromServer()
    {
        if (!apiConnector.IsPlayerExist(_playerName))
        {
            apiConnector.AddPlayer(_playerName, _scoreManager.Score, _fishCaught);
            yield return new WaitUntil(() => isDataGot);
        }
        if (apiConnector.IsPlayerExist(_playerName) && _scoreManager.Score > apiConnector.GetPlayerScore(_playerName))
        {
            apiConnector.UpdatePlayerScore(_playerName, _scoreManager.Score, _fishCaught);
            yield return new WaitUntil(() => isDataGot);
        }
        apiConnector.DisplayTopScore();
        yield return new WaitUntil(() => isDataGot);
        GameObject createdPlayer;
        List<PlayerData> playerData = new();
        playerData = apiConnector.GetPlayerData();
        for (int i = playerData.Count - 1; i >= 0; i--)
        {
            createdPlayer = Instantiate(playerInPanel, highScoreSpawnPoint.transform);
            createdPlayer.GetComponent<PlayerDataInTable>().SetPlayersTable($"#{i + 1}", playerData[i].playerName,
                playerData[i].playerScore.ToString(), playerData[i].playerFish.ToString());
        }
        highScorePanel.SetActive(true);
        isFirstTimePlaying = false;
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

        var caughtObject = _hookController.GetCatchedObject() as Fish;
        if (caughtObject)
        {
            if (!_roundManager.RightFish(caughtObject.fishStatus.fishId)) Lose();
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
                if (IsCatchableObjectOpened(CatchableObjectType.Bucket))
                {
                    _catchedObjects[CatchableObjectType.Bucket][1].SetActive(true);
                }
                break;
            case CatchableType.Fish:
                if (isEndlessMode && !_roundManager.RightFish((catchable as Fish).fishStatus.fishId)) return;
                _scoreManager.IncreaseGold((catchable as Fish).fishStatus.goldReward);
                _scoreManager.IncreaseScore((catchable as Fish).fishStatus.scoreReward);
                _fishCaught++;
                if (isEndlessMode && _roundManager.RightFish((catchable as Fish).fishStatus.fishId))
                {
                    _roundManager.SetNewFish();
                }
                break;
            case CatchableType.Object:
                var type = (catchable as CatchableObject).CatchableObjectType;
                if (_catchedObjects.ContainsKey(type))
                { 
                    foreach (var item in _catchedObjects[type])
                    {
                        item.SetActive(true);
                    }
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

    #region Shop Methods

    private void ShopHandler()
    {
        if (_actionState == PlayerActionState.Idle)
        {
            _actionState = PlayerActionState.Shop;
            _hints[_actionState].SetActive(false);
            _shopPlane.SetActive(true);
        }
        else if (_actionState == PlayerActionState.Shop)
        {
            _actionState = PlayerActionState.Idle;
            _shopPlane.SetActive(false);
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
                _actionState = PlayerActionState.Idle;
                _bookPlane.SetActive(false);
                break;
            case PlayerActionState.Shop:
                _actionState = PlayerActionState.Idle;
                _shopPlane.SetActive(false);
                break;
            default:
                break;
        }
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
    Shop,
    Lose,
}