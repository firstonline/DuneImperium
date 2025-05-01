using System;
using System.Collections;
using UnityEngine;

public static class AllianceUtils
{
    public static void RecalculateAlliance(ref GameData gameData)
    {
        var houses = Enum.GetValues(typeof(House));

        foreach (House house in houses)
        {
            int currentPlayerWithAllianceIndex = -1;
            int highestInfluencePlayerIndex = 0;

            // find current player with alliance
            for (int i = 0; i < gameData.Players.Count; i++)
            {
                var player = gameData.Players[i];
                if (player.Alliances[house])
                {
                    currentPlayerWithAllianceIndex = i;
                }

                if (gameData.Players[highestInfluencePlayerIndex].Influences[house] < player.Influences[house])
                {
                    highestInfluencePlayerIndex = i;
                }
            }

            var highestInfluencePlayer = gameData.Players[highestInfluencePlayerIndex];

            if (currentPlayerWithAllianceIndex == -1)
            {
                if (highestInfluencePlayer.Influences[house] >= PlayerData.ALLIANCE_INFLUENCE)
                {
                    highestInfluencePlayer.Alliances[house] = true;
                    gameData.Players[highestInfluencePlayerIndex] = highestInfluencePlayer;
                }
            }
            else
            {
                var currentPlayerWithAlliance = gameData.Players[currentPlayerWithAllianceIndex];

                if (highestInfluencePlayer.Influences[house] > currentPlayerWithAlliance.Influences[house])
                {
                    currentPlayerWithAlliance.Alliances[house] = false;
                    highestInfluencePlayer.Alliances[house] = true;

                    gameData.Players[currentPlayerWithAllianceIndex] = currentPlayerWithAlliance;
                    gameData.Players[highestInfluencePlayerIndex] = highestInfluencePlayer;
                }
            }
        }
    }

}
