using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFreezer : MonoBehaviour
{
    [HorizontalGroup("IsFreeze")]
    [SerializeField] private bool _freezeX;
    [HorizontalGroup("IsFreeze")]
    [SerializeField] private bool _freezeY;
    [HorizontalGroup("IsFreeze")]
    [SerializeField] private bool _freezeZ;
     
    [HorizontalGroup("FreezePos")] 
    [ShowIf(nameof(_freezeX))]
    [SerializeField] private float _posX;
    [HorizontalGroup("FreezePos")] 
    [ShowIf(nameof(_freezeY))]
    [SerializeField] private float _posY;
    [HorizontalGroup("FreezePos")] 
    [ShowIf(nameof(_freezeZ))]
    [SerializeField] private float _posZ;
     
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        if(_freezeX) pos.x = _posX;
        if(_freezeY) pos.y = _posY;
        if(_freezeZ) pos.z = _posZ;
        transform.position = pos;
    }
}
