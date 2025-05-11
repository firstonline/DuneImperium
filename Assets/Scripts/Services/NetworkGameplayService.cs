using Codice.CM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UniDi;
using UniRx;
using Unity.Multiplayer.Widgets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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

public struct GameData : INetworkSerializable
{
    public int CurrentPlayerIndex;
    public List<PlayerData> Players;
    public int RandomData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {

        serializer.SerializeValue(ref RandomData);

        int length = 0;
        PlayerData[] Array;
        if (serializer.IsWriter)
        {
            Array = Players.ToArray();
            length = Array.Length;
        }
        else
        {
            Array = new PlayerData[length];
        }
        serializer.SerializeValue(ref length);
        serializer.SerializeValue(ref Array);

        if (serializer.IsReader)
        {
            Players = Array.ToList();
        }
    }
}

public class NetworkGameplayService : NetworkBehaviour
{
    [Inject] AreasService _areasService;
    [Inject] CardsDatabase _cardsDatabase;

    [SerializeField] string _connectionScreen;
    [SerializeField] Button _leaveSessionButton;
    [SerializeField] WidgetConfiguration _widgetConfiguration;

    NetworkVariable<GameData> _networkVariable = new NetworkVariable<GameData>();
    BehaviorSubject<GameData> _gameData = new BehaviorSubject<GameData>(new GameData());

    public int PlayersCount => _widgetConfiguration.MaxPlayers;
    public GameData GameData => _gameData.Value;
    public IObservable<GameData> ObserveGameData() => _gameData;
    public IObservable<PlayerData> ObservePlayerData(int index) => _gameData.Select(x =>
    {
        if (x.Players != null && x.Players.Count > index)
        {
            return x.Players[index];

        }
        else
        {
            return PlayerData.Construct();
        }
    });

    public IObservable<PlayerData> ObserveLocalPlayerData() => _gameData.Select(x =>
    {
        if (x.Players != null && x.Players.Any(x => x.IsTaken && x.ClientId == NetworkManager.Singleton.LocalClientId))
        {
            var localPlayerData = x.Players.First(x => x.IsTaken && x.ClientId == NetworkManager.Singleton.LocalClientId);
            return localPlayerData;
        }
        else
        {
            return PlayerData.Construct();
        }
    });


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton == null)
        {
            throw new Exception($"");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            var gameData = new GameData();
    
            gameData.Players = new List<PlayerData> {  };
            for (int i = 0; i < PlayersCount; i++)
            {
                var playerData = PlayerData.Construct(_cardsDatabase.StartingDeck.Select(x => x.ID).ToList());

                if (i == 0)
                {
                    playerData.IsTaken = true;
                }

                gameData.Players.Add(playerData);
            }

            _networkVariable.Value = gameData;
            _gameData.OnNext(gameData);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            _gameData.OnNext(_networkVariable.Value);
        }

        _networkVariable.OnValueChanged += (oldData, newData) => 
        {
            _gameData.OnNext(newData);
        };


        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public void UpdateGameData(GameData gameData)
    {
        if (NetworkManager.Singleton != null)
        {
            gameData.RandomData++;
            _networkVariable.Value = gameData;
        }
    }

    public void DeployTroops(int count)
    {
        DeployTroopsServerRpc(count);
    }

    [ServerRpc(RequireOwnership = false)]
    void DeployTroopsServerRpc(int count, ServerRpcParams rpcParams = default)
    {
        var gameData = GameData;
        int playerIndex = -1;

        for (int i = 0; i < gameData.Players.Count; i++)
        {
            if (gameData.Players[i].ClientId == rpcParams.Receive.SenderClientId)
            {
                playerIndex = i;
                break;
            }
        }

        if (playerIndex == -1)
        {
            return;
        }

        var playerData = gameData.Players[playerIndex];

        if (playerData.DeployableTroopsCount <= count)
        {
            playerData.DeployableTroopsCount -= count;
            playerData.GarrisonedTroopsCount -= count;
            playerData.DeployedTroopsCount += count;

            gameData.Players[playerIndex] = playerData;
            UpdateGameData(gameData);
        }
    }

    void OnClientConnected(ulong clientID)
    {
        var gameData = _networkVariable.Value;

        for (int i = 0; i < gameData.Players.Count; i++)
        {
            var player = gameData.Players[i];
            if (!player.IsTaken)
            {
                player.IsTaken = true;
                player.ClientId = clientID;
                gameData.Players[i] = player;
                gameData.RandomData++;
                _networkVariable.Value = gameData;
                break;
            }
        }
    }

    void OnClientDisconnected(ulong clientID)
    {
        Debug.Log($"Client Disconnected {clientID}");

        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                _leaveSessionButton.onClick.Invoke();
            }
            SceneManager.LoadSceneAsync(_connectionScreen);
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            var gameData = _networkVariable.Value;

            for (int i = 0; i < gameData.Players.Count; i++)
            {
                var player = gameData.Players[i];
                if (player.ClientId == clientID)
                {
                    player.IsTaken = false;
                    gameData.Players[i] = player;
                    gameData.RandomData++;
                    _networkVariable.Value = gameData;
                    break;
                }
            }
        }
    }

    
}
