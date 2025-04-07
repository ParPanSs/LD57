
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Catchable :MonoBehaviour
{
    [SerializeField] private CatchableType _catchableType;
    [SerializeField] private float _shakingTime =0.3f;
    [SerializeField] private bool hasAnimator;
    [ShowIf(nameof(hasAnimator))]
    [SerializeField] private Animator animator;
    public CatchableType CatchableType => _catchableType;
    public float ShakingTime => _shakingTime;

    public void OnHook()
    {
        if (hasAnimator)
            animator.SetTrigger("Hook");
    }
}

public enum CatchableType
{
    Bait,
    Fish,
    Object, 
}
