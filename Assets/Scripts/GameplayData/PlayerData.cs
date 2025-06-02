using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Netcode;

public enum ResourceType
{
    Water,
    Solari,
    Spice,
}

public enum House
{
    Fremen = 0,
    Emperror = 1,
    BeneGesserit = 2,
    SpacingGuild = 3,
}

public struct PlayerData : INetworkSerializable
{
    public const int MAX_TROOPS = 12;
    public const int MAX_INFLUENCE = 6;
    public const int ALLIANCE_INFLUENCE = 4;

    public bool IsTaken;
    public ulong ClientId;

    public bool HasMakerHook;
    public bool HasCouncilSeat;
    public bool HaveSwordsman;
    public bool CanDeploy;
    public bool PerformedAction;
    public bool Revealed;

    public int AgentsCount;
    public int DeployedAgentsCount;
    public int DeployableTroopsCount;
    public int GarrisonedTroopsCount;
    public int DeployedTroopsCount;
    public int WormsCount;
    public List<int> Cards;
    public Dictionary<House, bool> Alliances;
    public Dictionary<House, int> Influences;
    public Dictionary<ResourceType, int> Resources;

    public int CombatStrength => WormsCount * 3 + DeployedTroopsCount * 2;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref IsTaken);
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref HasMakerHook);
        serializer.SerializeValue(ref HasCouncilSeat);
        serializer.SerializeValue(ref HaveSwordsman);
        serializer.SerializeValue(ref CanDeploy);
        serializer.SerializeValue(ref PerformedAction);
        serializer.SerializeValue(ref Revealed);
        serializer.SerializeValue(ref DeployableTroopsCount);
        serializer.SerializeValue(ref AgentsCount);
        serializer.SerializeValue(ref DeployedAgentsCount);
        serializer.SerializeValue(ref GarrisonedTroopsCount);
        serializer.SerializeValue(ref DeployedTroopsCount);
        serializer.SerializeValue(ref WormsCount);

        Resources = NetworkSerializationUtils.SerializeToDictionary<T, ResourceType>(serializer, Resources);
        Alliances = NetworkSerializationUtils.SerializeToDictionary<T, House>(serializer, Alliances);
        Influences = NetworkSerializationUtils.SerializeToDictionary<T, House>(serializer, Influences);

        if (serializer.IsWriter)
        {
            int[] cards = Cards.ToArray();
            serializer.SerializeValue(ref cards);
        }
        else
        {
            int[] cards = new int[0];
            serializer.SerializeValue(ref cards);
            Cards = cards.ToList();
        }
    }

    public static PlayerData Construct(List<int> initialDeck = null)
    {
        var playerData = new PlayerData();
        playerData.AgentsCount = 2;

        var resources = new Dictionary<ResourceType, int>();
        var itemTypes = Enum.GetValues(typeof(ResourceType));
        foreach (var itemType in itemTypes)
        {
            resources.Add((ResourceType)itemType, 0);
        }
        playerData.Resources = resources;
        playerData.Resources[ResourceType.Water]++;

        var houses = Enum.GetValues(typeof(House));
        var alliances = new Dictionary<House, bool>();
        var influences = new Dictionary<House, int>();

        foreach (var house in houses)
        {
            alliances.Add((House)house, false);
            influences.Add((House)house, 0);
        }
        playerData.Alliances = alliances;
        playerData.Influences = influences;

        playerData.Cards = new List<int>();

        if (initialDeck != null)
        {
            playerData.Cards.AddRange(initialDeck);
        }

        return playerData;
    }
}