using UnityEngine;

public abstract class GameElementDefinition : ScriptableObject, IDatabaseItem
{
    [SerializeField] int _id;
    public Sprite Icon;

    public int ID { get { return _id; } set { _id = value; } }
}
