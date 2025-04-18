using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentAreaDefinition", menuName = "ScriptableObjects/AgentAreaDefinition")]
public class AgentAreaDefinition : ScriptableObject, IDatabaseItem
{
    [SerializeField] int _id;
    public string Name;
    public List<Exchange> Exchanges;

    public int ID { get { return _id; } set { _id = value; } }
}
