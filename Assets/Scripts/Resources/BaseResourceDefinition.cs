using UnityEngine;

//[CreateAssetMenu(fileName = "SUPER NEAT THING", menuName = "SUPER/NEAT/THING")]
public abstract class BaseResourceDefinition : ScriptableObject, IDatabaseItem
{
    [SerializeField] int _id;
    public Sprite Icon;
    public abstract ResourceType ResourceType { get;}

    public int ID { get { return _id; } set { _id = value; } }
}
