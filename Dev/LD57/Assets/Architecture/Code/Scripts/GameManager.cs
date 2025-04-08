using System.Collections;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const KeyCode CANCEL_KEY_CODE = KeyCode.Escape;
    private const KeyCode HOOK_KEY_CODE = KeyCode.E;
    private const KeyCode BAIT_KEY_CODE = KeyCode.Q;
    private const KeyCode BOOK_KEY_CODE = KeyCode.R;
    private const KeyCode SHOP_KEY_CODE = KeyCode.T;
    private const KeyCode CATCH_KEY_CODE = KeyCode.Space;

    [SerializeField] private Light2D globalLight;

    [SerializeField] private float _aimingSpeed;
    [SerializeField] private Slider _aimingSlider;
    [SerializeField] private Slider _mainVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private TMP_InputField namePanel;
    [SerializeField] private AudioSource _mainVolumeSource;
    [SerializeField] private AudioSource _sfxVolumeSource;
    [SerializeField] private AudioClip _openBookClip;
    [SerializeField] private AudioClip _openBucketClip;
    [SerializeField] private AudioClip _openShopClip;
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioMixer sfxAudioMixer;
    [SerializeField] private GameObject _bookPlane;
    [SerializeField] private GameObject _shopPlane;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private GameObject highScoreSpawnPoint;
    [SerializeField] private GameObject cutscene;
    [SerializeField] private GameObject playerInPanel;
    [SerializeField] private GameObject input;
    [SerializeField] private HookController _hookController;
    [SerializeField] private FishManager _fishManager;
    [SerializeField] private BaitManager _baitManager;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private RoundManager _roundManager;
    [SerializeField] private ApiConnector apiConnector;
    [SerializeField] private List<GameObject> tails;
    //[SerializeField] private SerializedDictionary<PlayerActionState, GameObject> _hints;
    [SerializeField] private SerializedDictionary<CatchableObjectType, List<GameObject>> _catchedObjects;

    public List<GameObject> Tails => tails;
    public FishManager FishManager => _fishManager;
    public BaitManager BaitManager => _baitManager;
    public ScoreManager ScoreManager => _scoreManager;
    public float SpeedStat { get; set; }
    public float MovementStat { get; set; }
    public float FishPriceStat { get; set; }

    private PlayerActionState _actionState;
    public PlayerActionState ActionState => _actionState;
    public static bool isEndlessMode;
    public static bool isFirstTimePlaying = true;
    public bool isDataGot;
    private string _playerName;
    private int _fishCaught;

    [SerializeField] private List<Image> hooksHp;
    private int hp = 3;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainAudioMixer.GetFloat("Volume", out float volume);
        _mainVolumeSlider.value = volume;
        sfxAudioMixer.GetFloat("Volume", out float sfxVolume);
        _sfxVolumeSlider.value = sfxVolume;

        apiConnector.GetPlayers();
        if (isFirstTimePlaying && isEndlessMode)
        {
            _actionState = PlayerActionState.Pause;
            input.gameObject.SetActive(true);
        }
    }

    public void SetName()
    {
        if (namePanel.text == string.Empty) return;
        _playerName = namePanel.text;
        _actionState = PlayerActionState.Idle;
        namePanel.gameObject.transform.parent.gameObject.SetActive(false);
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

    public void LeaderboardButtonAction(int value)
    {
        switch (value)
        {
            case 0:
                FaderController.instance.FadeOut(0);
                break;
            case 1:
                FaderController.instance.FadeOut(2);
                break;
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
        //_hints[_actionState].SetActive(false);
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
        //_hints[_actionState].SetActive(false);
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
                if (IsCatchableObjectOpened(CatchableObjectType.Bucket))
                {
                    _catchedObjects[CatchableObjectType.Bucket][1].SetActive(true);
                }
                break;
            case CatchableType.Fish:
                var fish = catchable as Fish;
                if (isEndlessMode)
                {
                    if (!_roundManager.RightFish(fish.fishStatus.fishId))
                    {
                        hp--;
                        hooksHp[hp].color = new Color32(0, 0, 0, 128); 
                        if (isEndlessMode && hp <= 0) Lose();
                        return;
                    }
                    else
                    {
                        _roundManager.SetNewFish();
                    }
                    _scoreManager.IncreaseGold((int)(300));
                }
                else
                {
                    if (fish.fishStatus.fishId == FishId.Angler)
                    {
                        PlayerPrefs.SetInt("isAnglerCatched", 1);
                        cutscene.SetActive(true);
                    }
                    _scoreManager.IncreaseGold((int)(fish.fishStatus.goldReward * FishPriceStat));
                }
                _scoreManager.IncreaseScore(fish.fishStatus.scoreReward);
                _fishCaught++;
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
            PlaySFX(_openBucketClip);
            _actionState = PlayerActionState.SelectingBait;
            //_hints[_actionState].SetActive(false);
            _baitManager.ToggleBaitsPanelActive(true);
        }
        else if (_actionState == PlayerActionState.SelectingBait)
        {
            PlaySFX(_openBucketClip);
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
            PlaySFX(_openBookClip);
            _actionState = PlayerActionState.Reading;
            //_hints[_actionState].SetActive(false);
            _bookPlane.SetActive(true);
        }
        else if (_actionState == PlayerActionState.Reading)
        {
            PlaySFX(_openBookClip);
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
            PlaySFX(_openShopClip);
            _actionState = PlayerActionState.Shop;
            //_hints[_actionState].SetActive(false);
            _shopPlane.SetActive(true);
        }
        else if (_actionState == PlayerActionState.Shop)
        {
            PlaySFX(_openShopClip);
            _actionState = PlayerActionState.Idle;
            _shopPlane.SetActive(false);
        }
    }
    #endregion

    #region Pause Methods
    public void SetMainVolume(float value)
    {
        mainAudioMixer.SetFloat("Volume", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxAudioMixer.SetFloat("SFXVolume", value);
    }

    public void Resume()
    {
        _actionState = PlayerActionState.Idle;
        _pausePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        FaderController.instance.FadeOut(0);
    }
    #endregion

    public void SetLowpassForMixers(float y)
    {
        float lowpass;
        float lightLowpass;
        if (y > -2.5f)
        {
            lightLowpass = 1;
            lowpass = 22000;
        }
        else
        {
            // ��������� ���� �� -700 �� -2.5
            float clamped = Mathf.Clamp(y, -700f, -2.5f);

            // ����������� �� -700 (0) �� -2.5 (1)
            var t = Mathf.InverseLerp(-2.5f, -700f, clamped);
            lowpass = Mathf.Lerp(6000f, 250f, t);
            lightLowpass = Mathf.Lerp(1, 0.01f, t);
        }
        mainAudioMixer.SetFloat("Lowpass", lowpass);
        sfxAudioMixer.SetFloat("SFXLowpass", lowpass);
        globalLight.intensity = lightLowpass;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null) return;
        _sfxVolumeSource.pitch = Random.Range(0.95f, 1.05f);
        _sfxVolumeSource.clip = audioClip;
        _sfxVolumeSource.Play();
    }
    private void CancelHandle()
    {
        PlaySFX(_openBookClip);

        switch (_actionState)
        {
            case PlayerActionState.Idle:
                _actionState = PlayerActionState.Pause;
                _pausePanel.SetActive(true);
                break;
            case PlayerActionState.Pause:
                _actionState = PlayerActionState.Idle;
                _pausePanel.SetActive(false);
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
    Pause,
}