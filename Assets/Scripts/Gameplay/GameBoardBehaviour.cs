using System;
using System.Collections.Generic;
using System.Linq;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [Inject] AgentsPhaseService _agentsPhaseService;

    [SerializeField] Button _deployTroopsBtn;
    [SerializeField] SelectExchangePopup _selectExchangePopup;
    [SerializeField] DeployTroopsPopup _deployTroopsPopup;
    [SerializeField] PlayerHandBehaviour _playerHand;

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

        _deployTroopsBtn.OnClickAsObservable().Subscribe(_ =>
        {
            _deployTroopsPopup.Show((count) => { _agentsPhaseService.DeployTroops(count); }, _agentsPhaseService.DeployableTroopsCount);
        }).AddTo(_disposables);

        _agentsPhaseService.ObserveCanDeploy().Subscribe(canDeploy =>
        {
            _deployTroopsBtn.gameObject.SetActive(canDeploy);
        }).AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

    void OnAreaClicked(AgentAreaDefinition definition)
    {
        if (_playerHand.SelectedCard == null)
            return;

        if(definition.Exchanges.Count > 1)
        {
            _selectExchangePopup.Show(definition.Exchanges, (index) =>
            {
                _agentsPhaseService.VisitAgentArea(_playerHand.SelectedCard.ID, definition.ID, index);

            });
        }
        else
        {
            _agentsPhaseService.VisitAgentArea(_playerHand.SelectedCard.ID, definition.ID, 0);
        }
       
    }
}
