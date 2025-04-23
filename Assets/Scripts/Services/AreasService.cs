using System;
using UniDi;
using Unity.Netcode;
using UnityEditor.PackageManager;
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


        if (CanVisitArea(clientId, agentArea, selectedExchange))
        {
            bool canPay = true;

            var gameData = _networkGameplayService.GameData;
            var playerData = gameData.Players[(int)clientId];
            var exchange = agentArea.Exchanges[selectedExchange];

            if (CanPay(playerData, exchange))
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
                CostActionTypes.RemoveSpice => playerData.Inventory[ItemType.Spice] >= cost.Quantity,
                CostActionTypes.RemoveWater => playerData.Inventory[ItemType.Water] >= cost.Quantity,
                CostActionTypes.RemoveSolari => playerData.Inventory[ItemType.Solari] >= cost.Quantity,
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
                case CostActionTypes.RemoveSpice: playerData.Inventory[ItemType.Spice] -= cost.Quantity; break;
                case CostActionTypes.RemoveWater: playerData.Inventory[ItemType.Water] -= cost.Quantity; break;
                case CostActionTypes.RemoveSolari: playerData.Inventory[ItemType.Solari] -= cost.Quantity; break;
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
                    break;

                case RewardActionTypes.AddSpice:
                    playerData.Inventory[ItemType.Spice] += reward.Quantity;
                    break;

                case RewardActionTypes.AddWater:
                    playerData.Inventory[ItemType.Water] += reward.Quantity;
                    break;

                case RewardActionTypes.AddSolari:
                    playerData.Inventory[ItemType.Solari] += reward.Quantity;
                    break;

                case RewardActionTypes.AddFremenInfluence:
                    break;

                case RewardActionTypes.AddBenneGesseritInfluence:
                    break;

                case RewardActionTypes.AddSpacingGuildInfluence:
                    break;

                case RewardActionTypes.AddEmperorInfluence:
                    break;

                case RewardActionTypes.AddVictoryPoints:
                    break;

                case RewardActionTypes.AddAgent:
                    break;

                case RewardActionTypes.RecallAgent:
                    break;

                case RewardActionTypes.AddWorm:
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

    //void CheckRequirement()
    //{
    //    switch (requirement)
    //    {
    //        case RequirementActionTypes.RequireFrementInfluence:
    //            break;

    //        case RequirementActionTypes.RequireBeneGesseritInfluence:
    //            break;

    //        case RequirementActionTypes.RequireSpacingGuildInfluence:
    //            break;

    //        case RequirementActionTypes.RequireEmperorInfluence:
    //            break;

    //        case RequirementActionTypes.RequireFremenAlliance:
    //            break;

    //        case RequirementActionTypes.RequireBeneGesseritAlliance:
    //            break;

    //        case RequirementActionTypes.RequireSpacingGuildAlliance:
    //            break;

    //        case RequirementActionTypes.RequireEmperorAlliance:
    //            break;

    //        case RequirementActionTypes.RequireFremenCardInPlay:
    //            break;

    //        case RequirementActionTypes.RequireBenneGesseritCardInPlay:
    //            break;

    //        case RequirementActionTypes.RequireSpacingGuildCardInPlay:
    //            break;

    //        case RequirementActionTypes.RequireEmperorCardInPlay:
    //            break;

    //        case RequirementActionTypes.RequireMakerHook:
    //            break;
    //    }
    //}

    bool CanVisitArea(ulong clientId, AgentAreaDefinition agentArea, int selectedExchange)
    {
        if (!agentArea)
            return false;

        return true;
    }
}
