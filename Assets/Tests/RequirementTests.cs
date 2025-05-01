using NUnit.Framework;

public class RequirementsTests
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

    [TestCase(new object[] { RequirementActionTypes.RequireFrementInfluence, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireFrementInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireFrementInfluence, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireBeneGesseritInfluence, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireBeneGesseritInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireBeneGesseritInfluence, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireEmperorInfluence, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireEmperorInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireEmperorInfluence, 4, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildInfluence, 0, 2 }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildInfluence, 2, 2 }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildInfluence, 4, 2 }, ExpectedResult = true)]
    public bool Influence(RequirementActionTypes influenceRequirement, int haveCount, int requiredCount)
    {
        House house;
        switch (influenceRequirement)
        {
            case RequirementActionTypes.RequireFrementInfluence:
                house = House.Fremen;
                break;
            case RequirementActionTypes.RequireBeneGesseritInfluence:
                house = House.BeneGesserit;
                break;
            case RequirementActionTypes.RequireEmperorInfluence:
                house = House.Emperror;
                break;
            case RequirementActionTypes.RequireSpacingGuildInfluence:
                house = House.SpacingGuild;
                break;
            default:
                throw new System.Exception("Invalid type");
        }

        var requirement = RequirementBuilder.Build().WithAction(influenceRequirement).WithQuantity(requiredCount);
        var playerData = _gameData.Players[0];
        playerData.Influences[house] = haveCount;
        _gameData.Players[0] = playerData;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);
    }

    [TestCase(new object[] { RequirementActionTypes.RequireFremenAlliance, false, false, false, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireFremenAlliance, true, false, false, false }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireFremenAlliance, false, true, false, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireBeneGesseritAlliance, false, false, false, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireBeneGesseritAlliance, false, true, false, false }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireBeneGesseritAlliance, false, false, true, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireEmperorAlliance, false, false, false, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireEmperorAlliance, false, false, false, true }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireEmperorAlliance, true, false, false, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildAlliance, false, false, false, false }, ExpectedResult = false)]
    [TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildAlliance, false, false, true, false }, ExpectedResult = true)]
    [TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildAlliance, false, false, false, true }, ExpectedResult = false)]
    public bool Alliance(RequirementActionTypes allianceRequirement, bool fremen, bool beneGesserit, bool spacingGuild, bool emperror)
    {
        House house;
        switch (allianceRequirement)
        {
            case RequirementActionTypes.RequireFremenAlliance:
                house = House.Fremen;
                break;
            case RequirementActionTypes.RequireBeneGesseritAlliance:
                house = House.BeneGesserit;
                break;
            case RequirementActionTypes.RequireEmperorAlliance:
                house = House.Emperror;
                break;
            case RequirementActionTypes.RequireSpacingGuildAlliance:
                house = House.SpacingGuild;
                break;
            default:
                throw new System.Exception("Invalid type");
        }

        var requirement = RequirementBuilder.Build().WithAction(allianceRequirement);
        var playerData = _gameData.Players[0];
        playerData.Alliances[House.Fremen] = fremen;
        playerData.Alliances[House.BeneGesserit] = beneGesserit;
        playerData.Alliances[House.SpacingGuild] = spacingGuild;
        playerData.Alliances[House.Emperror] = emperror;


        _gameData.Players[0] = playerData;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);
    }

    //[TestCase(new object[] { RequirementActionTypes.RequireFremenCardInPlay, 0, 1 }, ExpectedResult = false)]
    //[TestCase(new object[] { RequirementActionTypes.RequireFremenCardInPlay, 1, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireFremenCardInPlay, 2, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireBenneGesseritCardInPlay, 0, 1 }, ExpectedResult = false)]
    //[TestCase(new object[] { RequirementActionTypes.RequireBenneGesseritCardInPlay, 1, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireBenneGesseritCardInPlay, 2, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildCardInPlay, 0, 1 }, ExpectedResult = false)]
    //[TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildCardInPlay, 1, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireSpacingGuildCardInPlay, 2, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireEmperorCardInPlay, 0, 1 }, ExpectedResult = false)]
    //[TestCase(new object[] { RequirementActionTypes.RequireEmperorCardInPlay, 1, 1 }, ExpectedResult = true)]
    //[TestCase(new object[] { RequirementActionTypes.RequireEmperorCardInPlay, 2, 1 }, ExpectedResult = true)]
    //public bool FactionCardInPlay()
    //{
    //    return false;
    //}

    [TestCase(new object[] { false }, ExpectedResult = false)]
    [TestCase(new object[] { true }, ExpectedResult = true)]
    public bool MakerHook(bool haveMakerHook)
    {
        var requirement = RequirementBuilder.Build().WithAction(RequirementActionTypes.RequireMakerHook);
        var playerData = _gameData.Players[0];
        playerData.HasMakerHook = haveMakerHook;
        _gameData.Players[0] = playerData;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);
    }

    [TestCase(new object[] { false, false }, ExpectedResult = false)]
    [TestCase(new object[] { true, false }, ExpectedResult = false)]
    [TestCase(new object[] { false, true }, ExpectedResult = true)]
    public bool HaveSwordsman(bool player1HaveSwordsman, bool player2HaveSwordsman)
    {
        var requirement = RequirementBuilder.Build().WithAction(RequirementActionTypes.RequireSwordsman).WithQuantity(0);
        var playerData1 = _gameData.Players[0];
        playerData1.HaveSwordsman = player1HaveSwordsman;

        var playerData2 = _gameData.Players[1];
        playerData2.HaveSwordsman = player2HaveSwordsman;

        _gameData.Players[0] = playerData1;
        _gameData.Players[1] = playerData2;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);
    }

    [TestCase(new object[] { false, false }, ExpectedResult = true)]
    [TestCase(new object[] { true, false }, ExpectedResult = false)]
    [TestCase(new object[] { false, true }, ExpectedResult = false)]
    public bool HaveNoSwordsman(bool player1HaveSwordsman, bool player2HaveSwordsman)
    {
        var requirement = RequirementBuilder.Build().WithAction(RequirementActionTypes.RequireNoSwordsman).WithQuantity(0);
        var playerData1 = _gameData.Players[0];
        playerData1.HaveSwordsman = player1HaveSwordsman;

        var playerData2 = _gameData.Players[1];
        playerData2.HaveSwordsman = player2HaveSwordsman;

        _gameData.Players[0] = playerData1;
        _gameData.Players[1] = playerData2;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);
    }


    [TestCase(new object[] { false }, ExpectedResult = false)]
    [TestCase(new object[] { true }, ExpectedResult = true)]
    public bool HaveCouncilSeat(bool haveSeat)
    {
        var requirement = RequirementBuilder.Build().WithAction(RequirementActionTypes.RequireCouncilSeat);
        var playerData = _gameData.Players[0];
        playerData.HasCouncilSeat = haveSeat;
        _gameData.Players[0] = playerData;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);

    }


    [TestCase(new object[] { false }, ExpectedResult = true)]
    [TestCase(new object[] { true }, ExpectedResult = false)]
    public bool HaveNoCouncilSeat(bool haveSeat)
    {
        var requirement = RequirementBuilder.Build().WithAction(RequirementActionTypes.RequireNoCouncilSeat).WithQuantity(0);
        var playerData = _gameData.Players[0];
        playerData.HasCouncilSeat = haveSeat;
        _gameData.Players[0] = playerData;

        return ExchangeHelper.CheckRequirement(_gameData, _gameData.Players[0], requirement);
    }
}
