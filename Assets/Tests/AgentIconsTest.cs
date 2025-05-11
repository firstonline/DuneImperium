using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class AgentIconsTest : MonoBehaviour
{
    GameData _gameData;
     AgentIconDefinition _agentIcon1;
     AgentIconDefinition _agentIcon2;
     AgentIconDefinition _agentIcon3;

     CardDefinition _card1;
     CardDefinition _card2;
     CardDefinition _card3;
     CardDefinition _card4;
     CardDefinition _card5;
     CardDefinition _card6;
     CardDefinition _card7;
     AgentAreaDefinition _agentArea1;
     AgentAreaDefinition _agentArea2;
     AgentAreaDefinition _agentArea3;

    [SetUp]
    public void Setup()
    {
        _gameData = new GameData();
        _gameData.Players = new()
        {
            PlayerData.Construct(),
            PlayerData.Construct(),
            PlayerData.Construct(),
            PlayerData.Construct()
        };
        _agentIcon1 = ScriptableObject.CreateInstance<AgentIconDefinition>();
        _agentIcon2 = ScriptableObject.CreateInstance<AgentIconDefinition>();
        _agentIcon3 = ScriptableObject.CreateInstance<AgentIconDefinition>();

        _card1 = CardBuilder.Build().WithId(0).WithIcon(_agentIcon1);
        _card2 = CardBuilder.Build().WithId(1).WithIcon(_agentIcon2);
        _card3 = CardBuilder.Build().WithId(2).WithIcon(_agentIcon3);
        _card4 = CardBuilder.Build().WithId(3).WithIcon(_agentIcon1).WithIcon(_agentIcon2);
        _card5 = CardBuilder.Build().WithId(4).WithIcon(_agentIcon2).WithIcon(_agentIcon3);
        _card6 = CardBuilder.Build().WithId(5).WithIcon(_agentIcon1).WithIcon(_agentIcon3);
        _card7 = CardBuilder.Build().WithId(6).WithIcon(_agentIcon1).WithIcon(_agentIcon2).WithIcon(_agentIcon3);

        _agentArea1 = AgentAreaBuilder.Build().WithAgentIcon(_agentIcon1);
        _agentArea2 = AgentAreaBuilder.Build().WithAgentIcon(_agentIcon2);
        _agentArea3 = AgentAreaBuilder.Build().WithAgentIcon(_agentIcon3);
    }

    [Test]
    public void Test1()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>();
        Assert.IsFalse(ExchangeHelper.CanVisitArea(_card1, _gameData, playerData, _agentArea1));
    }

    [Test]
    public void Test2()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card2.ID, _card3.ID};
        Assert.IsFalse(ExchangeHelper.CanVisitArea(_card1, _gameData, playerData, _agentArea1));
    }

    [Test]
    public void Test3()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card1.ID, _card2.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card1, _gameData, playerData, _agentArea1));
    }

    [Test]
    public void Test4()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card1.ID, _card2.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card2, _gameData, playerData, _agentArea2));
    }

    [Test]
    public void Test5()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card3.ID, _card2.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card3, _gameData, playerData, _agentArea3));
    }

    [Test]
    public void Test6()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card4.ID };
        Assert.IsFalse(ExchangeHelper.CanVisitArea(_card4, _gameData, playerData, _agentArea3));
    }

    [Test]
    public void Test7()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card4.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card4, _gameData, playerData, _agentArea1));
    }

    [Test]
    public void Test8()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card4.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card4, _gameData, playerData, _agentArea2));
    }

    [Test]
    public void Test9()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card5.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card5, _gameData, playerData, _agentArea3));
    }

    [Test]
    public void Test10()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card6.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card6, _gameData, playerData, _agentArea3));
    }

    [Test]
    public void Test11()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card7.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card7, _gameData, playerData, _agentArea1));
    }
    [Test]
    public void Test12()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card7.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card7, _gameData, playerData, _agentArea2));
    }
    [Test]
    public void Test13()
    {
        var playerData = _gameData.Players[0];
        playerData.Cards = new List<int>() { _card7.ID };
        Assert.IsTrue(ExchangeHelper.CanVisitArea(_card7, _gameData, playerData, _agentArea3));
    }
}
