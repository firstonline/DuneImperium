using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class PostCombatPhaseService : PhaseServiceBase
{
    [Inject] BehaviorSubject<GameData> _gameData;
    int _completedCount;

    public override async UniTask Process(CancellationTokenSource cancellationToken)
    {
    }



    [ServerRpc(RequireOwnership = false)]
    public void PostCombatCompleteServerRpc(ServerRpcParams rpcParams = default)
    {
        var gameData = _gameData.Value;
        _completedCount++;
        if (_completedCount >= gameData.Players.Count)
        {
            for (int i = 0; i < gameData.Players.Count; i++)
            {
                var player = gameData.Players[i];
                player.DeployedAgentsCount = 0;
                player.DeployableTroopsCount = 0;
                player.PerformedAction = false;
                player.Revealed = false;
                player.CanDeploy = false;

                gameData.Players[i] = player;
            }
            gameData.FirstPlayerIndex = (gameData.FirstPlayerIndex + 1) % gameData.Players.Count;
            gameData.CurrentPlayerIndex = gameData.FirstPlayerIndex;
            gameData.GameState = GameState.AgentsPhase;
            _completedCount = 0;
            _gameData.OnNext(gameData);
        }
    }
}
