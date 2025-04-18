using UnityEngine;

[CreateAssetMenu(fileName = "Influence", menuName = "ScriptableObjects/Resources/Influence")]
public class InfluenceDefinition : BaseResourceDefinition
{
    public InfluenceFaction InfluenceFaction;
    public override ResourceType ResourceType => ResourceType.Influence;
}

