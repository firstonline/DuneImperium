using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardBehaviour : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _quantity;
    public void Setup(RewardDefinition reward)
    {
        _icon.sprite = reward.Action.Icon;

        if (reward.Quantity == 1
            && reward.Action.Type != RewardActionTypes.AddSolari
            && reward.Action.Type != RewardActionTypes.AddSpice)
        {
            _quantity.text = "";
        }
        else
        {
            _quantity.text = reward.Quantity.ToString();
        }

      
        EditorUtils.SetDirty(_quantity);
        EditorUtils.SetDirty(_icon);
    }
}
