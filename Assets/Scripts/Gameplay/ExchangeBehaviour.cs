using System.Collections;
using UniDi;
using UnityEngine;

public class ExchangeBehaviour : MonoBehaviour
{
    [SerializeField] Transform _rewardsParent;
    [SerializeField] RewardBehaviour[] _rewards;
    [SerializeField] Transform _costParent;
    [SerializeField] CostBehaviour[] _costs;
    [SerializeField] GameObject _separator;

    public void Setup(ExchangeDefinition exchange, bool hideCost = false, bool showInfluence = true)
    {
        UnityUtils.HideAllChildren(_rewardsParent);
        UnityUtils.HideAllChildren(_costParent);

        for (int i = 0; i < exchange.Rewards.Count; i++)
        {
            var reward = exchange.Rewards[i];
            if (!showInfluence && 
                (reward.Action.Type == RewardActionTypes.AddBenneGesseritInfluence
                    || reward.Action.Type == RewardActionTypes.AddFremenInfluence
                    || reward.Action.Type == RewardActionTypes.AddSpacingGuildInfluence
                    || reward.Action.Type == RewardActionTypes.AddEmperorInfluence))
            {
                continue;
            }    
            var rewardBehaviour = _rewards[i];
            rewardBehaviour.Setup(reward);
            rewardBehaviour.gameObject.SetActive(true);
        }

        if (hideCost || exchange.Costs.Count == 0)
        {
            _separator.gameObject.SetActive(false);
            _costParent.gameObject.SetActive(false);
        }
        else
        {
            _separator.gameObject.SetActive(true);
            _costParent.gameObject.SetActive(true);

            for (int i = 0; i < exchange.Costs.Count; i++)
            {
                var cost = exchange.Costs[i];
                var costBehaviour = _costs[i];
                costBehaviour.Setup(cost);
                costBehaviour.gameObject.SetActive(true);
            }
        }
       

        EditorUtils.SetDirty(gameObject);
    }
}
