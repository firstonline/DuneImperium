using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniDi;
using Unity.Multiplayer.Widgets;
using UnityEngine;
using System.Linq;
using UniRx;

public class SetupPhaseService : PhaseServiceBase
{
    [Inject] CardsDatabase _cardsDatabase;
    [Inject] BehaviorSubject<GameData> _gameData;

    [SerializeField] WidgetConfiguration _widgetConfiguration; 

    public override async UniTask Process(CancellationTokenSource cancellationToken)
    {
        var gameData = new GameData();
        gameData.Players = new List<PlayerData> { };
        for (int i = 0; i < _widgetConfiguration.MaxPlayers; i++)
        {
            var playerData = PlayerData.Construct(_cardsDatabase.StartingDeck.Select(x => x.ID).ToList());

            if (i == 0)
            {
                playerData.IsTaken = true;
            }

            gameData.Players.Add(playerData);
        }
        gameData.GameState = GameState.AgentsPhase;
        _gameData.OnNext(gameData);
    }
}
