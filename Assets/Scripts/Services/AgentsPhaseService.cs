using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UniDi;
using UniRx;
using Unity.Netcode;

public class AgentsPhaseService : PhaseServiceBase
{
    [Inject] BehaviorSubject<GameData> _gameData;
    [Inject] AreasService _areasService;

    public int DeployableTroopsCount
    {
        get
        {
            var currentPlayer = _gameData.Value.Players.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
            return currentPlayer.DeployableTroopsCount;
        }
    }

    public IObservable<bool> ObserveCanDeploy() => _gameData.Select(x =>
    {
        if (_gameData.Value.Players == null)
        {
            return false;
        }

        var currentPlayer = _gameData.Value.Players.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        return currentPlayer.CanDeploy;
    });

   
    public override async UniTask Process(CancellationTokenSource cancellationTokenSource)
    {
        await UniTask.WaitUntil(() => _gameData.Value.Players.Any(x => x.Revealed), cancellationToken: cancellationTokenSource.Token);
    }

    public void VisitAgentArea(int cardId, int areaId, int selectedExchange)
    {
        VisitAgentAreaServerRpc(cardId, areaId, selectedExchange);
    }

    [ServerRpc(RequireOwnership = false)]
    void VisitAgentAreaServerRpc(int cardId, int areaId, int selectedExchange, ServerRpcParams rpcParams = default)
    {
        int playerIndex = -1; ;
        var gameData = _gameData.Value;

        for (int i = 0; i < gameData.Players.Count; i++)
        {
            if (gameData.Players[i].ClientId == rpcParams.Receive.SenderClientId)
            {
                playerIndex = i;
                break;
            }
        }

        if (playerIndex != gameData.CurrentPlayerIndex)
        {
            return;
        }
        _areasService.VisitAgentArea(playerIndex, cardId, areaId, selectedExchange);
    }

    public void DeployTroops(int count)
    {
        DeployTroopsServerRpc(count);
    }

    [ServerRpc(RequireOwnership = false)]
    void DeployTroopsServerRpc(int count, ServerRpcParams rpcParams = default)
    {
        var gameData = _gameData.Value;
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

        if (playerData.DeployableTroopsCount <= count)
        {
            playerData.DeployableTroopsCount -= count;
            playerData.GarrisonedTroopsCount -= count;
            playerData.DeployedTroopsCount += count;

            gameData.Players[playerIndex] = playerData;
            _gameData.OnNext(gameData);
        }
    }

    public void Reveal()
    {
        RevealServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void RevealServerRpc(ServerRpcParams rpcParams = default)
    {
        var gameData = _gameData.Value;
        var currentPlayer = gameData.Players[gameData.CurrentPlayerIndex];

        if (currentPlayer.ClientId == rpcParams.Receive.SenderClientId)
        {
            currentPlayer.Revealed = true;
            currentPlayer.PerformedAction = true;
            gameData.Players[gameData.CurrentPlayerIndex] = currentPlayer;
            // add some reveal logic
        }
        _gameData.OnNext(gameData);
    }
    public void EndTurn()
    {
        EndTurnServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void EndTurnServerRpc(ServerRpcParams rpcParams = default)
    {
        var gameData = _gameData.Value;
        var currentPlayer = gameData.Players[gameData.CurrentPlayerIndex];

        if (currentPlayer.ClientId == rpcParams.Receive.SenderClientId && !currentPlayer.PerformedAction)
        {
            currentPlayer.PerformedAction = false;
            gameData.Players[gameData.CurrentPlayerIndex] = currentPlayer;

            if (gameData.Players.All(x => x.Revealed))
            {
                gameData.GameState = GameState.Combat;
                gameData.CurrentPlayerIndex = gameData.FirstPlayerIndex;
                gameData.LastPlayedCombatPlayer = gameData.CurrentPlayerIndex;
                _gameData.OnNext(gameData);
            }
            else
            {
                for (int i = 1; i <= gameData.Players.Count; i++)
                {
                    var nextPlayerIndex = (gameData.CurrentPlayerIndex + i) % gameData.Players.Count;
                    var nextAvailablePlayer = gameData.Players[i];
                    if (!nextAvailablePlayer.Revealed)
                    {
                        gameData.CurrentPlayerIndex = nextPlayerIndex;
                    }
                }
                _gameData.OnNext(gameData);
            }
        }
    }
}
