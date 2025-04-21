using System;
using UnityEngine;

public class BaseActionDefinition<T> : ScriptableObject, IDatabaseItem where T : System.Enum
{
    public T Type;
    public Sprite Icon;
    public string Explanation;

    public int ID { get => Convert.ToInt32(Type); set { } }
}
