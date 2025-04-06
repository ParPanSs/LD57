using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDetector : MonoBehaviour
{
    [SerializeField] private Fish _fish;

    private void Start()
    {
        if(_fish.fishStatus.fishId == FishId.SiameseFish)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<HookController>(out HookController hook))
        {
            _fish.DetectHook(hook.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<HookController>(out HookController hook))
        {
            _fish.UndetectHook();
        }
    }
}
