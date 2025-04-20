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
        switch (reward.RewardType)
        {
            case RewardType.Resource:
                _icon.sprite = reward.Resource.Icon;
                break;
            default:
                break;
        }
        _quantity.text = reward.Quantity.ToString();
        EditorUtils.SetDirty(_quantity);
        EditorUtils.SetDirty(_icon);
    }
}
