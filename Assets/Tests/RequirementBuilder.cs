using System.Collections;
using UnityEngine;

public static class RequirementBuilder
{
    public static RequirementDefinition Build()
    {
        var requirement = new RequirementDefinition();
        return requirement;
    }

    public static RequirementDefinition WithAction(this RequirementDefinition definition, RequirementActionTypes type)
    {
        var action = ScriptableObject.CreateInstance<RequirementActionDefinition>();
        action.Type = type;
        definition.Action = action;
        return definition;
    }

    public static RequirementDefinition WithQuantity(this RequirementDefinition definition, int quantity)
    {
        definition.Quantity = quantity;
        return definition;
    }

}
