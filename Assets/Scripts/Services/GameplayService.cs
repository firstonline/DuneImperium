using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UniDi;
using UniRx;
using Unity.Multiplayer.Widgets;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    Setup = 0,
    AgentsPhase = 1,
    Combat = 2,
    PostCombat = 3,
    EndGame = 4,
}

public class GameplayService : NetworkBehaviour
{
    [Inject] BehaviorSubject<GameData> _gameData;
    [Inject] SetupPhaseService _setupPhaseService;
    [Inject] AgentsPhaseService _agentsPhaseService;
    [Inject] CombatPhaseService _combatPhaseService;
    [Inject] PostCombatPhaseService _postCombatPhaseService;
    [Inject] ResetPhaseService _resetPhaseService;

    [SerializeField] WidgetConfiguration _widgetConfiguration;

    CancellationTokenSource _cancelationTokenSource;
    int _completedCount;

    public int PlayersCount => _widgetConfiguration.MaxPlayers;
    public GameData GameData => _gameData.Value;

    public IObservable<GameData> ObserveGameData() => _gameData;
    public IObservable<PlayerData> ObservePlayerData(int index) => _gameData.Select(x =>
    {
        if (x.Players != null && x.Players.Count > index)
        {
            return x.Players[index];

        }
        else
        {
            return PlayerData.Construct();
        }
    });

    public IObservable<PlayerData> ObserveLocalPlayerData() => _gameData.Select(x =>
    {
        if (x.Players != null && x.Players.Any(x => x.IsTaken && x.ClientId == NetworkManager.Singleton.LocalClientId))
        {
            var localPlayerData = x.Players.First(x => x.IsTaken && x.ClientId == NetworkManager.Singleton.LocalClientId);
            return localPlayerData;
        }
        else
        {
            return PlayerData.Construct();
        }
    });

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton == null)
        {
            throw new Exception($"");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            GamePlayLoop().Forget();
        }
    }

    public override void OnDestroy()
    {
        if (_cancelationTokenSource != null)
        {
            _cancelationTokenSource.Cancel();
        }
    }

    async UniTask GamePlayLoop()
    {
        _cancelationTokenSource = new CancellationTokenSource();

        // Setup Phase
        await _setupPhaseService.Process(_cancelationTokenSource);

        // check victory points count
        while (true)
        {
            Debug.Log("Agents Phase");
            // Player Phase
            await _agentsPhaseService.Process(_cancelationTokenSource);

            Debug.Log("Combat Phase");
            // Combat Phase
            await _combatPhaseService.Process(_cancelationTokenSource);

            Debug.Log("Post Combat Phase");
            // Post Combat Phase
            await _postCombatPhaseService.Process(_cancelationTokenSource);

            Debug.Log("Reset Phase");
            // Reset Phase
            await _resetPhaseService.Process(_cancelationTokenSource);
        }


        // End Game Phase
    }
}
