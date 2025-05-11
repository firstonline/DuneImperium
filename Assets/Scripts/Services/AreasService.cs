using System;
using System.Linq;
using UniDi;
using Unity.Netcode;
using UnityEngine;

public class AreasService : NetworkBehaviour
{
    [Inject] AgentAreaDatabase _agentAreaDatabase;
    [Inject] CardsDatabase _cardsDatabase;
    [Inject] NetworkGameplayService _networkGameplayService;

    public void VisitAgentArea(CardDefinition card, int areaId, int selectedExchange)
    {
        VisitAgentAreaServerRpc(card.ID, areaId, selectedExchange);
    }

    [ServerRpc(RequireOwnership = false)]
    void VisitAgentAreaServerRpc(int cardId, int areaId, int selectedExchange, ServerRpcParams rpcParams = default)
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
        var card = _cardsDatabase.GetItem(cardId);
        var playerData = gameData.Players[playerIndex];
        playerData.CanDeploy = false;
        playerData.DeployableTroopsCount = 0;

        if (ExchangeHelper.CanVisitArea(card, gameData, playerData, agentArea))
        {
            var exchange = agentArea.Exchanges[selectedExchange];
            bool isValid = ExchangeHelper.IsExchangeValid(gameData, playerData, exchange);

            if (isValid)
            {
                if (agentArea.IsCombatArea)
                {
                    ExchangeHelper.MakeCanDeploy(ref playerData);
                }

                ExchangeHelper.PayCost(ref playerData, exchange);
                ExchangeHelper.ReceiveRewards(ref playerData, exchange);
                gameData.Players[playerIndex] = playerData;

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
