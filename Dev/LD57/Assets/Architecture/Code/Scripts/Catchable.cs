
using UnityEngine;

public abstract class Catchable :MonoBehaviour
{
    [SerializeField] private CatchableType _catchableType;
    public CatchableType CatchableType => _catchableType;
}

public enum CatchableType
{
    Bait,
    Fish,
    Object, 
}
