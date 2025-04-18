using UnityEngine;

[CreateAssetMenu(fileName = "Water", menuName = "ScriptableObjects/Resources/Water")]
public class Water : BaseResourceDefinition
{
    public override ResourceType ResourceType => ResourceType.Water;
}
