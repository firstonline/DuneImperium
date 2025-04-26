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
    [SerializeField] ExchangeBehaviour[] _exchanges;
    [SerializeField] AgentAreaDefinition _definition;
    [SerializeField] Transform _exchangesParent;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] ImperialFlagBehaviour _imperialFlagBehaviour;
    [SerializeField] Image _agentIcon;
    [SerializeField] Image _combatIcon;
    [SerializeField] CostBehaviour _cost;
    [SerializeField] RequirementBehaviour _requirement;

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

        int? cost = null;

        var exchangeWithCost = _definition.Exchanges.FirstOrDefault(x => x.Costs.Count > 0);
        bool sameCosts = true;

        foreach ( var exchanges in _definition.Exchanges)
        {
            int currentCost = 0;
            if (exchanges.Costs.Count == 0)
            {
                currentCost = 0;
            }
            else
            {
                currentCost = exchanges.Costs[0].Quantity;
            }

            if (cost == null)
            {
                cost = currentCost;
            }
            else if (cost != currentCost)
            {
                sameCosts = false;
                break;
            }
        }


        if (exchangeWithCost != null)
        {
            _cost.Setup(exchangeWithCost.Costs[0], !sameCosts);
            _cost.gameObject.SetActive(true);
        }
        else
        {
            _cost.gameObject.SetActive(false);
        }

        if (_definition.Requirement.Quantity > 0)
        {
            _requirement.gameObject.SetActive(true);
            _requirement.Setup(_definition.Requirement);
        }
        else
        {
            _requirement.gameObject.SetActive(false);
        }

        UnityUtils.HideAllChildren(_exchangesParent);
        for (int i = 0; i < _definition.Exchanges.Count; i++)
        {
            var exchange = _definition.Exchanges[i];
            var exchangeBehaviour = _exchanges[i];
            exchangeBehaviour.Setup(exchange, sameCosts, false);
            exchangeBehaviour.gameObject.SetActive(true);
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
