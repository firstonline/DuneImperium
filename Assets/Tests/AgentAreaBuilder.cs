using System.Collections;
using UnityEngine;

public static class AgentAreaBuilder 
{
    public static AgentAreaDefinition Build()
    {
        var agentArea = ScriptableObject.CreateInstance<AgentAreaDefinition>();
        agentArea.Requirement = new RequirementDefinition();
        return agentArea;
    }

    public static AgentAreaDefinition WithAgentIcon(this AgentAreaDefinition agentArea, AgentIconDefinition agentIcon)
    {
        agentArea.AgentIcon = agentIcon;
        return agentArea;
    }
}
