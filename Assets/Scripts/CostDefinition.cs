using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;


public enum CostType
{
    Resource = 0,
    DiscardCard = 1,
}

[Serializable]
public class CostDefinition
{
    public CostType CostType;

    [AllowNesting, ShowIf("CostType", CostType.Resource)] 
    public BaseResourceDefinition Resource;

    public int Quantity;
}
