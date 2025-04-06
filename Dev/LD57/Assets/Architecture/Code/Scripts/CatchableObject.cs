using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchableObject : Catchable
{
    [SerializeField] private CatchableObjectType _catchableObjectType;
    public CatchableObjectType CatchableObjectType => _catchableObjectType;
}
public enum CatchableObjectType
{
    Book,
    Bucket,
    Garbage,
}
