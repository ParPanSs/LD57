
using UnityEngine;

public abstract class Catchable :MonoBehaviour
{
    [SerializeField] private CatchableType _catchableType;
    public CatchableType CatchableType => _catchableType;
    [SerializeField] private float _shakingTime =0.3f;
    public float ShakingTime => _shakingTime;
}

public enum CatchableType
{
    Bait,
    Fish,
    Object, 
}
