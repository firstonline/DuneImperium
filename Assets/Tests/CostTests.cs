
using NUnit.Framework;
using System.Security.AccessControl;

public class CostTests 
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

    [TestCase(new object[] { CostActionTypes.RemoveSpice, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveSpice, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveSpice, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveWater, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveWater, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveWater, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveSolari, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveSolari, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveSolari, 4, 2 }, ExpectedResult = true)]
    public bool HaveResources(CostActionTypes resourceCost, int haveCount, int requiredCount)
    {
        ResourceType resourceType;
        switch (resourceCost)
        {
            case CostActionTypes.RemoveSpice: resourceType = ResourceType.Spice; break;
            case CostActionTypes.RemoveWater: resourceType = ResourceType.Water; break;
            case CostActionTypes.RemoveSolari: resourceType = ResourceType.Solari; break;
            default:
                throw new System.Exception("Invalid type");
        }

        var cost = CostBuilder.Build().WithAction(resourceCost).WithQuantity(requiredCount);
        var exchange = ExchangeBuilder.Build().WithCost(cost);

        var playerData = _gameData.Players[0];
        playerData.Resources[resourceType] = haveCount;
        _gameData.Players[0] = playerData;

        return ExchangeHelper.CanPayCost(_gameData.Players[0], exchange);
    }


    [TestCase(new object[] { CostActionTypes.RemoveBenneGesseritInfluence, 0, 1 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveBenneGesseritInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveBenneGesseritInfluence, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveFremenInfluence, 0, 1 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveFremenInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveFremenInfluence, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveSpacingGuildInfluence, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveSpacingGuildInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveSpacingGuildInfluence, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveEmperorInfluence, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { CostActionTypes.RemoveEmperorInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { CostActionTypes.RemoveEmperorInfluence, 4, 2 }, ExpectedResult = true)]
    public bool HaveInfluence(CostActionTypes influenceCost, int haveCount, int requiredCount)
    {
        House house;
        switch (influenceCost)
        {
            case CostActionTypes.RemoveBenneGesseritInfluence: house = House.BeneGesserit; break;
            case CostActionTypes.RemoveFremenInfluence: house = House.Fremen; break;
            case CostActionTypes.RemoveSpacingGuildInfluence: house = House.SpacingGuild; break;
            case CostActionTypes.RemoveEmperorInfluence: house = House.Emperror; break;
            default:
                throw new System.Exception("Invalid type");
        }

        var cost = CostBuilder.Build().WithAction(influenceCost).WithQuantity(requiredCount);
        var exchange = ExchangeBuilder.Build().WithCost(cost);

        var playerData = _gameData.Players[0];
        playerData.Influences[house] = haveCount;
        _gameData.Players[0] = playerData;

        return ExchangeHelper.CanPayCost(_gameData.Players[0], exchange);
    }

    [TestCase(new object[] { 0, 0, 0, 0, 1 }, ExpectedResult = false)]
    [TestCase(new object[] { 0, 1, 0, 0, 1 }, ExpectedResult = true)]
    [TestCase(new object[] { 0, 0, 1, 0, 1 }, ExpectedResult = true)]
    [TestCase(new object[] { 0, 0, 0, 1, 1 }, ExpectedResult = true)]
    [TestCase(new object[] { 0, 0, 0, 1, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { 1, 0, 0, 1, 2 }, ExpectedResult = true)]
    public bool HaveAnyInfluence(int fremen, int beneGesserit, int spacingGuild, int emperror, int requiredCount)
    {
        var cost = CostBuilder.Build().WithAction(CostActionTypes.RemoveAnyInfluence).WithQuantity(requiredCount);
        var exchange = ExchangeBuilder.Build().WithCost(cost);

        var playerData = _gameData.Players[0];
        playerData.Influences[House.Fremen] = fremen;
        playerData.Influences[House.BeneGesserit] = beneGesserit;
        playerData.Influences[House.SpacingGuild] = spacingGuild;
        playerData.Influences[House.Emperror] = emperror;

        _gameData.Players[0] = playerData;

        return ExchangeHelper.CanPayCost(_gameData.Players[0], exchange);
    }

    [TestCase(new object[] { CostActionTypes.RemoveSpice, 2, 2, 0})]
    [TestCase(new object[] { CostActionTypes.RemoveWater, 2, 2, 0})]
    [TestCase(new object[] { CostActionTypes.RemoveSolari, 2, 1, 1})]
    public void PayResource(CostActionTypes resourceCost, int haveCount, int costCount, int resultCount)
    {
        ResourceType resourceType;
        switch (resourceCost)
        {
            case CostActionTypes.RemoveSpice: resourceType = ResourceType.Spice; break;
            case CostActionTypes.RemoveWater: resourceType = ResourceType.Water; break;
            case CostActionTypes.RemoveSolari: resourceType = ResourceType.Solari; break;
            default:
                throw new System.Exception("Invalid type");
        }

        var cost = CostBuilder.Build().WithAction(resourceCost).WithQuantity(costCount);
        var exchange = ExchangeBuilder.Build().WithCost(cost);

        var playerData = _gameData.Players[0];
        playerData.Resources[resourceType] = haveCount;
        _gameData.Players[0] = playerData;

        Assert.IsTrue(ExchangeHelper.CanPayCost(_gameData.Players[0], exchange), "Cannot Pay");

        ExchangeHelper.PayCost(ref playerData, exchange);
        Assert.AreEqual(playerData.Resources[resourceType], resultCount);
    }



    [TestCase(new object[] { CostActionTypes.RemoveBenneGesseritInfluence, 2, 1, 1 })]
    [TestCase(new object[] { CostActionTypes.RemoveFremenInfluence, 2, 1, 1 })]
    [TestCase(new object[] { CostActionTypes.RemoveEmperorInfluence, 2, 1, 1 })]
    [TestCase(new object[] { CostActionTypes.RemoveSpacingGuildInfluence, 2, 1, 1 })]
    public void PayInfluence(CostActionTypes influenceCost, int haveCount, int costCount, int resultCount)
    {
        House house;
        switch (influenceCost)
        {
            case CostActionTypes.RemoveBenneGesseritInfluence: house = House.BeneGesserit; break;
            case CostActionTypes.RemoveFremenInfluence: house = House.Fremen; break;
            case CostActionTypes.RemoveSpacingGuildInfluence: house = House.SpacingGuild; break;
            case CostActionTypes.RemoveEmperorInfluence: house = House.Emperror; break;
            default:
                throw new System.Exception("Invalid type");
        }

        var cost = CostBuilder.Build().WithAction(influenceCost).WithQuantity(costCount);
        var exchange = ExchangeBuilder.Build().WithCost(cost);

        var playerData = _gameData.Players[0];
        playerData.Influences[house] = haveCount;
        _gameData.Players[0] = playerData;

        Assert.IsTrue(ExchangeHelper.CanPayCost(_gameData.Players[0], exchange), "Cannot Pay");

        ExchangeHelper.PayCost(ref playerData, exchange);
        Assert.AreEqual(playerData.Influences[house], resultCount);
    }
}
