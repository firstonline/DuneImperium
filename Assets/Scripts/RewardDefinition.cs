using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RewardDefinition
{
    public bool HasCost; 
    [AllowNesting, ShowIf("HasCost")] public CostDefinition CostActionDefinition;

    public bool HasRequirement;
    [AllowNesting, ShowIf("HasRequirement")] public RequirementDefinition Requirement;

    public RewardActionDefinition Action;
    public int Quantity;
}