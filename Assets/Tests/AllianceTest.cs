using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;

public class AllianceTest : MonoBehaviour
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

    public class InfluenceIncreaseSource
    {
        public static IEnumerable Cases
        {
            get
            {
                var houses = Enum.GetValues(typeof(House));
                foreach (var house in houses)
                {
                    RewardActionTypes rewardAction = RewardActionTypes.AddFremenInfluence;
                    switch (house)
                    {
                        case House.Fremen: rewardAction = RewardActionTypes.AddFremenInfluence;  break;
                        case House.BeneGesserit: rewardAction = RewardActionTypes.AddBenneGesseritInfluence;  break;
                        case House.SpacingGuild: rewardAction = RewardActionTypes.AddSpacingGuildInfluence;  break;
                        case House.Emperror: rewardAction = RewardActionTypes.AddEmperorInfluence;  break;
                    }

                    yield return new TestCaseData(rewardAction, house, 0).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 1).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 2).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 3).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 4).Returns(true);
                    yield return new TestCaseData(rewardAction, house, 5).Returns(true);
                    yield return new TestCaseData(rewardAction, house, 6).Returns(true);
                }
            }
        }
    }

    [TestCaseSource(typeof(InfluenceIncreaseSource), nameof(InfluenceIncreaseSource.Cases))]
    public bool InfluenceIncrease(RewardActionTypes action, House house, int quantity)
    {
        var reward = RewardBuilder.Build().WithAction(action).WithQuantity(1);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        var playerData = _gameData.Players[0];
        for (int i = 0; i < quantity; i++)
        {
            ExchangeHelper.ReceiveRewards(ref playerData, exchange);
        }
        _gameData.Players[0] = playerData;
        AllianceUtils.RecalculateAlliance(ref _gameData);

        return playerData.Alliances[house];
    }

    public class InfluenceOvertakeSource
    {
        public static IEnumerable Cases
        {
            get
            {
                var houses = Enum.GetValues(typeof(House));
                foreach (var house in houses)
                {
                    RewardActionTypes rewardAction = RewardActionTypes.AddFremenInfluence;
                    switch (house)
                    {
                        case House.Fremen: 
                            rewardAction = RewardActionTypes.AddFremenInfluence;
                            break;
                        case House.BeneGesserit: 
                            rewardAction = RewardActionTypes.AddBenneGesseritInfluence;
                            break;
                        case House.SpacingGuild: 
                            rewardAction = RewardActionTypes.AddSpacingGuildInfluence; 
                            break;
                        case House.Emperror: 
                            rewardAction = RewardActionTypes.AddEmperorInfluence; 
                            break;
                    }

                    yield return new TestCaseData(rewardAction, house, 4, 0).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 4, 2).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 4, 3).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 4, 4).Returns(false);
                    yield return new TestCaseData(rewardAction, house, 4, 5).Returns(true);
                    yield return new TestCaseData(rewardAction, house, 5, 6).Returns(true);
                    yield return new TestCaseData(rewardAction, house, 6, 6).Returns(false);
                }
            }
        }
    }

    [TestCaseSource(typeof(InfluenceOvertakeSource), nameof(InfluenceOvertakeSource.Cases))]
    public bool Overtaking(RewardActionTypes action, House house, int player1Influence, int player2Influence)
    {
        var player1 = _gameData.Players[0];
        var player2 = _gameData.Players[1];

        player1.Influences[house] = player1Influence;
        _gameData.Players[0] = player1;

        AllianceUtils.RecalculateAlliance(ref _gameData);
        Assert.IsTrue(player1.Alliances[house]);
        Assert.IsFalse(player2.Alliances[house]);

        var reward = RewardBuilder.Build().WithAction(action).WithQuantity(1);
        var exchange = ExchangeBuilder.Build().WithReward(reward);
        for (int i = 0; i < player2Influence; i++)
        {
            ExchangeHelper.ReceiveRewards(ref player2, exchange);
        }
        _gameData.Players[1] = player2;
        AllianceUtils.RecalculateAlliance(ref _gameData);

        return player2.Alliances[house];
    }


}
