using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RewardDefinition
{
    public bool HasCost; 
    [AllowNesting, ShowIf("HasCost")] public CostActionDefinition CostActionDefinition;
    [AllowNesting, ShowIf("HasCost")] public int CostQuantity;

    public bool HasRequirement;
    [AllowNesting, ShowIf("HasRequirement")] public RequirementActionDefinition Requirement;
    [AllowNesting, ShowIf("HasRequirement")] public int RequirementQuantity;

    public RewardActionDefinition Action;
    public int Quantity;
}