using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public struct GameData : INetworkSerializable
{
    public GameState GameState;
    public int FirstPlayerIndex;
    public int CurrentPlayerIndex;
    public List<PlayerData> Players;
    public int Iterator;
    public int LastPlayedCombatPlayer;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Iterator);
        serializer.SerializeValue(ref FirstPlayerIndex);
        serializer.SerializeValue(ref GameState);

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