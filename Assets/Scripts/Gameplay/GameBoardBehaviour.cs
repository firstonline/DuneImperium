using System;
using System.Collections.Generic;
using System.Linq;
using UniDi;
using UniRx;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Inject] AreasService _areaService;
    [SerializeField] SelectExchangePopup _selectExchangePopup;

    CompositeDisposable _disposables = new();

    List<AgentAreaBehaviour> _agentAreas;

    void Awake()
    {
        _selectExchangePopup.gameObject.SetActive(false);

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
        if(definition.Exchanges.Count > 1)
        {
            _selectExchangePopup.Show((index) =>
            {
                _areaService.VisitAgentArea(definition.ID, index);

            });
        }
        else
        {
            _areaService.VisitAgentArea(definition.ID, 0);
        }
       
    }
}
