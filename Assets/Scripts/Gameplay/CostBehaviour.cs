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
        _icon.sprite = cost.Action.Icon;

        if (cost.Quantity == 1 
            && cost.Action.Type != CostActionTypes.RemoveSolari
            && cost.Action.Type != CostActionTypes.RemoveSpice)
        {
            _quantity.text = "";
        }
        else
        {
            _quantity.text = isVariable ? "?" : cost.Quantity.ToString();
        }

        EditorUtils.SetDirty(_quantity);
        EditorUtils.SetDirty(_icon);
    }
}
