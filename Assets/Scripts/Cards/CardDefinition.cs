using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "ScriptableObjects/Cards/CardDefinition")]
public class CardDefinition : ScriptableObject, IDatabaseItem
{
    [SerializeField] int _id;
    public string Name;
    public Sprite Icon;
    public List<AgentIconDefinition> AgentIcons;
    public ExchangeDefinition PlayExchanges;
    public ExchangeDefinition RevealExchange;

    public int ID { get => _id; set => _id = value; }
}
