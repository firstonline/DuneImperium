using UnityEngine;

[CreateAssetMenu(fileName = "Worm", menuName = "ScriptableObjects/Resources/Worm")]
public class WormDefinition : BaseResourceDefinition
{
    public override ResourceType ResourceType => ResourceType.Worm;
}
