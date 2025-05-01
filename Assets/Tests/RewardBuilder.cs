using System.Collections;
using UnityEngine;

public static class RewardBuilder
{
    public static RewardDefinition Build()
    {
        var reward = new RewardDefinition();
        return reward;
    }

    public static RewardDefinition WithAction(this RewardDefinition definition, RewardActionTypes type)
    {
        var action = ScriptableObject.CreateInstance<RewardActionDefinition>();
        action.Type = type;
        definition.Action = action;
        return definition;
    }

    public static RewardDefinition WithQuantity(this RewardDefinition definition, int quantity)
    {
        definition.Quantity = quantity;
        return definition;
    }
}
