using NaughtyAttributes;
using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AgentAreaBehaviour : MonoBehaviour
{
    [SerializeField] AgentAreaDefinition _definition;
    [FormerlySerializedAs("exchangePrefab"), SerializeField] ExchangeBehaviour _exchangePrefab;
    [SerializeField] Transform _exchangesParent;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] ImperialFlagBehaviour _imperialFlagBehaviour;
    [SerializeField] Image _agentIcon;
    [SerializeField] Image _combatIcon;
    [SerializeField] CostBehaviour _cost;

    Button _button;

    public IObservable<Unit> ObserveClicked() => _button.OnClickAsObservable();
    public AgentAreaDefinition Definition => _definition;

    void Awake()
    {
        _button = GetComponent<Button>();
        Setup();
    }

    [Button]
    public void Setup()
    {
        _name.text = _definition.Name;
        var exchangesPool = new PrefabsPool<ExchangeBehaviour>(_exchangePrefab, _exchangesParent, 10);

        var exchangeWithCost = _definition.Exchanges.FirstOrDefault(x => x.Costs.Count > 0);
        bool differentCosts = _definition.Exchanges.Count > 1 && exchangeWithCost != null;


        if (exchangeWithCost != null)
        {
            _cost.Setup(exchangeWithCost.Costs[0], differentCosts);
            _cost.gameObject.SetActive(true);
        }
        else
        {
            _cost.gameObject.SetActive(false);
        }

        foreach (var exchange in _definition.Exchanges)
        {
            var exchangeBehaviour = exchangesPool.Get();
            exchangeBehaviour.Setup(exchange, !differentCosts);
        }

        _imperialFlagBehaviour.Setup(_definition.ImperialFlagReward);

        _agentIcon.sprite = _definition.AgentIcon.Icon;
        _agentIcon.color = _definition.AgentIcon.Color;
        _combatIcon.gameObject.SetActive(_definition.IsCombatArea);

        EditorUtils.SetDirty(_agentIcon);
        EditorUtils.SetDirty(_combatIcon);
        EditorUtils.SetDirty(gameObject);
    }

}
