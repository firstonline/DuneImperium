using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostBehaviour : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _quantity;
    public void Setup(CostDefinition cost, bool isVariable = false)
    {
        switch (cost.CostType)
        {
            case CostType.Resource:
                _icon.sprite = cost.Resource.Icon;
                break;
            case CostType.DiscardCard:
                break;
            default:
                break;
        }
        _quantity.text = isVariable ? "?" : cost.Quantity.ToString();
        EditorUtils.SetDirty(_quantity);
        EditorUtils.SetDirty(_icon);
    }
}
