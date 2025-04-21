using System.Collections;
using UniDi;
using UnityEngine;

public class ExchangeBehaviour : MonoBehaviour
{
    [SerializeField] Transform _rewardsParent;
    [SerializeField] Transform _costParent;
    [SerializeField] RewardBehaviour _rewardPrefab;
    [SerializeField] CostBehaviour _costPrefab;
    [SerializeField] GameObject _separator;

    public void Setup(ExchangeDefinition exchange, bool hideCost = false)
    {
        var rewardsPool = new PrefabsPool<RewardBehaviour>(_rewardPrefab, _rewardsParent, 10);
        var costsPool = new PrefabsPool<CostBehaviour>(_costPrefab, _costParent, 10);

        foreach (var reward in exchange.Rewards)
        {
            var rewardBehaviour = rewardsPool.Get();
            rewardBehaviour.Setup(reward);
        }
        if (hideCost)
        {
            _separator.gameObject.SetActive(false);
        }
        else
        {
            if (exchange.Costs.Count == 0)
            {
                _separator.gameObject.SetActive(false);
            }
            else
            {
                _separator.gameObject.SetActive(true);

                foreach (var cost in exchange.Costs)
                {
                    var costBehaviour = costsPool.Get();
                    costBehaviour.Setup(cost);
                }
            }
        }
       

        EditorUtils.SetDirty(gameObject);
    }
}
