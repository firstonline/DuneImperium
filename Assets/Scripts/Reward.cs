using System;
using System.Collections;
using UnityEngine;

public enum RewardType
{
    Resource = 0,
}

[Serializable]
public class Reward
{
    // Condition
    public RewardType RewardType;
    public BaseResourceDefinition Resource;
    public int Quantity;
}