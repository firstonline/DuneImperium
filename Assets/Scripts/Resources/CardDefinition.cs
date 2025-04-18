using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Resources/Card")]
public class CardDefinition : BaseResourceDefinition
{
    public override ResourceType ResourceType => ResourceType.Card;
}
