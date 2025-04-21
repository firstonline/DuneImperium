using System;
using System.Collections.Generic;

[Serializable]
public class ExchangeDefinition
{
    public List<RequirementDefinition> Requirements;
    public List<CostDefinition> Costs;
    public List<RewardDefinition> Rewards;
}
