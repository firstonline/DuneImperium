using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequirementBehaviour : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _quantity;
    
    public void Setup(RequirementDefinition requirement)
    {
        _icon.sprite = requirement.Action.Icon;
        _quantity.text = requirement.Quantity > 1 ? requirement.Quantity.ToString() : "";

        EditorUtils.SetDirty(_icon);
        EditorUtils.SetDirty(_quantity);
    }
}
