using UnityEngine;

public class Detectable : MonoBehaviour
{
    [SerializeField] private DetectableType _detectableType;
    public DetectableType DetectableType => _detectableType; 
}
public enum DetectableType
{
    Obstacle,
    Speedup,
    Slowdown,
    MoveLeft,
    MoveRight,
}