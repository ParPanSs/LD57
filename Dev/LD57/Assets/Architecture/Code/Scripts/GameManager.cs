using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SerializedMonoBehaviour
{
    public bool testOdin;
    [ShowIf(nameof(testOdin))]
    public Dictionary<string, int> testOdinDicktionary;
    
}
