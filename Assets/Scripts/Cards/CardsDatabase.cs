using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsDatabase", menuName = "ScriptableObjects/Cards/CardsDatabase")]
public class CardsDatabase : BaseDatabase<CardDefinition>
{
    public List<CardDefinition> StartingDeck;
}
