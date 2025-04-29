using System.Linq;
using UniDi;
using Unity.Netcode;
using UnityEngine;

public class AreasService : NetworkBehaviour
{
    [Inject] AgentAreaDatabase _agentAreaDatabase;
    [Inject] NetworkGameplayService _networkGameplayService;

    public void VisitAgentArea(int areaId, int selectedExchange)
    {
        VisitAgentAreaServerRpc(areaId, selectedExchange);
    }

    [ServerRpc(RequireOwnership = false)]
    void VisitAgentAreaServerRpc(int areaId, int selectedExchange, ServerRpcParams rpcParams = default)
    {
        var agentArea = _agentAreaDatabase.GetItem(areaId);
        var gameData = _networkGameplayService.GameData;

        int playerIndex = -1;

        for (int i = 0; i < gameData.Players.Count; i++)
        {
            if (gameData.Players[i].ClientId == rpcParams.Receive.SenderClientId)
            {
                playerIndex = i;
                break;
            }
        }

        if (playerIndex == -1)
        {
            return;
        }

        var playerData = gameData.Players[playerIndex];

        if (CanVisitArea(playerData, agentArea))
        {
            var exchange = agentArea.Exchanges[selectedExchange];
            bool allowedToBuy = exchange.Requirements.All(x => CheckRequirement(playerData, x));
            bool canPay = CanPay(playerData, exchange);
            bool canGainRewards = exchange.Rewards.Any(reward => !reward.HasRequirement || CheckRequirement(playerData, reward.Requirement));

            if (allowedToBuy && canPay && canGainRewards)
            {
                PayCost(ref playerData, exchange);
                ReceiveRewards(ref playerData, exchange);

                gameData.Players[playerIndex] = playerData;
                gameData.RandomData = gameData.RandomData + 1; // this enforce network variable change
                _networkGameplayService.UpdateGameData(gameData);
                VisitAgentAreaResponseClientRpc(true, rpcParams.Receive.SenderClientId);
            }
            else
            {
                VisitAgentAreaResponseClientRpc(false, rpcParams.Receive.SenderClientId);
            }
        }
        else
        {
            VisitAgentAreaResponseClientRpc(false, rpcParams.Receive.SenderClientId);
        }

    }

    [ClientRpc]
    void VisitAgentAreaResponseClientRpc(bool success, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        Debug.Log($"Request: {success}");
    }

    bool CanPay(PlayerData playerData, ExchangeDefinition exchange)
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
                CostActionTypes.RemoveFremenInfluence => false,
                CostActionTypes.RemoveBenneGesseritInfluence => false,
                CostActionTypes.RemoveSpacingGuildInfluence => false,
                CostActionTypes.RemoveEmperorInfluence => false,
                CostActionTypes.RemoveAnyInfluence => false,
                CostActionTypes.TrashCard => false,
                CostActionTypes.DiscardCard => false,
                CostActionTypes.TrashIntrigue => false,
                CostActionTypes.RecallSpy => false,
                _ => false
            };

            if (!canPay)
                break;
        }
        return canPay;
    }

    void PayCost(ref PlayerData playerData, ExchangeDefinition exchange)
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
                case CostActionTypes.RemoveFremenInfluence: break;
                case CostActionTypes.RemoveBenneGesseritInfluence: break;
                case CostActionTypes.RemoveSpacingGuildInfluence: break;
                case CostActionTypes.RemoveEmperorInfluence: break;
                case CostActionTypes.RemoveAnyInfluence: break;
                case CostActionTypes.TrashCard: break;
                case CostActionTypes.DiscardCard: break;
                case CostActionTypes.TrashIntrigue: break;
                case CostActionTypes.RecallSpy: break;
            }
        }
    }

    void ReceiveRewards(ref PlayerData playerData, ExchangeDefinition exchange)
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
                    playerData.Resources[ResourceType.FremenInfluence] = Mathf.Min(playerData.Resources[ResourceType.FremenInfluence] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddBenneGesseritInfluence:
                    playerData.Resources[ResourceType.BeneGesseritInfluence] = Mathf.Min(playerData.Resources[ResourceType.BeneGesseritInfluence] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddSpacingGuildInfluence:
                    playerData.Resources[ResourceType.SpacingGuildInfluence] = Mathf.Min(playerData.Resources[ResourceType.SpacingGuildInfluence] + reward.Quantity, PlayerData.MAX_INFLUENCE);
                    break;

                case RewardActionTypes.AddEmperorInfluence:
                    playerData.Resources[ResourceType.EmperorInfluence] = Mathf.Min(playerData.Resources[ResourceType.EmperorInfluence] + reward.Quantity, PlayerData.MAX_INFLUENCE);
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
                    playerData.Resources[ResourceType.MakerHook] += reward.Quantity;
                    break;

                case RewardActionTypes.StealIntrigue:
                    break;

                case RewardActionTypes.TrashCard:
                    break;
            }
        }
    }

    bool CheckRequirement(PlayerData playerData, RequirementDefinition requirement)
    {
        if (requirement.Quantity == 0)
            return true;

        bool meetsRequirement = requirement.Action.Type switch
        {
            RequirementActionTypes.RequireFrementInfluence => playerData.Resources[ResourceType.FremenInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireBeneGesseritInfluence => playerData.Resources[ResourceType.BeneGesseritInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireSpacingGuildInfluence => playerData.Resources[ResourceType.SpacingGuildInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireEmperorInfluence => playerData.Resources[ResourceType.EmperorInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireFremenAlliance => false,
            RequirementActionTypes.RequireBeneGesseritAlliance => false,
            RequirementActionTypes.RequireSpacingGuildAlliance => false,
            RequirementActionTypes.RequireEmperorAlliance => false,
            RequirementActionTypes.RequireFremenCardInPlay => false,
            RequirementActionTypes.RequireBenneGesseritCardInPlay => false,
            RequirementActionTypes.RequireSpacingGuildCardInPlay => false,
            RequirementActionTypes.RequireEmperorCardInPlay => false,
            RequirementActionTypes.RequireMakerHook => playerData.Resources[ResourceType.MakerHook] >= requirement.Quantity,
            RequirementActionTypes.RequireSwordsman => !playerData.HaveSwordsman && _networkGameplayService.GameData.Players.Any(x => x.HaveSwordsman),
            RequirementActionTypes.RequireNoSwordsman => _networkGameplayService.GameData.Players.All(x => !x.HaveSwordsman),
            RequirementActionTypes.RequireNoCouncilSeat => !playerData.HasCouncilSeat,
            RequirementActionTypes.RequireCouncilSeat => playerData.HasCouncilSeat,
        };
        return meetsRequirement;
    }

    bool CanVisitArea(PlayerData playerData, AgentAreaDefinition agentArea)
    {
        if (!agentArea)
            return false;

        return CheckRequirement(playerData, agentArea.Requirement);
    }
}
