using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardBuilder
{
    public static CardDefinition Build()
    {
        var card = ScriptableObject.CreateInstance<CardDefinition>();
        card.AgentIcons = new List<AgentIconDefinition>();
        return card; 
    }

    public static CardDefinition WithId(this CardDefinition card, int id)
    {
        card.ID = id;
        return card;
    }

    public static CardDefinition WithIcon(this CardDefinition card, AgentIconDefinition agentIcon)
    {
        card.AgentIcons.Add(agentIcon);
        return card;
    }
}
