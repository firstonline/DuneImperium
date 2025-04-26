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
        var clientId = (ulong)rpcParams.Receive.SenderClientId;


        var gameData = _networkGameplayService.GameData;
        var playerData = gameData.Players[(int)clientId];

        if (CanVisitArea(playerData, agentArea))
        {
            var exchange = agentArea.Exchanges[selectedExchange];
            bool canPay = CanPay(playerData, exchange);

            if (canPay)
            {
                PayCost(ref playerData, exchange);
                ReceiveRewards(ref playerData, exchange);

                gameData.Players[(int)clientId] = playerData;
                gameData.RandomData = gameData.RandomData + 1; // this enforce network variable change
                _networkGameplayService.UpdateGameData(gameData);
            }

            VisitAgentAreaResponseClientRpc(canPay, rpcParams.Receive.SenderClientId);
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
                CostActionTypes.RemoveSpice => playerData.Resoureces[ResourceType.Spice] >= cost.Quantity,
                CostActionTypes.RemoveWater => playerData.Resoureces[ResourceType.Water] >= cost.Quantity,
                CostActionTypes.RemoveSolari => playerData.Resoureces[ResourceType.Solari] >= cost.Quantity,
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
                case CostActionTypes.RemoveSpice: playerData.Resoureces[ResourceType.Spice] -= cost.Quantity; break;
                case CostActionTypes.RemoveWater: playerData.Resoureces[ResourceType.Water] -= cost.Quantity; break;
                case CostActionTypes.RemoveSolari: playerData.Resoureces[ResourceType.Solari] -= cost.Quantity; break;
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
                    playerData.Resoureces[ResourceType.Spice] += reward.Quantity;
                    break;

                case RewardActionTypes.AddWater:
                    playerData.Resoureces[ResourceType.Water] += reward.Quantity;
                    break;

                case RewardActionTypes.AddSolari:
                    playerData.Resoureces[ResourceType.Solari] += reward.Quantity;
                    break;

                case RewardActionTypes.AddFremenInfluence:
                    playerData.Resoureces[ResourceType.FremenInfluence] += reward.Quantity;
                    break;

                case RewardActionTypes.AddBenneGesseritInfluence:
                    playerData.Resoureces[ResourceType.BeneGesseritInfluence] += reward.Quantity;
                    break;

                case RewardActionTypes.AddSpacingGuildInfluence:
                    playerData.Resoureces[ResourceType.SpacingGuildInfluence] += reward.Quantity;
                    break;

                case RewardActionTypes.AddEmperorInfluence:
                    playerData.Resoureces[ResourceType.EmperorInfluence] += reward.Quantity;
                    break;

                case RewardActionTypes.AddVictoryPoints:
                    break;

                case RewardActionTypes.AddAgent:
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
            RequirementActionTypes.RequireFrementInfluence => playerData.Resoureces[ResourceType.FremenInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireBeneGesseritInfluence => playerData.Resoureces[ResourceType.BeneGesseritInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireSpacingGuildInfluence => playerData.Resoureces[ResourceType.SpacingGuildInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireEmperorInfluence => playerData.Resoureces[ResourceType.EmperorInfluence] >= requirement.Quantity,
            RequirementActionTypes.RequireFremenAlliance => false,
            RequirementActionTypes.RequireBeneGesseritAlliance => false,
            RequirementActionTypes.RequireSpacingGuildAlliance => false,
            RequirementActionTypes.RequireEmperorAlliance => false,
            RequirementActionTypes.RequireFremenCardInPlay => false,
            RequirementActionTypes.RequireBenneGesseritCardInPlay => false,
            RequirementActionTypes.RequireSpacingGuildCardInPlay => false,
            RequirementActionTypes.RequireEmperorCardInPlay => false,
            RequirementActionTypes.RequireMakerHook => false,
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
