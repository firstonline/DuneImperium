using UnityEngine;

[CreateAssetMenu(fileName = "Troop", menuName = "ScriptableObjects/Resources/Troop")]
public class TroopDefinition : BaseResourceDefinition
{
    public override ResourceType ResourceType => ResourceType.Troop;
}