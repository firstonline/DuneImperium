using System;
using System.Linq;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;

public class AreasService
{
    [Inject] BehaviorSubject<GameData> _gameData;
    [Inject] AgentAreaDatabase _agentAreaDatabase;
    [Inject] CardsDatabase _cardsDatabase;

    public void VisitAgentArea(int playerIndex, int cardId, int areaId, int selectedExchange)
    {
        var gameData = _gameData.Value;

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
                playerData.PerformedAction = true;
                gameData.Players[playerIndex] = playerData;

                AllianceUtils.RecalculateAlliance(ref gameData);
                _gameData.OnNext(gameData);
            }
        }
    }
}
