using UniDi;
using UnityEngine;

public class GameplaySceneInstaller : MonoInstaller<GameplaySceneInstaller>
{
    [SerializeField] NetworkGameplayService _networkGameplay;
    [SerializeField] AreasService _areaService;
    [SerializeField] AgentAreaDatabase _agentAreaDatabase;

    public override void InstallBindings()
    {
        _agentAreaDatabase.Setup();
        Container.Bind<AgentAreaDatabase>().FromInstance(_agentAreaDatabase);
        Container.Bind<NetworkGameplayService>().FromInstance(_networkGameplay);
        Container.Bind<AreasService>().FromInstance(_areaService);
    }
}
