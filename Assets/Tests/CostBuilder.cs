using System.Collections;
using UnityEngine;

public static class CostBuilder
{
    public static CostDefinition Build()
    {
        var cost = new CostDefinition();
        return cost;
    }

    public static CostDefinition WithAction(this CostDefinition definition, CostActionTypes type)
    {
        var action = ScriptableObject.CreateInstance<CostActionDefinition>();
        action.Type = type;
        definition.Action = action;
        return definition;
    }

    public static CostDefinition WithQuantity(this CostDefinition definition, int quantity)
    {
        definition.Quantity = quantity;
        return definition;
    }
}
