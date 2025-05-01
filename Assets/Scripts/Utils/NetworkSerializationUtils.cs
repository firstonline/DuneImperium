using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting.YamlDotNet.Serialization;

public static class NetworkSerializationUtils
{
    public static Dictionary<Key, int> SerializeToDictionary<T, Key>(BufferSerializer<T> serializer, Dictionary<Key, int> dictionary) 
        where T : IReaderWriter
        where Key : Enum
    {
        int length = 0;
        int[] keys;
        int[] values;

        if (serializer.IsWriter)
        {
            keys = dictionary.Keys.Select(x => (int)Enum.ToObject(typeof(Key), x)).ToArray();
            values = dictionary.Values.ToArray();
            length = dictionary.Count;
        }
        else
        {
            keys = new int[length];
            values = new int[length];
        }

        serializer.SerializeValue(ref length);
        serializer.SerializeValue(ref keys);
        serializer.SerializeValue(ref values);

        if (serializer.IsReader)
        {
            dictionary = new Dictionary<Key, int>();
            for (int i = 0; i < length; i++)
            {
                dictionary.Add((Key)Enum.ToObject(typeof(Key), keys[i]), values[i]);
            }
        }
        return dictionary;
    }

    public static Dictionary<Key, bool> SerializeToDictionary<T, Key>(BufferSerializer<T> serializer, Dictionary<Key, bool> dictionary)
        where T : IReaderWriter
        where Key : Enum
    {
        int length = 0;
        int[] keys;
        bool[] values;

        if (serializer.IsWriter)
        {
            keys = dictionary.Keys.Select(x => (int)Enum.ToObject(typeof(Key), x)).ToArray();
            values = dictionary.Values.ToArray();
            length = dictionary.Count;
        }
        else
        {
            keys = new int[length];
            values = new bool[length];
        }

        serializer.SerializeValue(ref length);
        serializer.SerializeValue(ref keys);
        serializer.SerializeValue(ref values);

        if (serializer.IsReader)
        {
            dictionary = new Dictionary<Key, bool>();
            for (int i = 0; i < length; i++)
            {
                dictionary.Add((Key)Enum.ToObject(typeof(Key), keys[i]), values[i]);
            }
        }
        return dictionary;
    }
}
