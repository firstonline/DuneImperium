using System.Collections.Generic;
using System.Linq;
using UniDi;
using UniRx;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Inject] AreasService _areaService;

    CompositeDisposable _disposables = new();

    List<AgentAreaBehaviour> _agentAreas;

    void Awake()
    {
        _agentAreas = FindObjectsByType<AgentAreaBehaviour>(FindObjectsSortMode.None).ToList();

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
        _areaService.VisitAgentArea(definition.ID, 0);
    }
}
