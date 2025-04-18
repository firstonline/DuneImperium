using UnityEngine;

[CreateAssetMenu(fileName = "Intrigue", menuName = "ScriptableObjects/Resources/Intrigue")]
public class IntrigueDefinition : BaseResourceDefinition
{
    public override ResourceType ResourceType => ResourceType.Intrigue;
}
