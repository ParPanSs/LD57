using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaitManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<BaitId, Sprite> _baitSpriteDT;

    [SerializeField] private GameObject _baitSelectionPrefab;
    [SerializeField] private GameObject _baitsPanel;
    [SerializeField] private List<BaitId> _defaultBaits;
    [SerializeField] private SerializedDictionary<BaitId, List<GameObject>> _objectsForAvailableBaits = new();

    private List<BaitId> _availableBaits = new();


    private void Start()
    {
        foreach (var item in _defaultBaits)
        {
            InitizlizeBait(item);
        }
    }

    public void InitizlizeBait(BaitId baitId)
    {
        var bait = Instantiate(_baitSelectionPrefab, _baitsPanel.transform);
        _availableBaits.Add(baitId);
        if (_objectsForAvailableBaits.ContainsKey(baitId))
        {
            foreach (var item in _objectsForAvailableBaits[baitId])
            {
                item.gameObject.SetActive(true);
            }
        }
        bait.GetComponent<Image>().sprite = GetBaitSprite(baitId);
        bait.GetComponentInChildren<TextMeshProUGUI>().text = _availableBaits.Count.ToString();
    }

    public Sprite GetBaitSprite(BaitId baitId)
    {
        return _baitSpriteDT[baitId];
    }

    public void ToggleBaitsPanelActive(bool active)
    {
        _baitsPanel.SetActive(active);
    }

    private void Update()
    {
        if (GameManager.Instance.ActionState == PlayerActionState.SelectingBait)
        {
            SelectBaitHandle();
        }
    }

    private void SelectBaitHandle()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKey(KeyCode.Alpha1 + i))
            {
                SelectBait(i);
                break;
            }
        }
    }

    private void SelectBait(int index)
    {
        if (_availableBaits.Count > index)
        {
            GameManager.Instance.SelectBait(_availableBaits[index]);
        }
    }
}

public enum BaitId
{
    Meat,
    Worm,
    Empty,
    Greens
}
