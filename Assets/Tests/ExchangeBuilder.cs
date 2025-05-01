using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExchangeBuilder
{
    public static ExchangeDefinition Build()
    {
        var exchange = new ExchangeDefinition();
        exchange.Requirements = new();
        exchange.Costs = new();
        exchange.Rewards = new();

        return exchange;
    }

    public static ExchangeDefinition WithCost(this ExchangeDefinition definition, CostDefinition cost)
    {
        definition.Costs.Add(cost);
        return definition;
    }

    public static ExchangeDefinition WithReward(this ExchangeDefinition definition, RewardDefinition reward)
    {
        definition.Rewards.Add(reward);
        return definition;
    }

}
