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
    [Inject] AreasService _areaService;
    [Inject] NetworkGameplayService _gameplayService;

    [SerializeField] Button _deployTroopsBtn;
    [SerializeField] SelectExchangePopup _selectExchangePopup;
    [SerializeField] DeployTroopsPopup _deployTroopsPopup;

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
            var player = _gameplayService.GameData.Players.First(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
            _deployTroopsPopup.Show((count) => { _gameplayService.DeployTroops(count); }, player.DeployableTroopsCount);
        }).AddTo(_disposables);

        _gameplayService.ObserveLocalPlayerData().Subscribe(x =>
        {
            _deployTroopsBtn.gameObject.SetActive(x.CanDeploy);
        }).AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

    void OnAreaClicked(AgentAreaDefinition definition)
    {
        if(definition.Exchanges.Count > 1)
        {
            _selectExchangePopup.Show(definition.Exchanges, (index) =>
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
