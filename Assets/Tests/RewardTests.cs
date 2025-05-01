using NUnit.Framework;
using System.Collections;
using System.Security.AccessControl;
using UnityEngine;

public class RewardTests : MonoBehaviour
{
    GameData _gameData;

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
    }


    [TestCase(new object[] { RewardActionTypes.AddSpice, 0, 2, 2 })]
    [TestCase(new object[] { RewardActionTypes.AddWater, 2, 2, 4 })]
    [TestCase(new object[] { RewardActionTypes.AddSolari, 2, 1, 3 })]
    public void Resource(RewardActionTypes rewardActionTypes, int haveCount, int addedCount, int resultCount)
    {
        ResourceType resourceType;
        switch (rewardActionTypes)
        {
            case RewardActionTypes.AddSpice: resourceType = ResourceType.Spice; break;
            case RewardActionTypes.AddWater: resourceType = ResourceType.Water; break;
            case RewardActionTypes.AddSolari: resourceType = ResourceType.Solari; break;
            default: throw new System.Exception("Invalid Type");
        }

        var reward = RewardBuilder.Build().WithAction(rewardActionTypes).WithQuantity(addedCount);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        playerData.Resources[resourceType] = haveCount;
        _gameData.Players[0] = playerData;


        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.AreEqual(playerData.Resources[resourceType], resultCount);
    }


    [TestCase(new object[] { RewardActionTypes.AddBenneGesseritInfluence, 0, 1, 1 })]
    [TestCase(new object[] { RewardActionTypes.AddFremenInfluence, 1, 1, 2 })]
    [TestCase(new object[] { RewardActionTypes.AddEmperorInfluence, 2, 1, 3 })]
    [TestCase(new object[] { RewardActionTypes.AddSpacingGuildInfluence, 3, 1, 4 })]
    [TestCase(new object[] { RewardActionTypes.AddFremenInfluence, PlayerData.MAX_INFLUENCE, 1, PlayerData.MAX_INFLUENCE })]
    [TestCase(new object[] { RewardActionTypes.AddSpacingGuildInfluence, PlayerData.MAX_INFLUENCE, 1, PlayerData.MAX_INFLUENCE })]
    public void Influence(RewardActionTypes rewardActionTypes, int haveCount, int addedCount, int resultCount)
    {
        House house;
        switch (rewardActionTypes)
        {
            case RewardActionTypes.AddFremenInfluence: house = House.Fremen; break;
            case RewardActionTypes.AddBenneGesseritInfluence: house = House.BeneGesserit; break;
            case RewardActionTypes.AddEmperorInfluence: house = House.Emperror; break;
            case RewardActionTypes.AddSpacingGuildInfluence: house = House.SpacingGuild; break;
            default: throw new System.Exception("Invalid Type");
        }

        var reward = RewardBuilder.Build().WithAction(rewardActionTypes).WithQuantity(addedCount);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        playerData.Influences[house] = haveCount;
        _gameData.Players[0] = playerData;

        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.AreEqual(playerData.Influences[house], resultCount);
    }

    [TestCase(new object[] { 0, 1, 1 })]
    [TestCase(new object[] { 3, 2, 5 })]
    [TestCase(new object[] { 3, 5, 8 })]
    [TestCase(new object[] { 8, 5, PlayerData.MAX_TROOPS })]
    [TestCase(new object[] { PlayerData.MAX_TROOPS, 1, PlayerData.MAX_TROOPS })]
    public void Troops(int haveCount, int addedCount, int resultCount)
    {
        var reward = RewardBuilder.Build().WithAction(RewardActionTypes.AddTroop).WithQuantity(addedCount);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        playerData.GarrisonedTroopsCount = haveCount;
        _gameData.Players[0] = playerData;

        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.AreEqual(playerData.GarrisonedTroopsCount, resultCount);
    }

    [TestCase(new object[] { 0, 1, 1 })]
    [TestCase(new object[] { 1, 2, 3 })]
    [TestCase(new object[] { 2, 3, 5 })]
    public void AddWorms(int haveCount, int addedCount, int resultCount)
    {
        var reward = RewardBuilder.Build().WithAction(RewardActionTypes.AddWorm).WithQuantity(addedCount);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        playerData.WormsCount = haveCount;
        _gameData.Players[0] = playerData;

        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.AreEqual(playerData.WormsCount, resultCount);
    }

    [Test]
    public void AddSwordsman()
    {
        var reward = RewardBuilder.Build().WithAction(RewardActionTypes.AddSwordsman);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        int prevAgentsCount = playerData.AgentsCount;
        _gameData.Players[0] = playerData;

        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.IsTrue(playerData.HaveSwordsman);
        Assert.AreEqual(playerData.AgentsCount, prevAgentsCount + 1);
    }

    [Test]
    public void AddCouncilSeat()
    {
        var reward = RewardBuilder.Build().WithAction(RewardActionTypes.AddCouncilSeat);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        _gameData.Players[0] = playerData;

        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.IsTrue(playerData.HasCouncilSeat);
    }

    [Test]
    public void AddMakerHook()
    {
        var reward = RewardBuilder.Build().WithAction(RewardActionTypes.AddMakerHook);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        _gameData.Players[0] = playerData;

        ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        Assert.IsTrue(playerData.HasMakerHook);
    }
}
