using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class PlayerData : IEquatable<PlayerData>
{
    public int RandomData;

    public bool Equals(PlayerData other)
    {
        return other.RandomData == RandomData;
    }
}

public class Player : NetworkBehaviour
{
    NetworkVariable<int> _networkVariable = new NetworkVariable<int>();

    void Awake()
    {
        _networkVariable.OnValueChanged += (prevData, newData) =>
        {
            Debug.Log($"Data Changed {newData}");
        };
    }

    public void Testing()
    {
        ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void ServerRpc()
    {
        _networkVariable.Value++;
    }

}
