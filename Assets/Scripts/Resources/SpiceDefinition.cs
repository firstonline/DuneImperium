using UnityEngine;

[CreateAssetMenu(fileName = "Spice", menuName = "ScriptableObjects/Resources/Spice")]
public class SpiceDefinition : BaseResourceDefinition
{
    public override ResourceType ResourceType => ResourceType.Spice;
}
