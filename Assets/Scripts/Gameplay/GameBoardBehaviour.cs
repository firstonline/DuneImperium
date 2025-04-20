using System.Collections.Generic;
using UniDi;
using UniRx;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Inject] AreasService _areaService;

    CompositeDisposable _disposables = new();

    AgentAreaBehaviour[] _agentAreas;

    void Awake()
    {
        _agentAreas = gameObject.GetComponentsInChildren<AgentAreaBehaviour>();

        foreach (var agentArea in _agentAreas)
        {
            agentArea.ObserveClicked().Subscribe(_ => OnAreaClicked(agentArea.Definition)).AddTo(_disposables);
        }
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

    void OnAreaClicked(AgentAreaDefinition definition)
    {
        _areaService.VisitAgentArea(definition.ID, definition.Exchanges.Count - 1);
    }
}
