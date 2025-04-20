using System;
using System.Collections;
using UnityEngine;

public enum RequirementType
{
    Resource = 0,
}

[Serializable]
public class Requirement
{
    public RequirementType RequirementType;
    public GameElementDefinition Resource;
}
