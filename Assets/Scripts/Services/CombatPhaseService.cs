using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class CombatPhaseService : PhaseServiceBase
{
    [Inject] BehaviorSubject<GameData> _gameData;

    public override async UniTask Process(CancellationTokenSource cancellationToken)
    {
    }

    [ServerRpc(RequireOwnership = false)]
    void EndCombatTurnServerRpc(ServerRpcParams rpcParams = default)
    {
        var gameData = _gameData.Value;
        var currentPlayer = gameData.Players[gameData.CurrentPlayerIndex];

        if (currentPlayer.ClientId == rpcParams.Receive.SenderClientId)
        {
            gameData.Players[gameData.CurrentPlayerIndex] = currentPlayer;
            gameData.CurrentPlayerIndex = (gameData.CurrentPlayerIndex + 1) % gameData.Players.Count;

            if (gameData.CurrentPlayerIndex == gameData.LastPlayedCombatPlayer)
            {
                gameData.GameState = GameState.PostCombat;
            }
            _gameData.OnNext(gameData);
        }
    }
}
