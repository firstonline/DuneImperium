using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public enum RewardType
{
    Resource = 0,
    Resource2 = 1,
}

[Serializable]
public class RewardDefinition
{
    // Condition
    public RewardType RewardType;
    [AllowNesting, ShowIf("RewardType", RewardType.Resource)]
    public BaseResourceDefinition Resource;

    public int Quantity;
}