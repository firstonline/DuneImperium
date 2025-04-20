using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class ExchangeBehaviour : MonoBehaviour
{
    [SerializeField] Transform _rewardsParent;
    [SerializeField] Transform _costParent;
    [SerializeField] RewardBehaviour _rewardPrefab;
    [SerializeField] CostBehaviour _costPrefab;
    [SerializeField] GameObject _separator;

    public void Setup(ExchangeDefinition exchange)
    {
        UnityUtils.HideAllChildren(_rewardsParent);
        UnityUtils.HideAllChildren(_costParent);

        foreach (var reward in exchange.Rewards)
        {
            var rewardBehaviour = Instantiate(_rewardPrefab, _rewardsParent);
            rewardBehaviour.Setup(reward);
        }

        if (exchange.Costs.Count == 0)
        {
            _separator.gameObject.SetActive(false);
        }
        else
        {
            _separator.gameObject.SetActive(true);

            foreach (var cost in exchange.Costs)
            {
                var costBehaviour = Instantiate(_costPrefab, _costParent);
                costBehaviour.Setup(cost);
            }
        }


        EditorUtils.SetDirty(gameObject);
    }
}
