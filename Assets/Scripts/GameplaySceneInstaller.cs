using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class GameplaySceneInstaller : MonoInstaller<GameplaySceneInstaller>
{
    [SerializeField] NetworkGameplayService _networkGameplay;
    [SerializeField] GameplayService _gameplayService;
    [SerializeField] AgentAreaDatabase _agentAreaDatabase;
    [SerializeField] CardsDatabase _cardDatabase;
    [SerializeField] SetupPhaseService _setupPhaseService;
    [SerializeField] AgentsPhaseService _agentsPhaseService;
    [SerializeField] CombatPhaseService _combatPhaseService;
    [SerializeField] PostCombatPhaseService _postCombatPhaseService;
    [SerializeField] ResetPhaseService _resetPhaseService;

    BehaviorSubject<GameData> _gameData = new(new GameData());

    public override void InstallBindings()
    {
        _agentAreaDatabase.Setup();
        _cardDatabase.Setup();
        Container.Bind<BehaviorSubject<GameData>>().FromInstance(_gameData);
        Container.Bind<AgentAreaDatabase>().FromInstance(_agentAreaDatabase);
        Container.Bind<CardsDatabase>().FromInstance(_cardDatabase);
        Container.Bind<NetworkGameplayService>().FromInstance(_networkGameplay);

        Container.Bind<GameplayService>().FromInstance(_gameplayService);
        Container.Bind<SetupPhaseService>().FromInstance(_setupPhaseService);
        Container.Bind<AgentsPhaseService>().FromInstance(_agentsPhaseService);
        Container.Bind<CombatPhaseService>().FromInstance(_combatPhaseService);
        Container.Bind<PostCombatPhaseService>().FromInstance(_postCombatPhaseService);
        Container.Bind<ResetPhaseService>().FromInstance(_resetPhaseService);

        Container.Bind<AreasService>().AsSingle();
    }
}
