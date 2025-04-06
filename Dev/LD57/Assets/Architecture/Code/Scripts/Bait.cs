using Sirenix.OdinInspector;
using UnityEngine;

public class Bait : Catchable
{
    [SerializeField] private BaitId _baitId;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public BaitId BaitId => _baitId;

    private void Start()
    {
        InitializeBait();
    }
    private void InitializeBait( )
    { 
        _spriteRenderer.sprite = GameManager.Instance.BaitManager.GetBaitSprite(_baitId);
    }
}
