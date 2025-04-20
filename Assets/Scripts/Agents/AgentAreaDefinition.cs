using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentAreaDefinition", menuName = "ScriptableObjects/AgentAreaDefinition")]
public class AgentAreaDefinition : ScriptableObject, IDatabaseItem
{
    [SerializeField] int _id;
    public string Name;
    public AgentIconDefinition AgentIcon;
    public RewardDefinition Reveal;
    public List<ExchangeDefinition> Exchanges;
    public bool IsMakerBoard;
    public bool IsCombatArea;
    public ExchangeDefinition ImperialFlagReward;

    public int ID { get { return _id; } set { _id = value; } }
}
