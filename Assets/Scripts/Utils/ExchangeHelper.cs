using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class ExchangeHelper 
{
    public static bool CanVisitArea(GameData gameData, PlayerData playerData, AgentAreaDefinition agentArea)
    {
        if (!agentArea)
            return false;

        return CheckRequirement(gameData, playerData, agentArea.Requirement);
    }

    public static bool CanGainRewards(GameData gameData, PlayerData playerData, ExchangeDefinition exchange)
    {
        return exchange.Rewards.Any(reward => !reward.HasRequirement || CheckRequirement(gameData, playerData, reward.Requirement));
    }

    public static bool CanPayCost(PlayerData playerData, ExchangeDefinition exchange)
    {
        bool canPay = true;
        foreach (var cost in exchange.Costs)
        {
            canPay = cost.Action.Type switch
            {
                CostActionTypes.RetreatTroop => false,
                CostActionTypes.DestroyTroop => false,
                CostActionTypes.RemoveSpice => playerData.Resources[ResourceType.Spice] >= cost.Quantity,
                CostActionTypes.RemoveWater => playerData.Resources[ResourceType.Water] >= cost.Quantity,
                CostActionTypes.RemoveSolari => playerData.Resources[ResourceType.Solari] >= cost.Quantity,
                CostActionTypes.RemoveFremenInfluence => playerData.Influences[House.Fremen] >= cost.Quantity,
                CostActionTypes.RemoveBenneGesseritInfluence => playerData.Influences[House.BeneGesserit] >= cost.Quantity,
                CostActionTypes.RemoveSpacingGuildInfluence => playerData.Influences[House.SpacingGuild] >= cost.Quantity,
                CostActionTypes.RemoveEmperorInfluence => playerData.Influences[House.Emperror] >= cost.Quantity,
                CostActionTypes.RemoveAnyInfluence => HasEnoughInfluence(playerData, cost.Quantity),
                CostActionTypes.TrashCard => false,
                CostActionTypes.DiscardCard => false,
                CostActionTypes.TrashIntrigue => false,
                CostActionTypes.RecallSpy => false,
                _ => false
            }; ;

            if (!canPay)
                break;
        }
        return canPay;
    }

    public static bool CheckRequirement(GameData gameData, PlayerData playerData, RequirementDefinition requirement)
    {
        if (requirement.Action == null)
            return true;

        bool meetsRequirement = requirement.Action.Type switch
        {
            RequirementActionTypes.RequireFrementInfluence => playerData.Influences[House.Fremen] >= requirement.Quantity,
            RequirementActionTypes.RequireBeneGesseritInfluence => playerData.Influences[House.BeneGesserit] >= requirement.Quantity,
            RequirementActionTypes.RequireSpacingGuildInfluence => playerData.Influences[House.SpacingGuild] >= requirement.Quantity,
            RequirementActionTypes.RequireEmperorInfluence => playerData.Influences[House.Emperror] >= requirement.Quantity,
            RequirementActionTypes.RequireFremenAlliance => playerData.Alliances[House.Fremen],
            RequirementActionTypes.RequireBeneGesseritAlliance => playerData.Alliances[House.BeneGesserit],
            RequirementActionTypes.RequireSpacingGuildAlliance => playerData.Alliances[House.SpacingGuild],
            RequirementActionTypes.RequireEmperorAlliance => playerData.Alliances[House.Emperror],
            RequirementActionTypes.RequireFremenCardInPlay => false,
            RequirementActionTypes.RequireBenneGesseritCardInPlay => false,
            RequirementActionTypes.RequireSpacingGuildCardInPlay => false,
            RequirementActionTypes.RequireEmperorCardInPlay => false,
            RequirementActionTypes.RequireMakerHook => playerData.HasMakerHook,
            RequirementActionTypes.RequireSwordsman => !playerData.HaveSwordsman && gameData.Players.Any(x => x.HaveSwordsman),
            RequirementActionTypes.RequireNoSwordsman => gameData.Players.All(x => !x.HaveSwordsman),
            RequirementActionTypes.RequireNoCouncilSeat => !playerData.HasCouncilSeat,
            RequirementActionTypes.RequireCouncilSeat => playerData.HasCouncilSeat,
        };
        return meetsRequirement;
    }

    public static void PayCost(ref PlayerData playerData, ExchangeDefinition exchange)
    {
        foreach (var cost in exchange.Costs)
        {
            switch (cost.Action.Type)
            {
                case CostActionTypes.RetreatTroop: break;
                case CostActionTypes.DestroyTroop: break;
                case CostActionTypes.RemoveSpice: playerData.Resources[ResourceType.Spice] -= cost.Quantity; break;
                case CostActionTypes.RemoveWater: playerData.Resources[ResourceType.Water] -= cost.Quantity; break;
                case CostActionTypes.RemoveSolari: playerData.Resources[ResourceType.Solari] -= cost.Quantity; break;
                case CostActionTypes.RemoveFremenInfluence: playerData.Influences[House.Fremen] -= cost.Quantity; break;
                case CostActionTypes.RemoveBenneGesseritInfluence: playerData.Influences[House.BeneGesserit] -= cost.Quantity; break;
                case CostActionTypes.RemoveSpacingGuildInfluence: playerData.Influences[House.SpacingGuild] -= cost.Quantity; break;
                case CostActionTypes.RemoveEmperorInfluence: playerData.Influences[House.Emperror] -= cost.Quantity; break;
                case CostActionTypes.RemoveAnyInfluence: break;
                case CostActionTypes.TrashCard: break;
                case CostActionTypes.DiscardCard: break;
                case CostActionTypes.TrashIntrigue: break;
                case CostActionTypes.RecallSpy: break;
            }
        }
    }

    public static void ReceiveRewards(ref PlayerData playerData, ExchangeDefinition exchange)
    {
        foreach (var reward in exchange.Rewards)
        {
            switch (reward.Action.Type)
            {
                case RewardActionTypes.AddTroop:
                    playerData.GarrisonedTroopsCount = Mathf.Min(playerData.GarrisonedTroopsCount + reward.Quantity, PlayerData.MAX_TROOPS - playerData.DeployedTroopsCount);
                    break;

                case RewardActionTypes.AddSpice:
                    playerData.Resources[ResourceType.Spice] += reward.Quantity;
                    break;

                case RewardActionTypes.AddWater:
                    playerData.Resources[ResourceType.Water] += reward.Quantity;
                    break;

                case RewardActionTypes.AddSolari:
                    playerData.Resources[ResourceType.Solari] += reward.Quantity;
                    break;

                case RewardActionTypes.AddFremenInfluence:
                    playerData.Influences[House.Fremen] = Mathf.Min(playerData.Influences[House.Fremen] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddBenneGesseritInfluence:
                    playerData.Influences[House.BeneGesserit] = Mathf.Min(playerData.Influences[House.BeneGesserit] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddSpacingGuildInfluence:
                    playerData.Influences[House.SpacingGuild] = Mathf.Min(playerData.Influences[House.SpacingGuild] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddEmperorInfluence:
                    playerData.Influences[House.Emperror] = Mathf.Min(playerData.Influences[House.Emperror] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddVictoryPoints:
                    break;

                case RewardActionTypes.AddSwordsman:
                    playerData.HaveSwordsman = true;
                    playerData.AgentsCount++;
                    break;

                case RewardActionTypes.RecallAgent:
                    break;

                case RewardActionTypes.AddWorm:
                    playerData.WormsCount = playerData.WormsCount + reward.Quantity;
                    break;

                case RewardActionTypes.AddPersuation:
                    break;

                case RewardActionTypes.AddCombatSword:
                    break;

                case RewardActionTypes.AddChoam:
                    break;

                case RewardActionTypes.AddCouncilSeat:
                    playerData.HasCouncilSeat = true;
                    break;

                case RewardActionTypes.DrawCard:
                    break;

                case RewardActionTypes.AddCard:
                    break;

                case RewardActionTypes.AddIntrigue:
                    break;

                case RewardActionTypes.AddAnyInfluence:
                    break;

                case RewardActionTypes.Add2DifferentInfluence:
                    break;

                case RewardActionTypes.BreakWall:
                    break;

                case RewardActionTypes.AddSpy:
                    break;

                case RewardActionTypes.AddMakerHook:
                    playerData.HasMakerHook = true;
                    break;

                case RewardActionTypes.StealIntrigue:
                    break;

                case RewardActionTypes.TrashCard:
                    break;
            }
        }
    }


    static bool HasEnoughInfluence(PlayerData playerData, int quantity)
    {
        int totalInfluence = 0;
        totalInfluence += playerData.Influences[House.Fremen];
        totalInfluence += playerData.Influences[House.BeneGesserit];
        totalInfluence += playerData.Influences[House.SpacingGuild];
        totalInfluence += playerData.Influences[House.Emperror];

        return totalInfluence >= quantity;
    }
}
