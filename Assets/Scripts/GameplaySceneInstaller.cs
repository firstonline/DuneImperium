using UniDi;
using UnityEngine;

public class GameplaySceneInstaller : MonoInstaller<GameplaySceneInstaller>
{
    [SerializeField] NetworkGameplayService _networkGameplay;
    [SerializeField] AreasService _areaService;
    [SerializeField] AgentAreaDatabase _agentAreaDatabase;
    [SerializeField] ResourcesDatabase _resourcesDatabase;

    public override void InstallBindings()
    {
        _agentAreaDatabase.Setup();
        _resourcesDatabase.Setup();
        Container.Bind<AgentAreaDatabase>().FromInstance(_agentAreaDatabase);
        Container.Bind<ResourcesDatabase>().FromInstance(_resourcesDatabase);
        Container.Bind<NetworkGameplayService>().FromInstance(_networkGameplay);
        Container.Bind<AreasService>().FromInstance(_areaService);
    }
}
