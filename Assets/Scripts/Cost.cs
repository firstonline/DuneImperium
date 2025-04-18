using System;
using System.Collections;
using UnityEngine;


public enum CostType
{
    Resource = 0,
}

[Serializable]
public class Cost
{
    public CostType CostType;
    public BaseResourceDefinition Resource;
    public int Quantity;
}
