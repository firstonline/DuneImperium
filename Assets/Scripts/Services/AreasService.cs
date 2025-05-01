using System;
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

        var agentArea = _agentAreaDatabase.GetItem(areaId);
        var playerData = gameData.Players[playerIndex];

        if (ExchangeHelper.CanVisitArea(gameData, playerData, agentArea))
        {
            var exchange = agentArea.Exchanges[selectedExchange];
            bool allowedToBuy = exchange.Requirements.All(x => ExchangeHelper.CheckRequirement(gameData, playerData, x));
            bool canPay = ExchangeHelper.CanPayCost(playerData, exchange);
            bool canGainRewards = ExchangeHelper.CanGainRewards(gameData, playerData, exchange);

            if (allowedToBuy && canPay && canGainRewards)
            {
                ExchangeHelper.PayCost(ref playerData, exchange);
                ExchangeHelper.ReceiveRewards(ref playerData, exchange);
                gameData.Players[playerIndex] = playerData;

                gameData.RandomData = gameData.RandomData + 1; // this enforce network variable change

                AllianceUtils.RecalculateAlliance(ref gameData);
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
}
